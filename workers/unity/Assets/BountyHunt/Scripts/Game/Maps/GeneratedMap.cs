using Fps.WorldTiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "BBH/Maps/GeneratedMap", order = 1)]
public class GeneratedMap : Map
{
    public int layers;
    public MapTemplate mapTemplate;
    public GameObject mapGO;
    public override GameObject Initialize(MonoBehaviour caller, bool isServer, Vector3 spawnPosition, string mapData)
    {
        var workerType = isServer ? "server" : "client";
        caller.StartCoroutine(LoadWorld(caller.transform, layers, workerType, isServer, mapData));
        return mapGO;

    }
    protected virtual IEnumerator LoadWorld(Transform transform, int worldSize, string workertype, bool isServer, string seed)
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
    }
}
