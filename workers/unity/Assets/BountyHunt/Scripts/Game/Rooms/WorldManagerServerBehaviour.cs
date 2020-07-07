using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Commands;

public class WorldManagerServerBehaviour : MonoBehaviour
{
    [Require] WorldManagerWriter WorldManagerWriter;
    [Require] WorldManagerCommandReceiver WorldManagerCommandReceiver;

    [Require] WorldCommandSender WorldCommandSender;

    [Require] RoomManagerCommandSender RoomManagerCommandSender;
    private void OnEnable()
    {
        WorldManagerCommandReceiver.OnEndRoomRequestReceived += OnEndRoom;
        WorldManagerCommandReceiver.OnCreateRoomRequestReceived += OnCreateRoom;

        // TODO: check for existing rooms and instantiate them if neccesarry
        // TODO: really check for cantina
        if(WorldManagerWriter.Data.ActiveRooms.Count < 1)
        {
            CreateRoom(new CreateRoomRequest("cantina-1", "lobby", new TimeInfo(System.DateTime.UtcNow.ToFileTimeUtc(), long.MaxValue),true));

            
        }
    }

    private void OnCreateRoom(WorldManager.CreateRoom.ReceivedRequest obj)
    {
        
    }

    private void OnEndRoom(WorldManager.EndRoom.ReceivedRequest obj)
    {
        
    }

    private void CreateRoom(CreateRoomRequest req)
    {
        // TODO: Get MapInfo
        // TODO: Get Free Slot depending on map size
        var roomCenter = new Vector3(0, 0, 0);
        // TODO: set room gamemode
        // TODO: create room
        var room = new Room("room" + UnityEngine.Random.Range(0, int.MaxValue), new List<long>(), new List<long>(), req.MapId,req.GamemodeId, req.TimeInfo,0);

        var roomManager = DonnerEntityTemplates.RoomManager(roomCenter, room);

        var list = WorldManagerWriter.Data.ActiveRooms;
        System.Action<WorldCommands.CreateEntity.ReceivedResponse> callback = delegate (WorldCommands.CreateEntity.ReceivedResponse res)
        {
            Debug.LogFormat("{0} created", room.RoomId);
            room.EntityId = res.EntityId.Value.Id;
            var map = WorldManagerWriter.Data.ActiveRooms;
            map.Add(room.RoomId, room);
            if (req.InstantStart)
            {
                RoomManagerCommandSender.SendStartRoomCommand(new EntityId(room.EntityId), new StartRoomRequest());
            }
            
        };
        WorldCommandSender.SendCreateEntityCommand(new WorldCommands.CreateEntity.Request(roomManager), callback);
    }

}
