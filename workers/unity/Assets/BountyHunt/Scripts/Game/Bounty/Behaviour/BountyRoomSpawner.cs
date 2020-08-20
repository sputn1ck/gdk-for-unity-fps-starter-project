using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Commands;
using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BountyRoomSpawner : MonoBehaviour
{

    [Require] private WorldCommandSender WorldCommandSender;
    private SatsCubeSpawnPoint[] satsCubeSpawnPoints;
     
    public bool SpawnTrigger;
    public long spawnAmount;

    private LinkedEntityComponent LinkedEntityComponent;

    private void OnEnable()
    {
        LinkedEntityComponent = GetComponent<LinkedEntityComponent>();
        Debug.Log(WorldCommandSender.IsValid);
    }
    public void Setup(SatsCubeSpawnPoint[] satsCubeSpawnPoints)
    {
        this.satsCubeSpawnPoints = satsCubeSpawnPoints;
    }

    void Update()
    {
        if (SpawnTrigger)
        {
            SpawnTrigger = false;
            SpawnDebugCube();
        }
    }

    private void SpawnDebugCube()
    {

        var spawnIndex = Random.Range(0, satsCubeSpawnPoints.Length);
        SpawnCube(satsCubeSpawnPoints[spawnIndex].transform, spawnAmount);
    }
    public void SpawnCube(Transform position, long satAmount)
    {
        if (satAmount < 1)
            return;
        var pos = position.position - LinkedEntityComponent.Worker.Origin;
       
        var bountypickup = DonnerEntityTemplates.BountyPickup(pos, satAmount);
        WorldCommandSender.SendCreateEntityCommand(new WorldCommands.CreateEntity.Request(bountypickup));
    }
}
