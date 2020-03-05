using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class GameMode : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    public GameModeGlobalSettings GlobalSettings;
    public GameModePlayerSettings PlayerSettings;

    public abstract void ServerOnGameModeStart(ServerGameModeBehaviour serverGameModeBehaviour);
    public abstract void ServerOnGameModeEnd(ServerGameModeBehaviour serverGameModeBehaviour);

    public abstract void ClientOnGameModeStart(ClientGameModeBehaviour clientGameModeBehaviour);
    public abstract void ClientOnGameModeEnd(ClientGameModeBehaviour clientGameModeBehaviour);
}
