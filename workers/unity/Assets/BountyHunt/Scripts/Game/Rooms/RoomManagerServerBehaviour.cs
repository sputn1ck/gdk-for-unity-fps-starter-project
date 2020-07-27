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
    [Require] RoomStatsWriter RoomStatsWriter;
    [Require] RoomStatsCommandReceiver RoomStatsCommandReceiver;
    [Require] WorldManagerCommandSender WorldManagerCommandSender;
    [Require] RoomPlayerCommandSender RoomPlayerCommandSender;
    [Require] WorldCommandSender WorldCommandSender;
    [Require] EntityId EntityId;
    [Require] HunterComponentCommandSender HunterComponentCommandSender;
    public LinkedEntityComponent LinkedEntityComponent;

    private Map mapInfo;

    private Dictionary<string, PlayerStats> playerStats;
    private ServerRoomGameModeBehaviour ServerRoomGameModeBehaviour;

    private void OnEnable()
    {


        LinkedEntityComponent = GetComponent<LinkedEntityComponent>();
        Initialize();

        RoomManagerCommandReceiver.OnAddPlayerRequestReceived += AddPlayer;
        RoomManagerCommandReceiver.OnRemovePlayerRequestReceived += RemovePlayer;
        RoomManagerCommandReceiver.OnReadyToJoinRequestReceived += ReadyToJoin;
        RoomStatsCommandReceiver.OnRequestStatsRequestReceived += RequestStats;
        RoomManagerCommandReceiver.OnAddRoomboundObjectRequestReceived += AddRoomBoundObject;
        InitializePlayerStats();
        // start gamemode rotation
        ServerRoomGameModeBehaviour = GetComponent<ServerRoomGameModeBehaviour>();
        ServerRoomGameModeBehaviour.StartRotation();
    }

    private void CloseRoom()
    {
        WorldManagerCommandSender.SendEndRoomCommand(new EntityId(3), new EndRoomRequest());
    }

    private void AddRoomBoundObject(RoomManager.AddRoomboundObject.ReceivedRequest obj)
    {
        mapInfo.LevelObjects.Add(obj.Payload.EntityId);
    }

    private  void InitializePlayerStats()
    {

        playerStats = new Dictionary<string, PlayerStats>();
        var room = RoomManagerWriter.Data.RoomInfo;
        room.EntityId = EntityId;
        foreach(var player in room.ActivePlayers)
        {
            playerStats.Add(player, new PlayerStats(0, 0, 0, 0, false));
        }
        SendUpdates(room);

    }
    private void RequestStats(RoomStats.RequestStats.ReceivedRequest obj)
    {
        RoomStatsCommandReceiver.SendRequestStatsResponse(obj.RequestId, new PlayerStatsUpdate(playerStats, new List<string>(), true));
    }

    private void ReadyToJoin(RoomManager.ReadyToJoin.ReceivedRequest obj)
    {
        var room = RoomManagerWriter.Data.RoomInfo;
        //TODO get spawnpoint
        var spawnPoint = mapInfo.GetSpawnPoint();

        HunterComponentCommandSender.SendTeleportPlayerCommand(obj.Payload.PlayerId, new TeleportRequest()
        {
            Heal = true,
            X = room.Origin.X +spawnPoint.x,
            Y = room.Origin.Y + spawnPoint.y,
            Z = room.Origin.Z + spawnPoint.z
        }, (cb) => {
            if (cb.StatusCode != Improbable.Worker.CInterop.StatusCode.Success)
            {
                Debug.LogError(cb.Message);
                RoomManagerCommandReceiver.SendReadyToJoinFailure(obj.RequestId, "teleport failed: "+cb.Message);
            }
        });
        RoomManagerCommandReceiver.SendReadyToJoinResponse(obj.RequestId, new Bountyhunt.Empty());
    }

    private void RemovePlayer(RoomManager.RemovePlayer.ReceivedRequest obj)
    {
        var map = new Dictionary<string, PlayerStats>();
        if (playerStats.TryGetValue(obj.Payload.PlayerPk, out var player))
        {
            player.Active = false;
            map[obj.Payload.PlayerPk] = player;
            var room = RoomManagerWriter.Data.RoomInfo;
            room.ActivePlayers.Remove(obj.Payload.PlayerPk);
            SendUpdates(room);
            UpdateDictionary(map);
        }
    }

    private void AddPlayer(RoomManager.AddPlayer.ReceivedRequest obj)
    {
        var map = new Dictionary<string, PlayerStats>();
        if (playerStats.TryGetValue(obj.Payload.PlayerPk, out var player))
        {
            player.Active = true;
            map[obj.Payload.PlayerPk] = player;
        } else
        {
            map.Add(obj.Payload.PlayerPk, new PlayerStats(0, 0, 0, 0, true));
        }
        var room = RoomManagerWriter.Data.RoomInfo;
        room.EntityId = EntityId;
        room.ActivePlayers.Add(obj.Payload.PlayerPk);
        SendUpdates(room);
        UpdateDictionary(map);
        RoomPlayerCommandSender.SendUpdatePlayerRoomCommand(obj.Payload.PlayerId, new UpdatePlayerRoomRequest(room));
    }
   
    private void SendUpdates(Room room)
    {
        RoomManagerWriter.SendUpdate(new RoomManager.Update()
        {
            RoomInfo = room
        }) ;
        WorldManagerCommandSender.SendUpdateRoomCommand(new EntityId(3), new UpdateRoomRequest()
        {
            Room = room
        });
    }

    private void UpdateDictionary(Dictionary<string, PlayerStats> newMap)
    {
        foreach(var kv in newMap)
        {
            if (playerStats.ContainsKey(kv.Key))
            {
                playerStats[kv.Key] = kv.Value;
            } else
            {
                playerStats.Add(kv.Key, kv.Value);
            }
            
        }
        RoomStatsWriter.SendMapUpdateEvent(new PlayerStatsUpdate(newMap ,new List<string>() , false));
    }


    private void Initialize()
    {
        mapInfo = Instantiate(MapDictStorage.Instance.GetMap(RoomManagerWriter.Data.RoomInfo.MapInfo.MapId));
        mapInfo.EntityId = EntityId;
        mapInfo.LevelObjects = new List<EntityId>();
        mapInfo.Initialize(this, true, this.transform.position, RoomManagerWriter.Data.RoomInfo.MapInfo.MapData, null, WorldCommandSender);
    }

    private void OnDisable()
    {
        var levelObjects = mapInfo.LevelObjects;
        foreach(var levelObject in levelObjects)
        {
            Debug.Log("deleting entityid: "+ levelObject.Id.ToString());
            WorldCommandSender.SendDeleteEntityCommand(new WorldCommands.DeleteEntity.Request(levelObject));
        }
        mapInfo.Remove();
        Destroy(mapInfo);
    }
}
