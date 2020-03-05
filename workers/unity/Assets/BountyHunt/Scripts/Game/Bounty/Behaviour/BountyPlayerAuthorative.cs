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

    public static BountyPlayerAuthorative instance;

    private long lastBounty;
    private long lastEarnings;
    private long lastBalance;
    private long earningsThisSession;
    private long lastEarningsThisSession;

    private CancellationTokenSource ct;
    private void Awake()
    {
        instance = this;
    }
    void OnEnable()
    {
        HunterComponentReader.OnBountyUpdate += HunterComponentReader_OnBountyUpdate;
        HunterComponentReader.OnEarningsUpdate += HunterComponentReader_OnEarningsUpdate;
        HunterComponentReader.OnSessionEarningsUpdate += HunterComponentReader_OnSessionEarningsUpdate;

        lastBounty = 0;
        lastEarnings = 0;
        lastBalance = 0;
        earningsThisSession = 0;

        ct = new CancellationTokenSource();
        UpdateTotalBalance();
        StartCoroutine(requestPayoutEnumerator());
    }

    

    public async void RequestPayout(long amount)
    {
        var res = await PlayerServiceConnections.instance.DonnerDaemonClient.GetInvoice("payout for " + HunterComponentReader.Data.Name + " Amount: " + amount, amount);

        HunterComponentCommandSender.SendRequestPayoutCommand(entityId, new RequestPayoutRequest(res),OnRequestPayout);
    }

    private void OnRequestPayout(HunterComponent.RequestPayout.ReceivedResponse res)
    {
        Debug.LogFormat("Payout Callback: status: {0} message {1} ", res.StatusCode, res.Message);
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
        
        lastEarnings = earnings;
        UpdateTotalBalance();
    }

    private void HunterComponentReader_OnBountyUpdate(long bounty)
    {
            ClientEvents.instance.onBountyUpdate.Invoke(new BountyUpdateEventArgs() {
                NewAmount = bounty,
                OldAmount = lastBounty,
                Reason = BountyReason.PICKUP });
            lastBounty = bounty;
    }
    private void HunterComponentReader_OnSessionEarningsUpdate(long obj)
    {
        earningsThisSession = obj;
        ClientEvents.instance.onSessionEarningsUpdate.Invoke(new SessionEarningsEventArgs()
        {
            NewAmount = earningsThisSession,
            OldAmount = lastEarningsThisSession
        });
        lastEarningsThisSession = earningsThisSession;
    }

    private async void UpdateTotalBalance()
    {
        var balance = await PlayerServiceConnections.instance.DonnerDaemonClient.GetWalletBalance();
        var totalBalance = lastEarnings + balance.LocalBalance;
        if (balance.MissingBalance > 0)
        {
            totalBalance -= balance.MissingBalance;
        } else
        {
            totalBalance += balance.StashBalance;
        }
        ClientEvents.instance.onBalanceUpdate.Invoke(new BalanceUpdateEventArgs()
        {
            NewAmount = totalBalance,
            OldAmount = lastBalance,
        });
        lastBalance = totalBalance;
    }


    private IEnumerator requestPayoutEnumerator()
    {
        while (!ct.IsCancellationRequested)
        {
            yield return new WaitForSeconds(3f);
            if(HunterComponentReader.Data.Earnings > 10)
            {
                RequestPayout(HunterComponentReader.Data.Earnings);
            }
        }
    }


    private void OnApplicationQuit()
    {
        ct.Cancel();
    }
}
