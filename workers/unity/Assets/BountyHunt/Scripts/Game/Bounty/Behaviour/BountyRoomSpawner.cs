using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Commands;
using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
public class BountyRoomSpawner : MonoBehaviour
{

    [Require] private WorldCommandSender WorldCommandSender;
    [Require] private RoomPlayerCommandSender RoomPlayerCommandSender;
    private SatsCubeSpawnPoint[] satsCubeSpawnPoints;
     
    public bool SpawnTrigger;
    public long spawnAmount;

    private LinkedEntityComponent LinkedEntityComponent;

    private void OnEnable()
    {
        LinkedEntityComponent = GetComponent<LinkedEntityComponent>();
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
        SpawnCube(satsCubeSpawnPoints[spawnIndex].transform.position, spawnAmount);
    }
    public void SpawnCube(Vector3 position, long satAmount)
    {
        if (satAmount < 1)
            return;
       
        var bountypickup = DonnerEntityTemplates.BountyPickup(position, satAmount);
        WorldCommandSender.SendCreateEntityCommand(new WorldCommands.CreateEntity.Request(bountypickup));
    }

    public void SpawnCubeAtEntity(EntityId entity, long satAmount)
    {
        if (satAmount < 1)
            return;
        RoomPlayerCommandSender.SendGetPositionCommand(entity, new Bountyhunt.Empty(), (cb) =>
        {
            if(cb.StatusCode != Improbable.Worker.CInterop.StatusCode.Success)
            {
                return;
            }
            SpawnCube(cb.ResponsePayload.Value.ToUnityVector(), satAmount);
        });
    }
}
