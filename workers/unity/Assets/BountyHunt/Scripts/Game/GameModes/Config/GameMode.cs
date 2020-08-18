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
    public abstract void ServerOnGameModeStart(RoomManagerServerBehaviour serverGameModeBehaviour,GameModeSettings settings,  long subsidy);
    public abstract void ServerOnGameModeEnd(RoomManagerServerBehaviour serverGameModeBehaviour);

    public abstract void ClientOnGameModeStart(RoomManagerClientBehaviour clientGameModeBehaviour);
    public abstract void ClientOnGameModeEnd(RoomManagerClientBehaviour clientGameModeBehaviour);
}
