using Improbable.Gdk.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Map : ScriptableObject
{
    public string MapId;
    public EntityId EntityId;
    public MapSettings Settings;
    public List<EntityId> LevelObjects;
    public abstract void Initialize(MonoBehaviour caller, bool isServer, Vector3 spawnPosition, string mapData, UnityAction onFinished = null, WorldCommandSender worldCommandSender = null);

    public abstract (Vector3 pos, float yaw, float pitch) GetSpawnPoint();
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
