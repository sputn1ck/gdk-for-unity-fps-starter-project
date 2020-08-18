using Fps.Respawning;
using Fps.WorldTiles;
using Improbable.Gdk.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "BBH/Maps/GeneratedMap", order = 1)]
public class GeneratedMap : Map
{
    public int layers;
    public MapTemplate mapTemplate;
    public GameObject mapGO;
    private SpawnPoints spawnPoints;

    private AdBillboard[] billBoards;
    public override void Initialize(MonoBehaviour caller, bool isServer, Vector3 spawnPosition, string mapData, UnityAction onFinished = null, WorldCommandSender worldCommandSender = null)
    {
        var workerType = isServer ? "server" : "client";
        caller.StartCoroutine(LoadWorld(caller.transform, layers, workerType, isServer, mapData, onFinished));
    }

    public override void Remove()
    {
        Debug.Log("generated map remove");
        Destroy(mapGO);
    }
    protected virtual IEnumerator LoadWorld(Transform transform, int worldSize, string workertype, bool isServer, string seed, UnityAction onFinished)
    {
        yield return DonnerMapBuilder.GenerateMap(
            mapTemplate,
            worldSize,
            transform,
            workertype, seed, this);
        if (isServer)
        {
            foreach (var childRenderer in mapGO.GetComponentsInChildren<Renderer>())
            {
                childRenderer.enabled = false;
            }
        }
        spawnPoints = mapGO.GetComponentInChildren<SpawnPoints>();
        billBoards = mapGO.transform.GetComponentsInChildren<AdBillboard>();
        yield return null;
        onFinished?.Invoke();
    }
    public override (Vector3 pos, float yaw, float pitch) GetSpawnPoint()
    {
        if (spawnPoints == null)
        {
            return (new Vector3(0, 2, 0), 0, 0);
        }
        var sp = spawnPoints.GetRandomSpawnPoint();
        return (sp.SpawnPosition, sp.SpawnYaw, sp.SpawnYaw);
    }

    public override AdBillboard[] GetBillboards()
    {
        return billBoards;
    }
}
