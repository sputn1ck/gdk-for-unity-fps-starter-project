using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "BBH/Maps/PrefabMap", order = 1)]
public class PrefabMap : Map
{
    public GameObject MapPrefab;

    private GameObject MapGo;
    public override void Initialize(MonoBehaviour caller, bool isServer, Vector3 spawnPosition, string mapData, UnityAction onFinished = null)
    {
        MapGo = Instantiate(MapPrefab, spawnPosition, Quaternion.identity);
        if (isServer)
        {
            foreach (var childRenderer in MapGo.GetComponentsInChildren<Renderer>())
            {
                childRenderer.enabled = false;
            }
        }
        onFinished?.Invoke();
    }

    public override void Remove()
    {
        Destroy(MapGo);
    }
}
