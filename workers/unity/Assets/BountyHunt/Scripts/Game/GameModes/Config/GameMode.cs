using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class GameMode : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    public GameModeGlobalSettings GlobalSettings;
    public GameModePlayerSettings PlayerSettings;

    public abstract void OnGameModeStart(ServerGameModeBehaviour serverGameModeBehaviour);
    public abstract void OnGameModeEnd(ServerGameModeBehaviour serverGameModeBehaviour);
}
