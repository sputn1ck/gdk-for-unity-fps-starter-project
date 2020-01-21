using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using System.Threading;
using Fps.Config;

[WorkerType(WorkerUtils.UnityClient)]
public class BountyPlayerAuthorative : MonoBehaviour
{

    [Require] BountyComponentReader BountyComponentReader;
    [Require] HunterComponentReader HunterComponentReader;

    private long lastBounty;
    private long lastEarnings;
    // Start is called before the first frame update
    void OnEnable()
    {
        BountyComponentReader.OnUpdate += BountyComponentReader_OnUpdate;
        HunterComponentReader.OnEarningsUpdate += HunterComponentReader_OnEarningsUpdate;
        lastBounty = 0;
        lastEarnings = 0;
    }

    private void HunterComponentReader_OnEarningsUpdate(long obj)
    {
        ClientEvents.instance.onEarningsUpdate.Invoke(new EarningsUpdateEventArgs()
        {
            NewAmount = obj,
            OldAmount = lastEarnings
        });
        lastEarnings = obj;
    }

    private void BountyComponentReader_OnUpdate(BountyComponent.Update obj)
    {
        if (obj.Bounty.HasValue)
        {
            ClientEvents.instance.onBountyUpdate.Invoke(new BountyUpdateEventArgs() {
                NewAmount = obj.Bounty.Value,
                OldAmount = lastBounty,
                Reason = BountyReason.PICKUP });
            lastBounty = obj.Bounty.Value;
        }
        
    }

}
