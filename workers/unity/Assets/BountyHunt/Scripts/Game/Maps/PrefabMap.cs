using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "BBH/Maps/PrefabMap", order = 1)]
public class PrefabMap : Map
{
    public GameObject MapPrefab;
    public override GameObject Initialize(MonoBehaviour caller, bool isServer, Vector3 spawnPosition, string mapData)
    {
        var mapGO = Instantiate(MapPrefab, spawnPosition, Quaternion.identity);
        if (isServer)
        {
            foreach (var childRenderer in mapGO.GetComponentsInChildren<Renderer>())
            {
                childRenderer.enabled = false;
            }
        }
        return mapGO;
    }
}
