using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Commands;


public class RoomManagerServerBehaviour : MonoBehaviour
{
    [Require] RoomManagerWriter RoomManagerWriter;
    [Require] RoomManagerCommandReceiver RoomManagerCommandReceiver;
    [Require] WorldManagerCommandSender WorldManagerCommandSender;
    [Require] RoomPlayerCommandSender RoomPlayerCommandSender;
    [Require] EntityId EntityId;
    LinkedEntityComponent LinkedEntityComponent;

    private GameObject mapGo;
    private void OnEnable()
    {

        LinkedEntityComponent = GetComponent<LinkedEntityComponent>();
        Initialize();

        RoomManagerCommandReceiver.OnAddPlayerRequestReceived += AddPlayer;
        RoomManagerCommandReceiver.OnRemovePlayerRequestReceived += RemovePlayer;
        RoomManagerCommandReceiver.OnReadyToJoinRequestReceived += ReadyToJoin;

        var map = RoomManagerWriter.Data.PlayerMap;
        var room = RoomManagerWriter.Data.RoomInfo;
        SendUpdates(map, room);
    }

    private void ReadyToJoin(RoomManager.ReadyToJoin.ReceivedRequest obj)
    {
        //TODO get spawnpoint
        
    }

    private void RemovePlayer(RoomManager.RemovePlayer.ReceivedRequest obj)
    {
        var map = RoomManagerWriter.Data.PlayerMap;
        if (map.ContainsKey(obj.Payload.PlayerId))
        {
            map.Remove(obj.Payload.PlayerId);
        }
        var room = RoomManagerWriter.Data.RoomInfo;
        room.ActivePlayers.Remove(obj.Payload.PlayerId);
        
        SendUpdates(map, room);
    }

    private void AddPlayer(RoomManager.AddPlayer.ReceivedRequest obj)
    {
        var map = RoomManagerWriter.Data.PlayerMap;
        map.Add(obj.Payload.PlayerId, new PlayerItem("", "", 0, 0, 0, 0));
        var room = RoomManagerWriter.Data.RoomInfo;
        room.ActivePlayers.Add(obj.Payload.PlayerId);
        SendUpdates(map,room);
        RoomPlayerCommandSender.SendUpdatePlayerRoomCommand(obj.Payload.PlayerId, new UpdatePlayerRoomRequest(room.RoomId, LinkedEntityComponent.EntityId));
    }

    private void SendUpdates(Dictionary<EntityId, PlayerItem> map, Room room)
    {
        room.EntityId = EntityId;
        RoomManagerWriter.SendUpdate(new RoomManager.Update()
        {
            RoomInfo = room,
            PlayerMap = map
        }) ;
        WorldManagerCommandSender.SendUpdateRoomCommand(new EntityId(3), new UpdateRoomRequest()
        {
            Room = room
        });
    }
    private void Initialize()
    {
        var mapInfo = MapDictStorage.Instance.GetMap(RoomManagerWriter.Data.RoomInfo.MapInfo.MapId);

        mapGo = mapInfo.Initialize(this, true, this.transform.position, RoomManagerWriter.Data.RoomInfo.MapInfo.MapData);

    }
}
