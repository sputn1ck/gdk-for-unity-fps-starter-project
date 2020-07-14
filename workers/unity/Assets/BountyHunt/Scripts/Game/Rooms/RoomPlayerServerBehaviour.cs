using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Commands;
using System.Linq;
using Improbable;
using Improbable.Gdk.QueryBasedInterest;
using Fps;

public class RoomPlayerServerBehaviour : MonoBehaviour
{
    [Require] WorldManagerCommandSender WorldManagerCommandSender;
    [Require] RoomPlayerCommandReceiver RoomPlayerCommandReceiver;
    [Require] RoomManagerCommandSender RoomManagerCommandSender;
    [Require] RoomPlayerWriter RoomPlayerWriter;
    [Require] InterestWriter InterestWriter;
    [Require] HunterComponentCommandSender HunterComponentCommandSender;
    [Require] EntityId EntityId;
    LinkedEntityComponent linkedEntityComponent;

    private string pubkey;
    
    private void OnEnable()
    {
        linkedEntityComponent = GetComponent<LinkedEntityComponent>();
        RoomPlayerCommandReceiver.OnUpdatePlayerRoomRequestReceived += OnUpdatePlayerRoom;
        pubkey = RoomPlayerWriter.Data.Pubkey;
        JoinWorld();



        
    }

    private void JoinWorld()
    {
        var sig = PlayerServiceConnections.instance.lnd.SignMessage(Utility.AuthMessage);
        WorldManagerCommandSender.SendAddActivePlayerCommand(new EntityId(3), new AddActivePlayerRequest(sig.Signature, pubkey, EntityId),(cb) => {
            if (cb.StatusCode != Improbable.Worker.CInterop.StatusCode.Success)
            {
                Debug.LogError(cb.Message);
                return;
            }
            RequestJoinCantina();
        });
    }

    private void OnUpdatePlayerRoom(RoomPlayer.UpdatePlayerRoom.ReceivedRequest obj)
    {
        var room = obj.Payload.Room;
        var newInterestTemplate = GetInterestTemplate(room);
        InterestWriter.SendUpdate(new Interest.Update()
        {
            ComponentInterest = newInterestTemplate.AsComponentInterest()
        });

        if (RoomPlayerWriter.Data.RoomId != null && RoomPlayerWriter.Data.RoomId != "")
        {
            RoomManagerCommandSender.SendRemovePlayerCommand(RoomPlayerWriter.Data.RoomEntityid, new RemovePlayerRequest(pubkey));
        }
        RoomPlayerWriter.SendUpdate(new RoomPlayer.Update()
        {
            RoomEntityid = room.EntityId,
            RoomId = room.RoomId
        });
    }

    public void RequestJoinCantina()
    {
        WorldManagerCommandSender.SendGetCantinaCommand(new EntityId(3), new GetCantinaRequest()
        {
            PlayerId = EntityId
        }, (cb) => {
            if (cb.StatusCode != Improbable.Worker.CInterop.StatusCode.Success)
            {
                Debug.LogError(cb.Message);
                return;
            }
            if (cb.ResponsePayload.Value.NewlyCreated)
            {
                StartCoroutine(WaitForJoin(cb.ResponsePayload.Value.Room.RoomId));
            }
            else
            {
                RequestJoinRoom(cb.ResponsePayload.Value.Room.RoomId);
            }

        });
    }
    public IEnumerator WaitForJoin(string roomid)
    {
        yield return new WaitForSeconds(2f);
        RequestJoinRoom(roomid);

    }
    public void RequestJoinRoom(string roomid)
    {
        WorldManagerCommandSender.SendJoinRoomCommand(new EntityId(3), new JoinRoomRequest
        {
            RoomId = roomid,
            PlayerId = linkedEntityComponent.EntityId,
        }, (cb) => {
            if (cb.StatusCode != Improbable.Worker.CInterop.StatusCode.Success)
            {
                Debug.LogError(cb.Message);
                return;
            }

        });
    }

    private InterestTemplate GetInterestTemplate(Room roomInfo)
    {
        var map = MapDictStorage.Instance.GetMap(roomInfo.MapInfo.MapId);
        var roomConstraint = Constraint.Box(map.Settings.DimensionX, 200, map.Settings.DimensionZ, Coordinates.FromUnityVector(Utility.Vector3FloatToVector3(roomInfo.Origin)));
        var relativeConstraint = Constraint.RelativeCylinder(100);
        var checkoutQuery = InterestQuery.Query(Constraint.Any(Constraint.All(roomConstraint, relativeConstraint), Constraint.RelativeCylinder(1)));
        var serverQuery = InterestQuery.Query(Constraint.Any(Constraint.RelativeCylinder(1),Constraint.Box(map.Settings.DimensionX, 200, map.Settings.DimensionZ, Coordinates.FromUnityVector(Utility.Vector3FloatToVector3(roomInfo.Origin)))));
        var worldManager = InterestQuery.Query(Constraint.Component(Bountyhunt.WorldManager.ComponentId));
        var roomManager = InterestQuery.Query(Constraint.All(Constraint.Component(Bountyhunt.RoomManager.ComponentId), roomConstraint));
        var gameManagerQuery = InterestQuery.Query(Constraint.Component(GameStats.ComponentId));
        var bountyTracerQuery = InterestQuery.Query(Constraint.All(Constraint.Component(TracerComponent.ComponentId), roomConstraint));
        var interestTemplate = InterestTemplate.Create().AddQueries<Position.Component>(serverQuery).AddQueries<ClientMovement.Component>(checkoutQuery, bountyTracerQuery, worldManager,roomManager, gameManagerQuery);
        return interestTemplate;
    }

}