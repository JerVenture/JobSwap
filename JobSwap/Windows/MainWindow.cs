using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using System.Collections.Generic;
using System;

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

        ImGui.Text($"Classes set to level to: {Plugin.Configuration.RequestedLevel}");
        ImGui.Text("You currently have these gearsets set to level:");

        unsafe
        {
            var gearsetModule = RaptureGearsetModule.Instance();
            if (gearsetModule != null)
            {
                for (int i = 0; i < Plugin.Configuration.GearsetNumbers.Count; i++)
                {
                    var entry = gearsetModule->GetGearset(Plugin.Configuration.GearsetNumbers[i]);
                    if (entry == null) continue;
                    var name = System.Text.Encoding.UTF8.GetString(entry->Name).TrimEnd('\0');
                    ImGui.Text($"{i +1 }. {name}");
                }
            }
        }

        if (ImGui.Button("Start Leveling"))
        {
            Plugin.SetAutodutyStopLevel(Plugin.Configuration.RequestedLevel);
            Plugin.StartQueue();
        }
        
        if (ImGui.Button("Stop Leveling"))
        {
            Plugin.Configuration.IsRunning = false;
            Plugin.StopAutoDuty();
            Plugin.Configuration.Save();
        }

        if (ImGui.Button("Open Config"))
        {
            Plugin.ToggleConfigUi();
        }
    }
}