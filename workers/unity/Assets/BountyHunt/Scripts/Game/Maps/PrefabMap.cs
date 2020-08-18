using Fps.Respawning;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
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
    private AdBillboard[] billBoards;

    public override void Initialize(MonoBehaviour caller, bool isServer, Vector3 spawnPosition, string mapData, UnityAction onFinished = null, WorldCommandSender worldCommandSender = null )
    {
       
        MapGo = Instantiate(MapPrefab, spawnPosition, Quaternion.identity);
        var origin = caller.GetComponent<LinkedEntityComponent>().Worker.Origin;
        foreach (var convertToEntity in MapGo.GetComponentsInChildren<IConvertToEntity>())
        {
            convertToEntity.Convert(worldCommandSender, this, origin);
        }
        if (isServer)
        {
            
            foreach (var childRenderer in MapGo.GetComponentsInChildren<Renderer>())
            {
                childRenderer.enabled = false;
            }
            foreach(var serverRemover in MapGo.GetComponentsInChildren<IServerRemover>())
            {
                Destroy(serverRemover.GetMonoBehaviour());
            }
        } else
        {
            foreach (var clientScript in MapGo.GetComponentsInChildren<IClientInitializer>())
            {
                clientScript.Initialize();
            }
        }
        spawnPoints = MapGo.GetComponentInChildren<SpawnPoints>();
        if (spawnPoints != null)
        {
            spawnPoints.SetSpawnPoints();
        }
        billBoards = MapGo.transform.GetComponentsInChildren<AdBillboard>();
        onFinished?.Invoke();
    }

    public override void Remove()
    {

        Debug.Log("prefab map remove");
        Destroy(MapGo);
    }

    public override (Vector3 pos, float yaw, float pitch) GetSpawnPoint()
    {
        if(spawnPoints == null)
        {
            
            return (new Vector3(0,2,0),0,0);
        }
        var sp = spawnPoints.GetRandomSpawnPoint();
        return (sp.SpawnPosition, sp.SpawnYaw, sp.SpawnYaw);
    }

    public override AdBillboard[] GetBillboards()
    {
        return billBoards;
    }
}
