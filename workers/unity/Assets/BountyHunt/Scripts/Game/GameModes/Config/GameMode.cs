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

}

public abstract class BountyGameMode : GameMode
{ 
    public abstract void PlayerKill(string killer, string victim, Vector3 position);

    public abstract void BountyTick(string player);
}

public struct GameModeFinancing
{
    public long totalSatAmount;
}
