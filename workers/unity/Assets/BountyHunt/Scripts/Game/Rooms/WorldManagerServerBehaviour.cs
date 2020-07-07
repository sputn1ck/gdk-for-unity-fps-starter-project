using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Commands;
using System.Linq;

public class WorldManagerServerBehaviour : MonoBehaviour
{
    [Require] WorldManagerWriter WorldManagerWriter;
    [Require] WorldManagerCommandReceiver WorldManagerCommandReceiver;

    [Require] WorldCommandSender WorldCommandSender;

    [Require] RoomManagerCommandSender RoomManagerCommandSender;


    public bool startNewRoomTrigger;
    private void OnEnable()
    {
        WorldManagerCommandReceiver.OnEndRoomRequestReceived += OnEndRoom;
        WorldManagerCommandReceiver.OnCreateRoomRequestReceived += OnCreateRoom;

        // TODO: check for existing rooms and instantiate them if neccesarry
        // TODO: really check for cantina
        if(WorldManagerWriter.Data.ActiveRooms.Count < 1)
        {
            CreateRoom(new CreateRoomRequest("cantina", "lobby", new TimeInfo(System.DateTime.UtcNow.ToFileTimeUtc(), long.MaxValue),true));

            
        }
    }
    private void Update()
    {
        if (startNewRoomTrigger)
        {
            startNewRoomTrigger = false;
            CreateRoom(new CreateRoomRequest("cantina", "lobby", new TimeInfo(System.DateTime.UtcNow.ToFileTimeUtc(), long.MaxValue), true));
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
        var mapInfo = MapDictStorage.Instance.GetMap(req.MapId);
        
        // TODO: Get Free Slot depending on map size
        var roomCenter = GetNextSlot(mapInfo.Settings);
        // TODO: create room
        var room = new Room("room" + UnityEngine.Random.Range(0, int.MaxValue), new List<long>(), new List<long>(), req.MapId,req.GamemodeId, req.TimeInfo,0, Utility.Vector3ToVector3Float(roomCenter));

        var roomManager = DonnerEntityTemplates.RoomManager(roomCenter, room);

        var list = WorldManagerWriter.Data.ActiveRooms;
        System.Action<WorldCommands.CreateEntity.ReceivedResponse> callback = (WorldCommands.CreateEntity.ReceivedResponse res) =>
        {
            if (res.StatusCode != Improbable.Worker.CInterop.StatusCode.Success)
            {
                Debug.LogErrorFormat("{0} RoomCreation errpr",  res.Message);
                return;
            }
            Debug.LogFormat("{0} created; res {1}", room.RoomId, res);
            room.EntityId = res.EntityId.Value.Id;
            var map = WorldManagerWriter.Data.ActiveRooms;
            map.Add(room.RoomId, room);
            WorldManagerWriter.SendUpdate(new WorldManager.Update()
            {
                ActiveRooms = map
            });
            if (req.InstantStart)
            {
                RoomManagerCommandSender.SendStartRoomCommand(new EntityId(room.EntityId), new StartRoomRequest());
            }
            
        };
        WorldCommandSender.SendCreateEntityCommand(new WorldCommands.CreateEntity.Request(roomManager), callback);
    }

    private Vector3 GetNextSlot(MapSettings mapSettings)
    {
        var activeRooms = WorldManagerWriter.Data.ActiveRooms.ToList();
        activeRooms.Sort((KeyValuePair<string, Room> p1, KeyValuePair<string, Room> p2) =>
        {
            return p1.Value.Origin.X.CompareTo(p2.Value.Origin.X);
        });
        float lastPos = 0f;
        for (int i = 0; i < activeRooms.Count; i++)
        {
            var thisMap = MapDictStorage.Instance.GetMap(activeRooms[i].Value.MapId);
            var thisRoom = activeRooms[i].Value;

            lastPos = thisRoom.Origin.Z + thisMap.Settings.DimensionZ / 2;

            if (i + 1 >= activeRooms.Count)
            break;
            
            var nextMap = MapDictStorage.Instance.GetMap(activeRooms[i + 1].Value.MapId);
            var nextRoom = activeRooms[i + 1].Value;

            var spaceBetweenMaps = (nextRoom.Origin.Z - nextMap.Settings.DimensionZ / 2) - (thisRoom.Origin.Z + thisMap.Settings.DimensionZ / 2);
            if (spaceBetweenMaps >= mapSettings.DimensionZ)
            {
                break;
            }
        }
        return new Vector3(0,0,lastPos + mapSettings.DimensionZ/2);
      
    }

}
