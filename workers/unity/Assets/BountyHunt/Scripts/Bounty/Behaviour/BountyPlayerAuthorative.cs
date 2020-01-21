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

    [Require] HunterComponentReader HunterComponentReader;

    private long lastBounty;
    private long lastEarnings;
    // Start is called before the first frame update
    void OnEnable()
    {
        HunterComponentReader.OnBountyUpdate += HunterComponentReader_OnBountyUpdate;
        HunterComponentReader.OnEarningsUpdate += HunterComponentReader_OnEarningsUpdate;
        lastBounty = 0;
        lastEarnings = 0;
    }

    private void HunterComponentReader_OnEarningsUpdate(long earnings)
    {
        ClientEvents.instance.onEarningsUpdate.Invoke(new EarningsUpdateEventArgs()
        {
            NewAmount = earnings,
            OldAmount = lastEarnings
        });
        lastEarnings = earnings;
    }

    private void HunterComponentReader_OnBountyUpdate(long bounty)
    {
            ClientEvents.instance.onBountyUpdate.Invoke(new BountyUpdateEventArgs() {
                NewAmount = bounty,
                OldAmount = lastBounty,
                Reason = BountyReason.PICKUP });
            lastBounty = bounty;
        
        
    }

}
