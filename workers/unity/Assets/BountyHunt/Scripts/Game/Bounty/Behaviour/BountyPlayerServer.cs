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
using Fps;
using Fps.SchemaExtensions;
using Improbable;

[WorkerType(WorkerUtils.UnityGameLogic)]
public class BountyPlayerServer : MonoBehaviour
{

    [Require] public HunterComponentWriter HunterComponentWriter;
    [Require] public HunterComponentCommandReceiver BountyComponentCommandReceiver;
    [Require] public GameStatsCommandSender GameStatsCommandSender;
    [Require] public PlayerHeartbeatClientCommandSender PlayerHeartbeatClientCommandSender;
    [Require] public ServerMovementWriter ServerMovementWriter;
    [Require] public PositionWriter spatialPosition;


    private LinkedEntityComponent LinkedEntityComponent;

    // Start is called before the first frame update
    void OnEnable()
    {

        LinkedEntityComponent = GetComponent<LinkedEntityComponent>();
        BountyComponentCommandReceiver.OnAddBountyRequestReceived += BountyComponentCommandReceiver_OnAddBountyRequestReceived;
        BountyComponentCommandReceiver.OnRequestPayoutRequestReceived += OnRequestPayout;
        BountyComponentCommandReceiver.OnTeleportPlayerRequestReceived += OnTeleport;

        
        Invoke("SetName", 1f);
        //StartCoroutine(BountyTick());
    }

    private void OnTeleport(HunterComponent.TeleportPlayer.ReceivedRequest obj)
    {
        var pos = new Vector3(obj.Payload.X, obj.Payload.Y, obj.Payload.Z);


        //var (pos, spawnYaw, spawnPitch) = SpawnPoints.GetRandomSpawnPoint();


        // Move to a spawn point (position and rotation)
        var newLatest = new ServerResponse
        {
            Position = pos.ToVector3Int(),
            IncludesJump = false,
            Timestamp = ServerMovementWriter.Data.Latest.Timestamp,
            TimeDelta = 0
        };

        var serverMovementUpdate = new ServerMovement.Update
        {
            Latest = newLatest
        };
        ServerMovementWriter.SendUpdate(serverMovementUpdate);

        transform.position = pos + LinkedEntityComponent.Worker.Origin;

        var spatialPositionUpdate = new Position.Update
        {
            Coords = Coordinates.FromUnityVector(pos)
        };
        spatialPosition.SendUpdate(spatialPositionUpdate);
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
        Debug.Log("new earnings");
        HunterComponentWriter.SendUpdate(new HunterComponent.Update() { Earnings = (long)newEarnings });
        BountyComponentCommandReceiver.SendRequestPayoutResponse(obj.RequestId, new RequestPayoutResponse(true, payment.PaymentPreimage.ToBase64(), invoice.Description,payment.PaymentRoute.TotalAmtMsat / 1000));
        PrometheusManager.TotalEarnings.Inc(invoice.NumSatoshis);

    }


    //TODO hacky...
    async void SetName()
    {
        //var bbhbackend = ServerServiceConnections.instance.BackendGameServerClient;
        //var user = await bbhbackend.GetUser(HunterComponentWriter.Data.Pubkey);
        //HunterComponentWriter.SendUpdate(new HunterComponent.Update { Name = user.Name });
        GameStatsCommandSender.SendSetNameCommand(new EntityId(2), new SetNameRequest(HunterComponentWriter.Data.Name, LinkedEntityComponent.EntityId, HunterComponentWriter.Data.Pubkey));
        StartCoroutine(hearbeatCoroutine());
    }

    IEnumerator hearbeatCoroutine()
    {
        while (!ServerServiceConnections.ct.IsCancellationRequested)
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


}
