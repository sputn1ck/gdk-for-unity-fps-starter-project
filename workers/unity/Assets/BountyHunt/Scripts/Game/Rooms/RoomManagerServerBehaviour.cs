using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Commands;
using UnityEngine.Events;
using System;

public class RoomManagerServerBehaviour : MonoBehaviour
{
    [Require] RoomManagerWriter RoomManagerWriter;
    [Require] RoomManagerCommandReceiver RoomManagerCommandReceiver;
    [Require] WorldManagerCommandSender WorldManagerCommandSender;
    [Require] RoomPlayerCommandSender RoomPlayerCommandSender;
    [Require] WorldCommandSender WorldCommandSender;
    [Require] EntityId EntityId;
    [Require] HunterComponentCommandSender HunterComponentCommandSender;
    public LinkedEntityComponent LinkedEntityComponent;

    private Map mapInfo;

    private ServerRoomGameModeBehaviour ServerRoomGameModeBehaviour;
    private ServerRoomGameStatsMap statsMap;
    private UnityAction RotationEnded;

    private void OnEnable()
    {

        statsMap = GetComponent<ServerRoomGameStatsMap>();
        LinkedEntityComponent = GetComponent<LinkedEntityComponent>();
        

        RoomManagerCommandReceiver.OnAddPlayerRequestReceived += AddPlayer;
        RoomManagerCommandReceiver.OnRemovePlayerRequestReceived += RemovePlayer;
        RoomManagerCommandReceiver.OnReadyToJoinRequestReceived += ReadyToJoin;
        RoomManagerCommandReceiver.OnAddRoomboundObjectRequestReceived += AddRoomBoundObject;
        RoomManagerCommandReceiver.OnGetSpawnPositionRequestReceived += GetSpawnPosition;
        InitializePlayerStats();
        // start gamemode rotation
        ServerRoomGameModeBehaviour = GetComponent<ServerRoomGameModeBehaviour>();
        ServerRoomGameModeBehaviour.AddFinishedAction(CloseRoom);

        Initialize();
    }

    private void GetSpawnPosition(RoomManager.GetSpawnPosition.ReceivedRequest obj)
    {
        var sP = mapInfo.GetSpawnPoint();
        var room = RoomManagerWriter.Data.RoomInfo;
        var newpos = Utility.Vector3ToVector3Float(sP.pos);
        newpos.X += room.Info.Origin.X;
        newpos.Y += room.Info.Origin.Y;
        newpos.Z += room.Info.Origin.Z;
        Debug.Log("respawning at " + newpos);
        RoomManagerCommandReceiver.SendGetSpawnPositionResponse(obj.RequestId, new SpawnPosition(newpos, sP.yaw, sP.pitch));
    }

    private void Update()
    {
        if(RoomManagerWriter.Data.RoomState == RoomState.CREATED)
        {
            CheckIfStartGameMode();
        }
    }

    private void CheckIfStartGameMode()
    {
        if(DateTime.UtcNow.ToFileTime() > RoomManagerWriter.Data.RoomInfo.Info.StartTime )
        {
            RoomManagerWriter.SendUpdate(new RoomManager.Update()
            {
                RoomState = RoomState.STARTED
            });
            ServerRoomGameModeBehaviour.StartRotation();
            
            Debug.Log("Starting Room");
        }
    }
    private void CloseRoom()
    {
        Debug.Log("Closing Room");
        WorldManagerCommandSender.SendEndRoomCommand(new EntityId(3), new EndRoomRequest(this.EntityId, RoomManagerWriter.Data.RoomInfo.Info.RoomId));
        RoomManagerWriter.SendUpdate(new RoomManager.Update()
        {
            RoomState = RoomState.ENDED
        });
    }

    private void AddRoomBoundObject(RoomManager.AddRoomboundObject.ReceivedRequest obj)
    {
        mapInfo.LevelObjects.Add(obj.Payload.EntityId);
    }

