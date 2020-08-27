using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bbhrpc;
using UnityEngine.Events;

public abstract class GameMode : ScriptableObject, IBaseGameMode
{
    public string Name;
    public string GameModeId;
    public Sprite Icon;
    public GameModeSettings GameModeSettings;
    public IMapBountySpawnPointer MapInfo;
    public GameModeFinancing Financing;
    public IServerRoomGameStatsMap serverRoomGameStatsMap;
    public IBountyRoomSpawner bountyRoomSpawner;
    public Vector3 workerOrigin;

    public void Initialize(GameModeSettings settings, IMapBountySpawnPointer mapInfo, GameModeFinancing financing, IServerRoomGameStatsMap serverRoomGameStatsMap, IBountyRoomSpawner bountyRoomSpawner, Vector3 workerOrigin)
    {
        this.GameModeSettings = settings;
        this.MapInfo = mapInfo;
        this.Financing = financing;
        this.serverRoomGameStatsMap = serverRoomGameStatsMap;
        this.bountyRoomSpawner = bountyRoomSpawner;
        this.workerOrigin = workerOrigin;
    }
    public abstract void ServerOnGameModeStart();
    public abstract void ServerOnGameModeEnd();


}

public interface IBaseGameMode
{
    void ServerOnGameModeStart();
    void ServerOnGameModeEnd();
    
}

public interface IPlayerJoinLeaveEvents
{
    void OnPlayerJoin(string playerId);

    void OnPlayerLeave(string playerId);
}

public interface IUpdateGameMode
{
    void GameModeUpdate(float deltaTime);
}

public interface IKillGameMode
{
    void PlayerKill(string killer, string victim, Vector3 position);
}


public struct GameModeFinancing
{
    public long totalSatAmount;
}
