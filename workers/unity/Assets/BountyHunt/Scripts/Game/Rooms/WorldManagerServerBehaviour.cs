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
    [Require] RoomManagerCommandSender RoomManagerCommandSender;

    [Require] WorldCommandSender WorldCommandSender;


    
    public bool startGenerated;
    public string mapID;
    public bool startPrefabMap;

    public static int cantinaPlayers = 20;
    private void OnEnable()
    {
        WorldManagerCommandReceiver.OnEndRoomRequestReceived += OnEndRoom;
        WorldManagerCommandReceiver.OnCreateRoomRequestReceived += OnCreateRoom;
        WorldManagerCommandReceiver.OnUpdateRoomRequestReceived += OnUpdateRoom;
        WorldManagerCommandReceiver.OnStartRoomRequestReceived += OnStartRoom;
        WorldManagerCommandReceiver.OnJoinRoomRequestReceived += OnJoinRoom;
        WorldManagerCommandReceiver.OnAddActivePlayerRequestReceived += AddActivePlayer;
        WorldManagerCommandReceiver.OnRemoveActivePlayerRequestReceived += RemoveActivePlayer;
        WorldManagerCommandReceiver.OnGetCantinaRequestReceived += OnGetCantina;
        // TODO: check for existing rooms and instantiate them if neccesarry
        // TODO: really check for cantina
        if(WorldManagerWriter.Data.ActiveRooms.Count < 1)
        {
            CreateRoom(new CreateRoomRequest(new MapInfo("cantina",""), "lobby", new TimeInfo(System.DateTime.UtcNow.ToFileTimeUtc(), long.MaxValue), cantinaPlayers),"cantina-"+cantinaPlayers );
            cantinaPlayers++;
        }
    }

    

    private void RemoveActivePlayer(WorldManager.RemoveActivePlayer.ReceivedRequest obj)
    {
        var map = WorldManagerWriter.Data.ActivePlayers;
        if (map.TryGetValue(obj.Payload.PlayerPk, out var player)) {

            map.Remove(obj.Payload.PlayerPk);
            WorldManagerWriter.SendUpdate(new WorldManager.Update()
            {
                ActivePlayers = map
            });
            RoomManagerCommandSender.SendRemovePlayerCommand(player.ActiveRoom, new RemovePlayerRequest(obj.Payload.PlayerPk));
        }
    }

    private async void AddActivePlayer(WorldManager.AddActivePlayer.ReceivedRequest obj)
    {
        var res = await ServerServiceConnections.instance.lnd.VerifyMessage(Utility.AuthMessage, obj.Payload.Signature);
        var map = WorldManagerWriter.Data.ActivePlayers;
        var bbhbackend = ServerServiceConnections.instance.BackendGameServerClient;
        var user = await bbhbackend.GetUser(res.Pubkey);
        if (!map.ContainsKey(obj.Payload.PlayerPk))
        {
            map.Add(obj.Payload.PlayerPk, new Bountyhunt.PlayerInfo(obj.Payload.PlayerId, user.Name,new EntityId(0)));
        } else
        {
            map[obj.Payload.PlayerPk] = new Bountyhunt.PlayerInfo(obj.Payload.PlayerId, user.Name, new EntityId(0));
        }
        WorldManagerWriter.SendUpdate(new WorldManager.Update()
        {
            ActivePlayers = map
        });
        WorldManagerCommandReceiver.SendAddActivePlayerResponse(obj.RequestId, new Bountyhunt.Empty());
    }

    private void OnJoinRoom(WorldManager.JoinRoom.ReceivedRequest obj)
    {
        Debug.LogFormat("Join room {0}, {1}", obj.Payload.RoomId, obj.Payload.PlayerId);
        var map = WorldManagerWriter.Data.ActiveRooms;
        
        if (!map.TryGetValue(obj.Payload.RoomId, out var room))
        {
            WorldManagerCommandReceiver.SendJoinRoomFailure(obj.RequestId, "room does not exist");
            return;
        }
        if(room.EntityId.Id == 0)
        {
            WorldManagerCommandReceiver.SendJoinRoomFailure(obj.RequestId, "room not ready yet");
            return;
        }
        var playerPk = GetPlayerPKByEntity(obj.Payload.PlayerId);
        // TODO queue management
        RoomManagerCommandSender.SendAddPlayerCommand(room.EntityId, new AddPlayerRequest(playerPk, obj.Payload.PlayerId));
        WorldManagerCommandReceiver.SendJoinRoomResponse(obj.RequestId, new Bountyhunt.Empty());

        UpdateActivePlayerRoom(playerPk, room.EntityId);
    }
    private void OnGetCantina(WorldManager.GetCantina.ReceivedRequest obj)
    {
        var newCantina = GetFreeCantina();
        var newlyCreated = false;
        if (newCantina.RoomId == "none")
        {
            newCantina = CreateRoom(new CreateRoomRequest(new MapInfo("cantina", ""), "lobby", new TimeInfo(System.DateTime.UtcNow.ToFileTimeUtc(), long.MaxValue), cantinaPlayers), "cantina-" + cantinaPlayers);
            cantinaPlayers++;
            newlyCreated = true;
        }
        WorldManagerCommandReceiver.SendGetCantinaResponse(obj.RequestId, new GetCantinaResponse(newCantina, newlyCreated));
    }

    private Room GetFreeCantina()
    {
        var map = WorldManagerWriter.Data.ActiveRooms;
        var cantinalist = new List<Room>();
        foreach (var room in map)
        {
            if(room.Key.Contains("cantina"))
            {
                cantinalist.Add(room.Value);
            }
        }
        var newCantina = new Room() {
            RoomId = "none"
        };
        foreach(var cantina in cantinalist)
        {
            if(cantina.MaxPlayers > cantina.ActivePlayers.Count)
            {
                newCantina = cantina;
                break;
            }
        }
        return newCantina;
    }
    private void UpdateActivePlayerRoom(string key, EntityId roomId)
    {
        var map = WorldManagerWriter.Data.ActivePlayers;
        if (map.ContainsKey(key))
        {
            var player = map[key];
            player.ActiveRoom = roomId;
            map[key] = player;
        }
        WorldManagerWriter.SendUpdate(new WorldManager.Update()
        {
            ActivePlayers = map
        });
    }
    private string GetPlayerPKByEntity(EntityId id)
    {
        var map = WorldManagerWriter.Data.ActivePlayers;
        foreach (var kv in map)
        {
            if(kv.Value.EntityId == id)
            {
                return kv.Key;
            }
        }
        return "";
    }

    private void OnStartRoom(WorldManager.StartRoom.ReceivedRequest obj)
    {
  
    }

    private void OnUpdateRoom(WorldManager.UpdateRoom.ReceivedRequest obj)
    {
        var map = WorldManagerWriter.Data.ActiveRooms;

        if (!map.TryGetValue(obj.Payload.Room.RoomId, out var room))
        {
            WorldManagerCommandReceiver.SendJoinRoomFailure(obj.RequestId, "room does not exist");
            return;
        }
        map[obj.Payload.Room.RoomId] = obj.Payload.Room;
        WorldManagerWriter.SendUpdate(new WorldManager.Update
        {
            ActiveRooms = map
        });
    }

    private void Update()
    {
        
        if (startGenerated)
        {
            startGenerated = false;
            CreateRoom(new CreateRoomRequest(new MapInfo("generated_20",UnityEngine.Random.Range(float.MinValue, float.MaxValue).ToString()), "lobby", new TimeInfo(System.DateTime.UtcNow.ToFileTimeUtc(), long.MaxValue), 20));
        }

        if(startPrefabMap)
        {
            startPrefabMap = false;
            var mapInfo = new MapInfo(mapID,Utility.GetUniqueString());
            var req = new CreateRoomRequest(mapInfo,"satsstacker", new TimeInfo(System.DateTime.UtcNow.ToFileTimeUtc(), long.MaxValue),10);
            CreateRoom(req);
        }
    }
    private void OnCreateRoom(WorldManager.CreateRoom.ReceivedRequest obj)
    {
        
    }

    private void OnEndRoom(WorldManager.EndRoom.ReceivedRequest obj)
    {
        
    }


    private Room CreateRoom(CreateRoomRequest req, string id = "")
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
        var room = new Room(id, new List<string>(), new List<string>(), req.MapInfo,req.GamemodeId, req.TimeInfo,new EntityId(), Utility.Vector3ToVector3Float(roomCenter), req.MaxPlayers);

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
            room.EntityId = res.EntityId.Value;
            var map = WorldManagerWriter.Data.ActiveRooms;
            map.Add(room.RoomId, room);
            WorldManagerWriter.SendUpdate(new WorldManager.Update()
            {
                ActiveRooms = map
            });
            
        };
        WorldCommandSender.SendCreateEntityCommand(new WorldCommands.CreateEntity.Request(roomManager), callback);
        return room;
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
