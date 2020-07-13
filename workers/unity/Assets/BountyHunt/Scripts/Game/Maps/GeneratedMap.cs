using Fps.WorldTiles;
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

    public override void Initialize(MonoBehaviour caller, bool isServer, Vector3 spawnPosition, string mapData, UnityAction onFinished = null)
    {
        var workerType = isServer ? "server" : "client";
        caller.StartCoroutine(LoadWorld(caller.transform, layers, workerType, isServer, mapData, onFinished));
    }

    public override void Remove()
    {
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
        yield return null;
        onFinished?.Invoke();
    }
}