    private  void InitializePlayerStats()
    {

        var room = RoomManagerWriter.Data.RoomInfo;
        room.Info.EntityId = EntityId;
        SendUpdates(room);
        statsMap.Initialize(room);

    }
    

    private void ReadyToJoin(RoomManager.ReadyToJoin.ReceivedRequest obj)
    {
        var room = RoomManagerWriter.Data.RoomInfo;
        //TODO get spawnpoint
        var spawnPoint = mapInfo.GetSpawnPoint();

        HunterComponentCommandSender.SendTeleportPlayerCommand(obj.Payload.PlayerId, new TeleportRequest()
        {
            Heal = true,
            X = room.Info.Origin.X +spawnPoint.pos.x,
            Y = room.Info.Origin.Y + spawnPoint.pos.y,
            Z = room.Info.Origin.Z + spawnPoint.pos.z
        }, (cb) => {
            if (cb.StatusCode != Improbable.Worker.CInterop.StatusCode.Success)
            {
                Debug.LogError(cb.Message);
                RoomManagerCommandReceiver.SendReadyToJoinFailure(obj.RequestId, "teleport failed: "+cb.Message);
            }
        });
        RoomManagerCommandReceiver.SendReadyToJoinResponse(obj.RequestId, new Bountyhunt.Empty());
        if(ServerRoomGameModeBehaviour.currentMode != null && ServerRoomGameModeBehaviour.currentMode is IPlayerJoinLeaveEvents)
        {
            (ServerRoomGameModeBehaviour.currentMode as IPlayerJoinLeaveEvents).OnPlayerJoin(obj.Payload.PlayerPubkey);
        }

    }

    private void RemovePlayer(RoomManager.RemovePlayer.ReceivedRequest obj)
    {
        if(ServerRoomGameModeBehaviour.currentMode != null && ServerRoomGameModeBehaviour.currentMode is IPlayerJoinLeaveEvents)
        {
            (ServerRoomGameModeBehaviour.currentMode as IPlayerJoinLeaveEvents).OnPlayerLeave(obj.Payload.PlayerPk);
        }
        statsMap.RemovePlayer(obj.Payload.PlayerPk);
        var room = RoomManagerWriter.Data.RoomInfo;
        if(room.PlayerInfo.ActivePlayers.Remove(obj.Payload.PlayerPk))
        {
            SendUpdates(room);
        }
    }

    private void AddPlayer(RoomManager.AddPlayer.ReceivedRequest obj)
    {
        Debug.Log("called addplayer on room adding player to room " + this.EntityId);
        statsMap.AddPlayer(obj.Payload.PlayerPk);
        
        var room = RoomManagerWriter.Data.RoomInfo;
        room.Info.EntityId = EntityId;
        room.PlayerInfo.ActivePlayers.Add(obj.Payload.PlayerPk, obj.Payload.PlayerId);
        SendUpdates(room);
        RoomPlayerCommandSender.SendUpdatePlayerRoomCommand(obj.Payload.PlayerId, new UpdatePlayerRoomRequest(room));
    }
   
    public void SendUpdates(Room room)
    {
        Debug.Log("Send updates called");
        RoomManagerWriter.SendUpdate(new RoomManager.Update()
        {
            RoomInfo = room
        }) ;
        WorldManagerCommandSender.SendUpdateRoomCommand(new EntityId(3), new UpdateRoomRequest()
        {
            Room = room
        });
    }



    private void Initialize()
    {
        mapInfo = Instantiate(MapDictStorage.Instance.GetMap(RoomManagerWriter.Data.RoomInfo.Info.MapInfo.MapId));
        mapInfo.EntityId = EntityId;
        mapInfo.LevelObjects = new List<EntityId>();
        mapInfo.Initialize(this, true, this.transform.position, RoomManagerWriter.Data.RoomInfo.Info.MapInfo.MapData, null, WorldCommandSender);
        ServerRoomGameModeBehaviour.Setup(mapInfo);
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
        Debug.Log("room destroyed");
    }
}
