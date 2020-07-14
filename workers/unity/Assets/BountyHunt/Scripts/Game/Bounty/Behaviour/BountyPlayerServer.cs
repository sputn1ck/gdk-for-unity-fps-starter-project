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
using Improbable.Gdk.Core.Commands;
using Lnrpc;

[WorkerType(WorkerUtils.UnityGameLogic)]
public class BountyPlayerServer : MonoBehaviour
{

    [Require] public HunterComponentWriter HunterComponentWriter;
    [Require] public HunterComponentCommandReceiver HunterCommandReceiver;
    [Require] public GameStatsCommandSender GameStatsCommandSender;
    [Require] public PlayerHeartbeatClientCommandSender PlayerHeartbeatClientCommandSender;
    [Require] public ServerMovementWriter ServerMovementWriter;
    [Require] public PositionWriter spatialPosition;
    [Require] public WorldCommandSender wcs;
    [Require] public EntityId entityId;
    public bool kickTrigger;

    private BountyTracerServerBehaviour bountyTracer;

    private LinkedEntityComponent LinkedEntityComponent;

    // Start is called before the first frame update
    void OnEnable()
    {

        LinkedEntityComponent = GetComponent<LinkedEntityComponent>();
        HunterCommandReceiver.OnAddBountyRequestReceived += BountyComponentCommandReceiver_OnAddBountyRequestReceived;
        HunterCommandReceiver.OnRequestPayoutRequestReceived += OnRequestPayout;
        HunterCommandReceiver.OnTeleportPlayerRequestReceived += OnTeleport;
        HunterComponentWriter.OnEarningsUpdate += OnEarningsUpdate;
        HunterCommandReceiver.OnKickPlayerRequestReceived += OnKickPlayer;
        HunterCommandReceiver.OnRefreshAppearanceRequestReceived += OnRefreshAppearance;
        Invoke("SetName", 1f);

        //StartCoroutine(BountyTick());
        ServerGameStats.Instance.AttachPlayer(LinkedEntityComponent.EntityId, this.gameObject);
        SpawnTracer();
    }
    private void OnDisable()
    {
        StopCoroutine(hearbeatCoroutine());
        ServerGameStats.Instance.RemovePlayerGameObject(LinkedEntityComponent.EntityId);
    }
    private void SpawnTracer()
    {
        var bountyTracer = DonnerEntityTemplates.BountyTracer(this.transform.position, this.LinkedEntityComponent.EntityId.Id);
        wcs.SendCreateEntityCommand(new WorldCommands.CreateEntity.Request(bountyTracer));
    }


    private void OnKickPlayer(HunterComponent.KickPlayer.ReceivedRequest obj)
    {
        if (obj.CallerAttributeSet[0] != WorkerUtils.UnityGameLogic)
            return;
        KickPlayer();
    }

