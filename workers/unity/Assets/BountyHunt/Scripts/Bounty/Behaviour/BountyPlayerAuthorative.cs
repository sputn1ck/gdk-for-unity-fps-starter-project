using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using System.Threading;
using Fps.Config;
using Improbable.Gdk.Core;

[WorkerType(WorkerUtils.UnityClient)]
public class BountyPlayerAuthorative : MonoBehaviour
{

    [Require] public HunterComponentReader HunterComponentReader;
    [Require] public HunterComponentCommandSender HunterComponentCommandSender;
    [Require] EntityId entityId;

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

    public void RequestPayout(long amount)
    {
        // TODO get invoice

        HunterComponentCommandSender.SendRequestPayoutCommand(entityId, new RequestPayoutRequest(""),OnRequestPayout);
    }

    private void OnRequestPayout(HunterComponent.RequestPayout.ReceivedResponse res)
    {
        if(res.StatusCode == Improbable.Worker.CInterop.StatusCode.Success)
        {
            // TODO something went right
        }
        else
        {
            // TODO something went wrong
        }
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
