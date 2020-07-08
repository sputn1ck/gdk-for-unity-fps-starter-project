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
    LinkedEntityComponent linkedEntityComponent;
    
    private void OnEnable()
    {
        linkedEntityComponent = GetComponent<LinkedEntityComponent>();
        RoomPlayerCommandReceiver.OnUpdatePlayerRoomRequestReceived += OnUpdatePlayerRoom;
        RequestJoinLobby();
    }

    private void OnUpdatePlayerRoom(RoomPlayer.UpdatePlayerRoom.ReceivedRequest obj)
    {
        var room = WorldManagerClientBehaviour.Instance.WorldManagerReader.Data.ActiveRooms[obj.Payload.RoomId];
        var newInterestTemplate = GetInterestTemplate(room);
        InterestWriter.SendUpdate(new Interest.Update()
        {
            ComponentInterest = newInterestTemplate.AsComponentInterest()
        });

        if (RoomPlayerWriter.Data.RoomId != null && RoomPlayerWriter.Data.RoomId != "")
        {
            RoomManagerCommandSender.SendRemovePlayerCommand(RoomPlayerWriter.Data.RoomEntityid, new RemovePlayerRequest(linkedEntityComponent.EntityId));
        }
        RoomPlayerWriter.SendUpdate(new RoomPlayer.Update()
        {
            RoomEntityid = obj.Payload.RoomEntityid,
            RoomId = obj.Payload.RoomId
        });

    }

    public void RequestJoinLobby()
    {
        WorldManagerCommandSender.SendJoinRoomCommand(new EntityId(3), new JoinRoomRequest
        {
            RoomId = "cantina-1",
            PlayerId = linkedEntityComponent.EntityId
        }, (cb) => {
            if (cb.StatusCode != Improbable.Worker.CInterop.StatusCode.Success)
            {
                Debug.LogError(cb.Message);
            }
        });
    }

    private InterestTemplate GetInterestTemplate(Room roomInfo)
    {
        var map = MapDictStorage.Instance.GetMap(roomInfo.MapInfo.MapId);
        var boxConstraint = Constraint.Box(map.Settings.DimensionX, 200, map.Settings.DimensionZ, Coordinates.FromUnityVector(Utility.Vector3FloatToVector3(roomInfo.Origin)));
        var relativeConstraint = Constraint.RelativeCylinder(150);
        var checkoutQuery = InterestQuery.Query(Constraint.Any(Constraint.All(boxConstraint, relativeConstraint), Constraint.RelativeCylinder(2)));
        var serverQuery = InterestQuery.Query(Constraint.Box(map.Settings.DimensionX, 200, map.Settings.DimensionZ, Coordinates.FromUnityVector(Utility.Vector3FloatToVector3(roomInfo.Origin))));
        var worldManager = InterestQuery.Query(Constraint.Component(Bountyhunt.WorldManager.ComponentId));
        var roomManager = InterestQuery.Query(Constraint.EntityId(roomInfo.EntityId));
        var gameManagerQuery = InterestQuery.Query(Constraint.Component(GameStats.ComponentId));
        var bountyTracerQuery = InterestQuery.Query(Constraint.All(Constraint.Component(TracerComponent.ComponentId), boxConstraint));
        var interestTemplate = InterestTemplate.Create().AddQueries<Position.Component>(serverQuery).AddQueries<ClientMovement.Component>(checkoutQuery, bountyTracerQuery, worldManager,roomManager, gameManagerQuery);
        return interestTemplate;
    }

}
