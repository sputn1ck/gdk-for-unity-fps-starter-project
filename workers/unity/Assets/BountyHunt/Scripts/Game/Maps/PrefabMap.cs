using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "BBH/Maps/PrefabMap", order = 1)]
public class PrefabMap : Map
{
    public GameObject MapPrefab;
    public override void Initialize(bool isServer, Vector3 spawnPosition)
    {
        var mapGO = Instantiate(MapPrefab, spawnPosition, Quaternion.identity);
        if (isServer)
        {
            foreach (var childRenderer in mapGO.GetComponentsInChildren<Renderer>())
            {
                childRenderer.enabled = false;
            }
        }
    }
}
