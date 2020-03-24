using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bbh;

public abstract class GameMode : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    public GameModeSettings GameModeSettings;
    public abstract void ServerOnGameModeStart(ServerGameModeBehaviour serverGameModeBehaviour, GameModeSettings settings, long subsidy);
    public abstract void ServerOnGameModeEnd(ServerGameModeBehaviour serverGameModeBehaviour);

    public abstract void ClientOnGameModeStart(ClientGameModeBehaviour clientGameModeBehaviour);
    public abstract void ClientOnGameModeEnd(ClientGameModeBehaviour clientGameModeBehaviour);
}
