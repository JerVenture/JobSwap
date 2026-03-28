using Dalamud.Configuration;
using System;
using System.Collections.Generic;

namespace JobSwap;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public List<int> GearsetNumbers { get; set; } = [];
    public int RequestedLevel { get; set; } = 100;
    public bool IsRunning { get; set; } = false;
    public bool EnableARMultiMode = false;

    // The below exists just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
