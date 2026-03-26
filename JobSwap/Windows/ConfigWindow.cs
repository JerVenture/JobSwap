using Dalamud.Interface.Windowing;

namespace JobSwap;

public class ConfigWindow : Window
{
    private Plugin Plugin;

    public ConfigWindow(Plugin plugin) : base("Job Swap configuration")
    {
        Plugin = plugin;
    }

    public override void Draw()
    {
        
    }
}