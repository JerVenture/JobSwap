using System.Collections.Generic;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using System.Linq;

namespace JobSwap;

public class ConfigWindow : Window
{
    private Plugin Plugin;

    private int levelInput = 100;
    private int dropdown = 0;

    public ConfigWindow(Plugin plugin) : base("Job Swap configuration")
    {
        Plugin = plugin;


    }

    public override void Draw()
    {
        List<string> ClassList = []; 

        unsafe
        {
            var gearsetModule = RaptureGearsetModule.Instance();

            for (int i = 0; gearsetModule->GetGearset(i) != null; i++)
            {
                var entry = gearsetModule->GetGearset(i);
                ClassList.Add($"{i + 1} - {entry->Name.ToString()}");
            }
        }

        string[] classArray = ClassList.ToArray();

        ImGui.Text("Target level:"); 
        ImGui.SameLine();
        ImGui.InputInt("##level", ref levelInput);
        ImGui.SameLine();
        if (ImGui.Button("Set")) 
        {
            Plugin.Configuration.RequestedLevel = levelInput;
            Plugin.Configuration.Save();
        }

        ImGui.Combo("##gearset", ref dropdown, classArray, classArray.Length);
        ImGui.SameLine();
        if (ImGui.Button("Add class to leveling list"))
        {
            Plugin.Configuration.GearsetNumbers.Add(dropdown);
            Plugin.Configuration.Save();
        }

        if (ImGui.Button("Clear the leveling list"))
        {
            Plugin.Configuration.GearsetNumbers = [];
            Plugin.Configuration.Save();
        }

    }
}