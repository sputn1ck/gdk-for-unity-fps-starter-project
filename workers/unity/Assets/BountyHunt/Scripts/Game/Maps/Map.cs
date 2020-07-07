using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Map : ScriptableObject, IMap
{
    public string MapId;
    public MapSettings Settings;

    public abstract void Initialize(bool isServer, Vector3 spawnPosition);
}

[System.Serializable]
public class MapSettings
{
    public float DimensionX;
    public float DimensionZ;
    public List<string> SupportedGameModes;
    public int RecommendedPlayers;
}
