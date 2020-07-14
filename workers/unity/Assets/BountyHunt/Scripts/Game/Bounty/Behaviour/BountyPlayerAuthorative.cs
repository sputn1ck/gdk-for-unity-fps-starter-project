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
    private long earningsThisSession;
    private long lastEarningsThisSession;

    private int kills;
    private int deaths;

    private CancellationTokenSource ct;
    private void Awake()
    {
        instance = this;
    }
    void OnEnable()
    {
        ClientGameObjectManager.Instance.SetAuthorativePlayer(this.gameObject, this.entityId);

        HunterComponentReader.OnBountyUpdate += HunterComponentReader_OnBountyUpdate;
        HunterComponentReader.OnEarningsUpdate += HunterComponentReader_OnEarningsUpdate;
        HunterComponentReader.OnSessionEarningsUpdate += HunterComponentReader_OnSessionEarningsUpdate;
        HunterComponentReader.OnKillsUpdate += HunterComponentReader_OnKillsUpdate;
        HunterComponentReader.OnDeathsUpdate += HunterComponentReader_OnDeathsUpdate;


        lastBounty = 0;
        lastEarnings = 0;
        earningsThisSession = 0;

        ct = new CancellationTokenSource();
        UpdateTotalBalance();
        
    }

    void OnDisable()
    {
        ClientGameObjectManager.Instance.DisableAuthorativePlayer();
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

    private void HunterComponentReader_OnKillsUpdate(int kills)
    {
        this.kills = kills;
        ClientEvents.instance.onPlayerKillsAndDeathsUpdate.Invoke(new KillsAndDeathsUpdateEventArgs { kills = this.kills, deaths = this.deaths });
    }

    private void HunterComponentReader_OnDeathsUpdate(int deaths)
    {
        this.deaths = deaths;
        ClientEvents.instance.onPlayerKillsAndDeathsUpdate.Invoke(new KillsAndDeathsUpdateEventArgs { kills = this.kills, deaths = this.deaths });
    }

    private async void UpdateTotalBalance()
    {
        var balance = await PlayerServiceConnections.instance.DonnerDaemonClient.GetWalletBalance();
        
        ClientEvents.instance.onBalanceUpdate.Invoke(new BalanceUpdateEventArgs()
        {
            GameServerBalance = HunterComponentReader.Data.Earnings,
            BufferBalance = balance.BufferBalance,
            DaemonBalance = balance.DaemonBalance,
            ChannelCost = balance.ChannelMissingBalance
        });
    }

    public void RefreshAppearance()
    {
        HunterComponent.RefreshAppearance.Request req = new HunterComponent.RefreshAppearance.Request(entityId, new Bountyhunt.Empty());
        HunterComponentCommandSender.SendRefreshAppearanceCommand(req);
    }


    private void OnApplicationQuit()
    {
        ct.Cancel();
    }
}
