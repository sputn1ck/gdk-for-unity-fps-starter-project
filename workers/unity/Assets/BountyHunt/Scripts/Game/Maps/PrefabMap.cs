using Fps.Respawning;
using Improbable.Gdk.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "BBH/Maps/PrefabMap", order = 1)]
public class PrefabMap : Map
{
    public GameObject MapPrefab;

    private GameObject MapGo;
    private SpawnPoints spawnPoints;
    public override void Initialize(MonoBehaviour caller, bool isServer, Vector3 spawnPosition, string mapData, UnityAction onFinished = null, WorldCommandSender worldCommandSender = null )
    {
        MapGo = Instantiate(MapPrefab, spawnPosition, Quaternion.identity);
        foreach (var convertToEntity in MapGo.GetComponentsInChildren<IConvertToEntity>())
        {
            convertToEntity.Convert(worldCommandSender, this);
        }
        if (isServer)
        {
            
            foreach (var childRenderer in MapGo.GetComponentsInChildren<Renderer>())
            {
                childRenderer.enabled = false;
            }
        }
        onFinished?.Invoke();
        spawnPoints = MapGo.GetComponentInChildren<SpawnPoints>();
    }

    public override void Remove()
    {
        Destroy(MapGo);
    }

    public override Vector3 GetSpawnPoint()
    {
        if(spawnPoints == null)
        {
            
            return new Vector3(0,2,0);
        }
        return spawnPoints.GetRandomSpawnPoint().SpawnPosition;
    }
}
