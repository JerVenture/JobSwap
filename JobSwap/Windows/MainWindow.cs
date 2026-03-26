using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;

namespace JobSwap;
public class MainWindow : Window 
{
    private Plugin Plugin;
    public MainWindow(Plugin plugin) : base("Job Swap")
    {
        Plugin = plugin;
    }

    public override void Draw()
    {
        ImGui.Text("You currently have these gearsets set to level:");
        for (int i = 0; i < Plugin.Configuration.GearsetNumbers.Count; i++)
        {
            ImGui.Text($"{Plugin.Configuration.GearsetNumbers[i]} ");
        }
        if (ImGui.Button("Start Leveling"))
        {
            Plugin.Configuration.IsRunning = true;
            Plugin.Configuration.Save();
        }
        if (ImGui.Button("Stop Leveling"))
        {
            Plugin.Configuration.IsRunning = false;
            Plugin.Configuration.Save();
        }
    }
}