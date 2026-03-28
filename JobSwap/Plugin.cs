using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ECommons;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Client.Graphics.Render;

namespace JobSwap;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IPlayerState PlayerState { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;
    [PluginService] internal static IFramework Framework { get; private set; } = null!;
    [PluginService] internal static IObjectTable ObjectTable { get; private set; } = null!;

    private const string CommandName = "/jobswap";
    public void ToggleConfigUi() => ConfigWindow.Toggle();

    public Configuration Configuration { get; init; }
    private MainWindow MainWindow { get; init; } = null!;
    private ConfigWindow ConfigWindow { get; init; } = null!;
    
    private AutoDutyIPC AutoDutyIPC;
    public void StopAutoDuty()
    {
        AutoDutyIPC.Stop();
        AutoDutyIPC.SetConfig("StopLevel", "False");
    }
    public void SetAutodutyStopLevel(int level)
    {
        AutoDutyIPC.SetConfig("StopLevel", "True");
        AutoDutyIPC.SetConfig("StopLevelInt", level.ToString());
    }

    private int queueIndex = 0;
    private double timeSinceLastCheck = 0;
    private double swapDelay = 0;
    private double startDelay = 0;
    public readonly WindowSystem WindowSystem = new("JobSwap");
    public Plugin()
    {
        ECommonsMain.Init(PluginInterface, this);
        AutoDutyIPC = new AutoDutyIPC();

        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        
        MainWindow = new MainWindow(this);
        WindowSystem.AddWindow(MainWindow);

        ConfigWindow = new ConfigWindow(this);
        WindowSystem.AddWindow(ConfigWindow);  
        
        Framework.Update += OnUpdate;
 
        PluginInterface.UiBuilder.Draw += WindowSystem.Draw;
        PluginInterface.UiBuilder.OpenMainUi += () => MainWindow.Toggle();
        PluginInterface.UiBuilder.OpenConfigUi += () => ConfigWindow.Toggle();

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Opens the JobSwap window"
        });

        Log.Information("JobSwap has started");
    }

    public void Dispose()
    {
        ECommonsMain.Dispose();

        Framework.Update -= OnUpdate;
        PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
        PluginInterface.UiBuilder.OpenMainUi -= () => MainWindow.Toggle();
        PluginInterface.UiBuilder.OpenConfigUi -= () => ConfigWindow.Toggle();

        CommandManager.RemoveHandler(CommandName);
        
        WindowSystem.RemoveAllWindows();
    }

    private void OnCommand(string command, string args)
    {
        MainWindow.Toggle();
    }

    public void StartQueue()
    {
        queueIndex = 0;
        timeSinceLastCheck = 14.5;
        Configuration.IsRunning = true;
        Configuration.Save();
        swapDelay = 0;
        startDelay = 0;
    }

private void OnUpdate(IFramework framework)
{
    if (!Configuration.IsRunning) return;
    if (Configuration.GearsetNumbers.Count == 0) return;

    if (startDelay > 0)
    {
        startDelay -= framework.UpdateDelta.TotalSeconds;
        if (startDelay <= 0)
        {
            Log.Information($"StartDelay fired, starting Autoduty on queueIndex: {queueIndex}, gearset: {Configuration.GearsetNumbers[queueIndex]}");
            AutoDutyIPC.Start(true);
            AutoDutyIPC.SetConfig("LoopTimes", "99");
        }
        return;
    }

    if (swapDelay > 0)
    {
        swapDelay -= framework.UpdateDelta.TotalSeconds;
        if (swapDelay <= 0)
        {
            Log.Information($"SwapDelay fired - level: {ObjectTable.LocalPlayer?.Level}, target: {Configuration.RequestedLevel}, queueIndex: {queueIndex}");
            if (ObjectTable.LocalPlayer?.Level >= Configuration.RequestedLevel)
            {
                queueIndex++;
                Log.Information($"Level met, advancing queue to index: {queueIndex}");
                if (queueIndex >= Configuration.GearsetNumbers.Count)
                {
                    AutoDutyIPC.SetConfig("StopLevel", "False");
                    Configuration.IsRunning = false;
                    Configuration.Save();
                    return;
                }
                int gearset = Configuration.GearsetNumbers[queueIndex];
                unsafe
                {
                    var gearsetModule = RaptureGearsetModule.Instance();
                    gearsetModule->EquipGearset(gearset);
                }
                swapDelay = 5;
            }
            else
            {
                startDelay = 5;        
            }
        }
        return;
    }

    timeSinceLastCheck += framework.UpdateDelta.TotalSeconds;
    if (timeSinceLastCheck < 15) return;
    timeSinceLastCheck = 0;
    Log.Information($"Timer fired - IsStopped: {AutoDutyIPC.IsStopped()}, queueIndex: {queueIndex}");

    if (!AutoDutyIPC.IsStopped()) return;

    int currentGearset = Configuration.GearsetNumbers[queueIndex];
    Log.Information($"Initial equip - queueIndex: {queueIndex}, gearset: {currentGearset}");
    unsafe
    {
        var gearsetModule = RaptureGearsetModule.Instance();
        gearsetModule->EquipGearset(currentGearset);
    }
    swapDelay = 5;
}

}
