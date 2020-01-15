using Improbable.Gdk.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientFlagManager : MonoBehaviour
{

    public long defaultChannelCost;

    private const string lnChannelCostFlag = "ln_channel_cost";
    public static ClientFlagManager instance;

    private Worker worker;

    public void Awake()
    {
        instance = this;
    }

    public void WorkerCreated(Worker obj)
    {
        worker = obj;
    }
    public long GetChannelCost()
    {
        long channelCost;
        if (long.TryParse(worker.GetWorkerFlag(lnChannelCostFlag), out channelCost))
            return channelCost;
        return defaultChannelCost;
    }
}
