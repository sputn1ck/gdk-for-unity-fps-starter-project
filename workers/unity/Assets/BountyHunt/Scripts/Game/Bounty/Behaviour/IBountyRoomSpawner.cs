using Improbable.Gdk.Core;
using UnityEngine;

public interface IBountyRoomSpawner
{
    void Setup(SatsCubeSpawnPoint[] satsCubeSpawnPoints);
    void SpawnCube(Vector3 position, long satAmount);
    void SpawnCubeAtEntity(EntityId entity, long satAmount);
}