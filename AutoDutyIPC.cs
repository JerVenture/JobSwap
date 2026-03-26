using ECommons.EzIpcManager;

namespace JobSwap;

public class AutoDutyIPC
{
    public AutoDutyIPC()
    {
        EzIPC.Init(this, "AutoDuty");
    }

    [EzIPC]
    public Action<bool> Start;
    [EzIPC]
    public Func<bool> IsStopped;
}