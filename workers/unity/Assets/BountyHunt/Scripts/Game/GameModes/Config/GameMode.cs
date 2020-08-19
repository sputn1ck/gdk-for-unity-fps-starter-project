using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bbhrpc;

public abstract class GameMode : ScriptableObject
{
    public string Name;
    public string GameModeId;
    public Sprite Icon;
    public GameModeSettings GameModeSettings;
    public Map MapInfo;
    public GameModeFinancing Financing;

    public void Initialize(GameModeSettings settings, Map mapInfo, GameModeFinancing financing)
    {
        this.GameModeSettings = settings;
        this.MapInfo = mapInfo;
        this.Financing = financing;
       
    }
    public abstract void ServerOnGameModeStart(ServerRoomGameModeBehaviour serverGameModeBehaviour);
    public abstract void ServerOnGameModeEnd(ServerRoomGameModeBehaviour serverGameModeBehaviour);
    public abstract void GameModeUpdate(float deltaTime);
    public abstract void ClientOnGameModeStart(RoomManagerClientBehaviour clientGameModeBehaviour);
    public abstract void ClientOnGameModeEnd(RoomManagerClientBehaviour clientGameModeBehaviour);
}

public struct GameModeFinancing
{
    public long totalSatAmount;
}
