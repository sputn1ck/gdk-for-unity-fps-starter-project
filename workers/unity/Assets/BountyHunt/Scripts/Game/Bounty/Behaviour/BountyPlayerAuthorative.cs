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
        var totalBalance = lastEarnings + balance.DaemonBalance;
        if (balance.ChannelMissingBalance > 0)
        {
            totalBalance -= balance.ChannelMissingBalance;
        } else
        {
            totalBalance += balance.BufferBalance;
        }
        ClientEvents.instance.onBalanceUpdate.Invoke(new BalanceUpdateEventArgs()
        {
            NewAmount = totalBalance,
            OldAmount = lastBalance,
        });
        lastBalance = totalBalance;
    }




    private void OnApplicationQuit()
    {
        ct.Cancel();
    }
}
