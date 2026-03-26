using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ECommons;

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

    private const string CommandName = "/jobSwap";

    public Configuration Configuration { get; init; }
    
    private AutoDutyIPC AutoDutyIPC;

    private int queueIndex = 0;
    public readonly WindowSystem WindowSystem = new("JobSwap");
    public Plugin()
    {
        ECommonsMain.Init(PluginInterface, this);
        AutoDutyIPC = new AutoDutyIPC();

        Framework.Update += OnUpdate;
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        PluginInterface.UiBuilder.Draw += WindowSystem.Draw;

        Log.Information("JobSwap has started");
    }

    public void Dispose()
    {
        ECommonsMain.Dispose();

        Framework.Update -= OnUpdate;
        PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
        
        WindowSystem.RemoveAllWindows();
    }

    private void OnCommand(string command, string args)
    {

    }

    private void OnUpdate(IFramework framework)
    {
        if (Configuration.IsRunning == true)
        {
            if (AutoDutyIPC.IsStopped() == true)
            {
                if (ObjectTable.LocalPlayer?.Level >= Configuration.RequestedLevel)
                {
                    if (queueIndex < Configuration.GearsetNumbers.Count)
                    {
                        int gearset = Configuration.GearsetNumbers[queueIndex];
                        CommandManager.ProcessCommand($"/gearset change {gearset}");
                        AutoDutyIPC.Start(true);
                        queueIndex++;
                    }
                    else
                    {
                        Configuration.IsRunning = false;
                        Configuration.Save();
                    }
                }
                else return;
            }
            else return;
        }
        else return;
    }

}
