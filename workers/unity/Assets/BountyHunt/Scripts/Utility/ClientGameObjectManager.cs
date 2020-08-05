using Fps.Movement;
using Improbable.Gdk.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ClientGameObjectManager : MonoBehaviour
{
    public static ClientGameObjectManager Instance;
    public GameObject AuthorativePlayerGameObject { get; private set; }
    public EntityId AuthorativePlayerEntityId;

    public EntityId ActiveRoomEntityId;
    public GameObject ActiveRoom;

    public Dictionary<EntityId, GameObject> PlayerGameObjects;
    public Dictionary<EntityId, GameObject> BountyTracers;
    public Dictionary<EntityId, GameObject> Rooms;
    public Dictionary<string, Bountyhunt.PlayerInfo> PlayerInfos;
    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(Instance);
            Instance = this;
        }

        PlayerGameObjects = new Dictionary<EntityId, GameObject>();
        BountyTracers = new Dictionary<EntityId, GameObject>();
        Rooms = new Dictionary<EntityId, GameObject>();
        PlayerInfos = new Dictionary<string, Bountyhunt.PlayerInfo>();
    }

    public void SetAuthorativePlayer(GameObject go, EntityId id)
    {
        AuthorativePlayerGameObject = go;
        AuthorativePlayerEntityId = id;
    }

    public void DisableAuthorativePlayer()
    {
        AuthorativePlayerGameObject = null;
        AuthorativePlayerEntityId = new EntityId();
    }

    public void AddPlayerGO(EntityId id, GameObject go)
    {
        if (PlayerGameObjects.ContainsKey(id))
        {
            PlayerGameObjects[id] = go;
        } else
        {
            PlayerGameObjects.Add(id, go);
        }
    }

    public void RemovePlayerGO(EntityId id)
    {
        if (PlayerGameObjects.ContainsKey(id))
        {

            PlayerGameObjects.Remove(id);
        }
    }

    public void AddRoomGo(EntityId id, GameObject go)
    {
        if (Rooms.ContainsKey(id))
        {
            Rooms[id] = go;
        } else
        {
            Rooms.Add(id, go);
        }
    }

    public void RemoveRoomGo(EntityId id)
    {
        if (Rooms.ContainsKey(id))
        {
            Rooms.Remove(id);
        }
    }
    public GameObject GetRoomGO(EntityId id)
    {
        if (Rooms.ContainsKey(id))
        {
            return Rooms[id];
        }
        return null;
    }

    public GameObject GetPlayerGameObject(EntityId id)
    { 
        if (PlayerGameObjects.ContainsKey(id))
        {

            return PlayerGameObjects[id];
        }
        return null;
    }
    public void AddBountyTracerGO(EntityId connectedPlayerId, GameObject go)
    {
        if (BountyTracers.ContainsKey(connectedPlayerId))
        {
            BountyTracers[connectedPlayerId] = go;
        }
        else
        {
            BountyTracers.Add(connectedPlayerId, go);
        }
    }

    public void RemoveBountyTracerGO(EntityId connectedPlayerId)
    {
        if (BountyTracers.ContainsKey(connectedPlayerId))
        {

            BountyTracers.Remove(connectedPlayerId);
        }
    }

    public GameObject GetBountyTracer(EntityId connectedPlayerId)
    {
        if (BountyTracers.ContainsKey(connectedPlayerId))
        {

            return BountyTracers[connectedPlayerId];
        }
        return null;
    }
}
