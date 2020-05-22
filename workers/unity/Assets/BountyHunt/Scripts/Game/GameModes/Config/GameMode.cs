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
    public abstract void ServerOnGameModeStart(ServerGameModeBehaviour serverGameModeBehaviour, GameModeSettings settings, long subsidy);
    public abstract void ServerOnGameModeEnd(ServerGameModeBehaviour serverGameModeBehaviour);

    public abstract void ClientOnGameModeStart(ClientGameModeBehaviour clientGameModeBehaviour);
    public abstract void ClientOnGameModeEnd(ClientGameModeBehaviour clientGameModeBehaviour);
}
