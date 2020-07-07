using System.IO;
using Improbable;
using Improbable.Gdk.Core;
using UnityEditor;
using UnityEngine;
using Fps;

public class DonnerSnapshot : MonoBehaviour
{
    private static readonly string DefaultSnapshotPath =
        Path.Combine(Application.dataPath, "../../../snapshots/default.snapshot");

    private static readonly string CloudSnapshotPath =
        Path.Combine(Application.dataPath, "../../../snapshots/cloud.snapshot");

    private static readonly string SessionSnapshotPath =
        Path.Combine(Application.dataPath, "../../../snapshots/session.snapshot");

    [MenuItem("SpatialOS/Generate Donner Snapshot")]
    private static void GenerateFpsSnapshot()
    {
        SaveSnapshot(DefaultSnapshotPath, GenerateDefaultSnapshot());
        SaveSnapshot(CloudSnapshotPath, GenerateDefaultSnapshot());
    }

    private static Snapshot GenerateDefaultSnapshot()
    {
        var snapshot = new Snapshot();
        snapshot.AddEntity(DonnerEntityTemplates.Spawner(Coordinates.Zero));
        AddGameManager(snapshot);
        AddWorldManager(snapshot);
        //AddBountyPacks(snapshot);
        return snapshot;
    }


    private static void SaveSnapshot(string path, Snapshot snapshot)
    {
        snapshot.WriteToFile(path);
        Debug.LogFormat("Successfully generated initial world snapshot at {0}", path);
    }


    private static void AddBountyPacks(Snapshot snapshot)
    {
        for (int i = 0; i < 25; i++)
        {
            var pos = new Vector3(UnityEngine.Random.Range(-140, 140), 0, UnityEngine.Random.Range(-140, 140));
            var healthPack = DonnerEntityTemplates.BountyPickup(pos, 10);
            snapshot.AddEntity(healthPack);
        }
    }

    private static void AddGameManager(Snapshot snapshot)
    {
        var nodeInfo = DonnerEntityTemplates.GameManager(new Vector3(0, 0, 0));

        snapshot.AddEntity(nodeInfo);
    }
    private static void AddWorldManager(Snapshot snapshot)
    {
        var worldManager = DonnerEntityTemplates.WorldManager(new Vector3(0, 0, 0));

        snapshot.AddEntity(worldManager);
    }
}
