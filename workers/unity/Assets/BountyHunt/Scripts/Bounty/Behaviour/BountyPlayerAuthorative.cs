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

    public async void RequestPayout(long amount)
    {
        var res = await PlayerServiceConnections.instance.DonnerDaemonClient.GetInvoice("payout for " + HunterComponentReader.Data.Name + " Amount: " + amount, amount);

        HunterComponentCommandSender.SendRequestPayoutCommand(entityId, new RequestPayoutRequest(res),OnRequestPayout);
    }

    private void OnRequestPayout(HunterComponent.RequestPayout.ReceivedResponse res)
    {
        if(res.StatusCode == Improbable.Worker.CInterop.StatusCode.Success)
        {
            // TODO something went right
            ClientEvents.instance.onPaymentSucces.Invoke(new PaymentSuccesArgs()
            {
                amount = res.ResponsePayload.Value.Amount,
                invoice = res.RequestPayload.PayReq,
                descripion = res.ResponsePayload.Value.Message
            });
            
        }
        else
        {
            // TODO something went wrong
            ClientEvents.instance.onPaymentFailure.Invoke(new PaymentFailureArgs
            {
                invoice = res.RequestPayload.PayReq,
                message = res.Message
            });
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
