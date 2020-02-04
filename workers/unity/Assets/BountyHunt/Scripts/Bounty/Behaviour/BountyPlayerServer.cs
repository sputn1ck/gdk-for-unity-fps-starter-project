using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using System.Threading;
using Fps.Config;
using Improbable.Gdk.Core;
using Improbable.Gdk.PlayerLifecycle;
using System;

[WorkerType(WorkerUtils.UnityGameLogic)]
public class BountyPlayerServer : MonoBehaviour
{

    [Require] public HunterComponentWriter HunterComponentWriter;
    [Require] public HunterComponentCommandReceiver BountyComponentCommandReceiver;
    [Require] public GameStatsCommandSender GameStatsCommandSender;
    [Require] public PlayerHeartbeatClientCommandSender PlayerHeartbeatClientCommandSender;
    


    private LinkedEntityComponent LinkedEntityComponent;

    private CancellationTokenSource ct;
    // Start is called before the first frame update
    void OnEnable()
    {

        LinkedEntityComponent = GetComponent<LinkedEntityComponent>();
        BountyComponentCommandReceiver.OnAddBountyRequestReceived += BountyComponentCommandReceiver_OnAddBountyRequestReceived;
        BountyComponentCommandReceiver.OnRequestPayoutRequestReceived += OnRequestPayout;
        ct = new CancellationTokenSource();

        
        Invoke("SetName", 1f);
        //StartCoroutine(BountyTick());
    }

    private async void OnRequestPayout(HunterComponent.RequestPayout.ReceivedRequest obj)
    {
        Lnrpc.PayReq invoice;
        try
        {
            invoice = await ServerServiceConnections.instance.lnd.DecodePayreq(obj.Payload.PayReq);

        }
        catch (Exception e)
        {
            BountyComponentCommandReceiver.SendRequestPayoutFailure(obj.RequestId, "cannot decode invoice: " + e.Message);
            return;
        }
        if(invoice == null)
        {
            BountyComponentCommandReceiver.SendRequestPayoutFailure(obj.RequestId, "cannot decode invoice:");
            return;
        }
        if (invoice.NumSatoshis > HunterComponentWriter.Data.Earnings)
        {
            BountyComponentCommandReceiver.SendRequestPayoutFailure(obj.RequestId, "not enough sats");
            return;
        }
        var payment = await ServerServiceConnections.instance.lnd.PayInvoice(obj.Payload.PayReq);
        if (payment.PaymentError != "")
        {
            BountyComponentCommandReceiver.SendRequestPayoutFailure(obj.RequestId, payment.PaymentError);
            return;
        }
        var newEarnings = Mathf.Clamp(HunterComponentWriter.Data.Earnings - payment.PaymentRoute.TotalAmtMsat / 1000,0, int.MaxValue);
        HunterComponentWriter.SendUpdate(new HunterComponent.Update() { Earnings = (long)newEarnings });
        BountyComponentCommandReceiver.SendRequestPayoutResponse(obj.RequestId, new RequestPayoutResponse(true, payment.PaymentPreimage.ToBase64(), ""));

    }


    //TODO hacky...
    async void SetName()
    {
        var bbhbackend = PlayerServiceConnections.instance.BackendPlayerClient;
        var name = await bbhbackend.GetUsername(HunterComponentWriter.Data.Pubkey);
        HunterComponentWriter.SendUpdate(new HunterComponent.Update { Name = name });
        GameStatsCommandSender.SendSetNameCommand(new EntityId(2), new SetNameRequest(name, LinkedEntityComponent.EntityId, HunterComponentWriter.Data.Pubkey));
        StartCoroutine(hearbeatCoroutine());
    }

    IEnumerator hearbeatCoroutine()
    {
        while (!ct.IsCancellationRequested)
        {
            PlayerHeartbeatClientCommandSender.SendPlayerHeartbeatCommand(LinkedEntityComponent.EntityId, new Improbable.Gdk.Core.Empty(), OnHearbeat);
            yield return new WaitForSeconds(3f);
        }
    }

    private void OnHearbeat(PlayerHeartbeatClient.PlayerHeartbeat.ReceivedResponse res)
    {
        if(res.StatusCode == Improbable.Worker.CInterop.StatusCode.Success)
        {
            var data = HunterComponentWriter.Data;
            ServerServiceConnections.instance.BackendGameServerClient.AddPlayerHeartbeat(data.Pubkey, data.Bounty, data.Kills, data.Deaths);
        }
    }
    private void BountyComponentCommandReceiver_OnAddBountyRequestReceived(HunterComponent.AddBounty.ReceivedRequest obj)
    {
        if (obj.CallerAttributeSet[0] != WorkerUtils.UnityGameLogic)
            return;
        HunterComponentWriter.SendUpdate(new HunterComponent.Update { Bounty = HunterComponentWriter.Data.Bounty + obj.Payload.Amount });
    }

    private void OnDisable()
    {
        StopCoroutine(hearbeatCoroutine());
    }

    private void OnApplicationQuit()
    {
        ct.Cancel();
    }

}
