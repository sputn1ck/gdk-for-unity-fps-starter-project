using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Map : ScriptableObject
{
    public string MapId;
    public MapSettings Settings;

    public abstract void Initialize(MonoBehaviour caller, bool isServer, Vector3 spawnPosition, string mapData, UnityAction onFinished = null);
    public abstract void Remove();
}

[System.Serializable]
public class MapSettings
{
    public float DimensionX;
    public float DimensionZ;
    public List<string> SupportedGameModes;
    public int RecommendedPlayers;
}
