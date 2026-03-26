using Dalamud.Interface.Windowing;

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
        
    }
}