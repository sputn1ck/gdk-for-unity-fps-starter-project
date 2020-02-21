using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "BBH/GameModes/GameMode", order = 0)]
public class GameMode : ScriptableObject
{
    public string Name;
    public GameModeGlobalSettings GlobalSettings;
    public GameModePlayerSettings PlayerSettings;
}