    void  Update()
    {
        if(kickTrigger)
        {
            kickTrigger = false;
            KickPlayer();
        }
    }
    private async void OnEarningsUpdate(long obj)
    {
        var amount = HunterComponentWriter.Data.Earnings;
        if (amount < 1)
        {
            HunterComponentWriter.SendUpdate(new HunterComponent.Update { Earnings = 0 });
            return;
        }
        
        SendResponse res = new SendResponse() { PaymentError = "something went wrong" };
        ServerGameChat.instance.SendPrivateMessage(this.entityId.Id, this.entityId.Id, "PAYMENTS_SERVER", "trying to pay out " + res.PaymentError, Chat.MessageType.DEBUG_LOG, false);
        bool payed = false;
        string exceptionMessage = "";
        // Pay to platform
        try
        {
            if (!payed)
            {
                res = await ServerServiceConnections.instance.lnd.KeysendBufferDeposit(ServerServiceConnections.instance.PlatformPubkey, HunterComponentWriter.Data.Pubkey, amount);
                if (res.PaymentError == "")
                    payed = true;

            }
             
        } catch (Exception e)
        {
            exceptionMessage += ";PLATFORM_KEYSEND:" + e.Message;
        }
        if (payed)
        {
            HunterComponentWriter.SendUpdate(new HunterComponent.Update { Earnings = HunterComponentWriter.Data.Earnings - amount });
        }
        if (res.PaymentError != "" && !payed)
        {
            //ServerGameChat.instance.SendPrivateMessage(this.entityId.Id, this.entityId.Id, "PAYMENTS_SERVER", , Chat.MessageType.DEBUG_LOG,false);
            Debug.Log("error while paying out: " + res.PaymentError);
        } else if(exceptionMessage != "" && !payed) {
            //ServerGameChat.instance.SendPrivateMessage(this.entityId.Id, this.entityId.Id, "PAYMENTS_SERVER", "error while paying out: " + exceptionMessage, Chat.MessageType.DEBUG_LOG, false);
            Debug.Log("error while paying out: " + exceptionMessage);
        }
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
        HunterCommandReceiver.SendTeleportPlayerResponse(obj.RequestId, new Bountyhunt.Empty());
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
            HunterCommandReceiver.SendRequestPayoutFailure(obj.RequestId, "cannot decode invoice: " + e.Message);
            return;
        }
        if(invoice == null)
        {
            HunterCommandReceiver.SendRequestPayoutFailure(obj.RequestId, "cannot decode invoice:");
            return;
        }
        if (invoice.NumSatoshis > HunterComponentWriter.Data.Earnings)
        {
            HunterCommandReceiver.SendRequestPayoutFailure(obj.RequestId, "not enough sats");
            return;
        }
        Lnrpc.SendResponse payment;
        try
        {
            payment = await ServerServiceConnections.instance.lnd.PayInvoice(obj.Payload.PayReq);
        } catch (PaymentException pe)
        {

            HunterCommandReceiver.SendRequestPayoutFailure(obj.RequestId, pe.Message);
            return;
        } catch (Exception e)
        {
            return;
        }
        if (payment.PaymentError != "")
        {
            
        }
        var newEarnings = Mathf.Clamp(HunterComponentWriter.Data.Earnings - payment.PaymentRoute.TotalAmtMsat / 1000,0, int.MaxValue);
        Debug.Log("new earnings");
        HunterComponentWriter.SendUpdate(new HunterComponent.Update() { Earnings = (long)newEarnings });
        HunterCommandReceiver.SendRequestPayoutResponse(obj.RequestId, new RequestPayoutResponse(true, payment.PaymentPreimage.ToBase64(), invoice.Description,payment.PaymentRoute.TotalAmtMsat / 1000));
        PrometheusManager.TotalEarnings.Inc(invoice.NumSatoshis);

    }


    //TODO hacky...
    async void SetName()
    {
        var bbhbackend = ServerServiceConnections.instance.BackendGameServerClient;
        var user = await bbhbackend.GetUser(HunterComponentWriter.Data.Pubkey);
        var skin = await bbhbackend.GetUserSkin(HunterComponentWriter.Data.Pubkey);
        Utility.Log("returning Skin: " + skin, Color.magenta);
        HunterComponentWriter.SendUpdate(new HunterComponent.Update { Name = user.Name, EquippedSkin = skin });
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
        IncreaseBounty(obj.Payload.Amount);
    }
    public void IncreaseBounty(long amount)
    {
        HunterComponentWriter.SendUpdate(new HunterComponent.Update { Bounty = HunterComponentWriter.Data.Bounty + amount });
    }

    public void KickPlayer()
    {
        wcs.SendDeleteEntityCommand(new Improbable.Gdk.Core.Commands.WorldCommands.DeleteEntity.Request { EntityId = this.LinkedEntityComponent.EntityId });
    }

    public void OnRefreshAppearance(HunterComponent.RefreshAppearance.ReceivedRequest obj)
    {
        if (obj.EntityId == this.entityId)SetName();
    }
}
