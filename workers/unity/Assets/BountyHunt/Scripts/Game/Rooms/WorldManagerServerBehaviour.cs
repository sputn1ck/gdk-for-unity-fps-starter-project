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
    [Require] RoomPlayerCommandSender RoomPlayerCommandSender;


    
    public bool startGenerated;
    public static int cantinaCounter = 1;
    
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
            var cantinaRotation = new List<ModeRotationItem>()
           {
               new ModeRotationItem("lobby", Utility.SecondsToNano(300))
           };
            var cantinaRequest = new CreateRoomRequest()
            {
                Advertisers = null,
                MapInfo = new MapInfo("cantina", ""),
                MaxPlayers = 20,
                ModeRotation = cantinaRotation,
                Repetitions = int.MaxValue,
                StartTime = System.DateTime.UtcNow.ToFileTimeUtc(),
                StartSats = 0, 
            };
            CreateRoom((cantinaRequest), "cantina-"+cantinaCounter);
            cantinaCounter++;
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
        if(room.Info.EntityId.Id == 0)
        {
            WorldManagerCommandReceiver.SendJoinRoomFailure(obj.RequestId, "room not ready yet");
            return;
        }
        var playerPk = GetPlayerPKByEntity(obj.Payload.PlayerId);
        if (room.PlayerInfo.ActivePlayers.Contains(playerPk))
        {
            WorldManagerCommandReceiver.SendJoinRoomFailure(obj.RequestId, "already connected to room");
            return;
        }
        // TODO queue management
        RoomManagerCommandSender.SendAddPlayerCommand(room.Info.EntityId, new AddPlayerRequest(playerPk, obj.Payload.PlayerId));
        WorldManagerCommandReceiver.SendJoinRoomResponse(obj.RequestId, new Bountyhunt.Empty());

        UpdateActivePlayerRoom(playerPk, room.Info.EntityId);
    }
    private void OnGetCantina(WorldManager.GetCantina.ReceivedRequest obj)
    {
        var newCantina = GetFreeCantina();
        var newlyCreated = false;
        if (newCantina.Info.RoomId == "none")
        {
            var cantinaRotation = new List<ModeRotationItem>()
           {
               new ModeRotationItem("lobby", Utility.SecondsToNano(300))
           };
            var cantinaRequest = new CreateRoomRequest()
            {
                Advertisers = null,
                MapInfo = new MapInfo("cantina", ""),
                MaxPlayers = 20,
                ModeRotation = cantinaRotation,
                Repetitions = int.MaxValue,
                StartTime = System.DateTime.UtcNow.ToFileTimeUtc(),
                StartSats = 0,
            };
            newCantina = CreateRoom(cantinaRequest, "cantina-"+cantinaCounter);
            cantinaCounter++;
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
        var newCantina = new Room()
        {
            Info = new RoomBaseInfo()
            {
                RoomId = "none"
            }
        };
        foreach (var cantina in cantinalist)
        {
            if(cantina.PlayerInfo.MaxPlayers > cantina.PlayerInfo.ActivePlayers.Count)
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

        if (!map.TryGetValue(obj.Payload.Room.Info.RoomId, out var room))
        {
            WorldManagerCommandReceiver.SendJoinRoomFailure(obj.RequestId, "room does not exist");
            return;
        }
        map[obj.Payload.Room.Info.RoomId] = obj.Payload.Room;
        WorldManagerWriter.SendUpdate(new WorldManager.Update
        {
            ActiveRooms = map
        });
    }

    private void Update()
    {
        
        if (startGenerated)
        {
            var rotation = new List<ModeRotationItem>()
           {
               new ModeRotationItem("lobby", Utility.SecondsToNano(120)),
               new ModeRotationItem("satsstacker",Utility.SecondsToNano(240))
           };
            startGenerated = false;
            var startTime = System.DateTime.UtcNow.AddSeconds(10).ToFileTimeUtc();
            var roomRequest = new CreateRoomRequest()
            {
                Advertisers = null,
                MapInfo = new MapInfo("generated_20", UnityEngine.Random.Range(float.MinValue, float.MaxValue).ToString()),
                MaxPlayers = 20,
                ModeRotation = rotation,
                Repetitions = 2,
                StartTime = startTime,
                StartSats = 0,
            };
            CreateRoom(roomRequest);
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
        Debug.Log("Deleting room");

        WorldCommandSender.SendDeleteEntityCommand(new WorldCommands.DeleteEntity.Request(obj.Payload.EntityId));
        var map = WorldManagerWriter.Data.ActiveRooms;
        if (map.TryGetValue(obj.Payload.RoomId, out var room))
        {
            foreach(var playerString in room.PlayerInfo.ActivePlayers)
            {
                if(WorldManagerWriter.Data.ActivePlayers.TryGetValue(playerString, out var player))
                {
                    RoomPlayerCommandSender.SendSendToCantinaCommand(player.EntityId, new Bountyhunt.Empty());
                }
            }
        }
        map.Remove(obj.Payload.RoomId);
        WorldManagerWriter.SendUpdate(new WorldManager.Update
        {
            ActiveRooms = map
        });
    }


    private Room CreateRoom(CreateRoomRequest req, string id = "")
    {
        // Get MapInfo
        var mapInfo = MapDictStorage.Instance.GetMap(req.MapInfo.MapId);
        
        // Get Free Slot depending on map size
        var roomCenter = GetNextSlot(mapInfo.Settings);
        // create room
        if (id == "")
        {
            id = "room" + UnityEngine.Random.Range(0, int.MaxValue);
        }
        RoomFinanceInfo financeInfo;
        if (req.Advertisers == null) {
            financeInfo = new RoomFinanceInfo(req.StartSats, 0, 0, null);
        } else
        {
            financeInfo = new RoomFinanceInfo(req.StartSats, 0, 0, new FixedAdvertisers(req.Advertisers));
        }
        var room = new Room()
        {
            Info = new RoomBaseInfo(id, req.MapInfo, new EntityId(), Utility.Vector3ToVector3Float(roomCenter), req.StartTime),
            GameModeInfo = new RoomGameModeInfo(req.ModeRotation, req.Repetitions, 0),
            PlayerInfo = new RoomPlayerInfo(new List<string>(), new List<string>(), req.MaxPlayers),
            FinanceInfo = financeInfo
        };
        var roomManager = DonnerEntityTemplates.RoomManager(roomCenter, room);

        var list = WorldManagerWriter.Data.ActiveRooms;
        System.Action<WorldCommands.CreateEntity.ReceivedResponse> callback = (WorldCommands.CreateEntity.ReceivedResponse res) =>
        {
            if (res.StatusCode != Improbable.Worker.CInterop.StatusCode.Success)
            {
                Debug.LogErrorFormat("{0} RoomCreation errpr",  res.Message);
                return;
            }
            Debug.LogFormat("{0} created; res {1}", room.Info.RoomId, res);
            room.Info.EntityId = res.EntityId.Value;
            var map = WorldManagerWriter.Data.ActiveRooms;
            map.Add(room.Info.RoomId, room);
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
            return p1.Value.Info.Origin.X.CompareTo(p2.Value.Info.Origin.X);
        });
        float lastPos = 0f;
        for (int i = 0; i < activeRooms.Count; i++)
        {
            var thisMap = MapDictStorage.Instance.GetMap(activeRooms[i].Value.Info.MapInfo.MapId);
            var thisRoom = activeRooms[i].Value;

            lastPos = thisRoom.Info.Origin.Z + thisMap.Settings.DimensionZ / 2;

            if (i + 1 >= activeRooms.Count)
            break;
            
            var nextMap = MapDictStorage.Instance.GetMap(activeRooms[i + 1].Value.Info.MapInfo.MapId);
            var nextRoom = activeRooms[i + 1].Value;

            var spaceBetweenMaps = (nextRoom.Info.Origin.Z - nextMap.Settings.DimensionZ / 2) - (thisRoom.Info.Origin.Z + thisMap.Settings.DimensionZ / 2);
            if (spaceBetweenMaps >= mapSettings.DimensionZ)
            {
                break;
            }
        }
        return new Vector3(0,0,lastPos + mapSettings.DimensionZ/2);
      
    }

}
