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



    public bool startGenerated;
    private void OnEnable()
    {
        WorldManagerCommandReceiver.OnEndRoomRequestReceived += OnEndRoom;
        WorldManagerCommandReceiver.OnCreateRoomRequestReceived += OnCreateRoom;
        WorldManagerCommandReceiver.OnUpdateRoomRequestReceived += OnUpdateRoom;
        WorldManagerCommandReceiver.OnStartRoomRequestReceived += OnStartRoom;
        WorldManagerCommandReceiver.OnJoinRoomRequestReceived += OnJoinRoom;
        // TODO: check for existing rooms and instantiate them if neccesarry
        // TODO: really check for cantina
        if(WorldManagerWriter.Data.ActiveRooms.Count < 1)
        {
            CreateRoom(new CreateRoomRequest(new MapInfo("cantina",""), "lobby", new TimeInfo(System.DateTime.UtcNow.ToFileTimeUtc(), long.MaxValue)),"cantina-1"); 
        }
    }


    private void OnJoinRoom(WorldManager.JoinRoom.ReceivedRequest obj)
    {
        throw new System.NotImplementedException();
    }

    private void OnStartRoom(WorldManager.StartRoom.ReceivedRequest obj)
    {
  
    }

    private void OnUpdateRoom(WorldManager.UpdateRoom.ReceivedRequest obj)
    {
        throw new System.NotImplementedException();
    }

    private void Update()
    {
        
        if (startGenerated)
        {
            startGenerated = false;
            CreateRoom(new CreateRoomRequest(new MapInfo("generated_20",UnityEngine.Random.Range(float.MinValue, float.MaxValue).ToString()), "lobby", new TimeInfo(System.DateTime.UtcNow.ToFileTimeUtc(), long.MaxValue)));
        }
    }
    private void OnCreateRoom(WorldManager.CreateRoom.ReceivedRequest obj)
    {
        
    }

    private void OnEndRoom(WorldManager.EndRoom.ReceivedRequest obj)
    {
        
    }

    private void CreateRoom(CreateRoomRequest req, string id = "")
    {
        // TODO: Get MapInfo
        var mapInfo = MapDictStorage.Instance.GetMap(req.MapInfo.MapId);
        
        // TODO: Get Free Slot depending on map size
        var roomCenter = GetNextSlot(mapInfo.Settings);
        // TODO: create room
        if (id == "")
        {
            id = "room" + UnityEngine.Random.Range(0, int.MaxValue);
        }
        var room = new Room(id, new List<long>(), new List<long>(), req.MapInfo,req.GamemodeId, req.TimeInfo,0, Utility.Vector3ToVector3Float(roomCenter));

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
            
        };
        WorldCommandSender.SendCreateEntityCommand(new WorldCommands.CreateEntity.Request(roomManager), callback);
    }

    private Vector3 GetNextSlot(MapSettings mapSettings)
    {
        
        var activeRooms = WorldManagerWriter.Data.ActiveRooms.ToList();
        if(activeRooms.Count < 1)
        {
            return new Vector3(0, 0, 0);
        }
        activeRooms.Sort((KeyValuePair<string, Room> p1, KeyValuePair<string, Room> p2) =>
        {
            return p1.Value.Origin.X.CompareTo(p2.Value.Origin.X);
        });
        float lastPos = 0f;
        for (int i = 0; i < activeRooms.Count; i++)
        {
            var thisMap = MapDictStorage.Instance.GetMap(activeRooms[i].Value.MapInfo.MapId);
            var thisRoom = activeRooms[i].Value;

            lastPos = thisRoom.Origin.Z + thisMap.Settings.DimensionZ / 2;

            if (i + 1 >= activeRooms.Count)
            break;
            
            var nextMap = MapDictStorage.Instance.GetMap(activeRooms[i + 1].Value.MapInfo.MapId);
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
