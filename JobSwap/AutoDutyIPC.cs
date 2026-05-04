using ECommons.EzIpcManager;
using System;

namespace JobSwap;

public class AutoDutyIPC
{
    public AutoDutyIPC()
    {
        EzIPC.Init(this, "AutoDuty");
    }

    [EzIPC] public Action<bool> Start = null!;
    [EzIPC] public Action Stop = null!;
    [EzIPC] public Func<bool> IsStopped = null!;
    [EzIPC] public Action<string, object> SetConfig = null!;
    [EzIPC] public Action<int> SetLevelingMode = null!;
}