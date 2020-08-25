using UnityEngine;

namespace Fps.Respawning
{
    public class SpawnPoints : MonoBehaviour
    {
        public class SpawnPoint
        {
            public Vector3 SpawnPosition;
            public float SpawnYaw;
            public float SpawnPitch;

            public void Deconstruct(out Vector3 position, out float yaw, out float pitch)
            {
                position = SpawnPosition;
                yaw = SpawnYaw;
                pitch = SpawnPitch;
            }
        }

        private static SpawnPoint[] spawnPointListStatic;
        private static SpawnPoint[] spawnPointList;
        [SerializeField] private bool snapsToGround = true;

        public static SpawnPoint GetRandomSpawnPointStatic()
        {
            if (spawnPointListStatic == null || spawnPointListStatic.Length == 0)
            {
                Debug.LogWarning("No spawn points found - using origin.");
                return new SpawnPoint();
            }

            return spawnPointListStatic[Random.Range(0, spawnPointListStatic.Length)];
        }
        public  SpawnPoint GetRandomSpawnPoint()
        {
            if (spawnPointListStatic == null || spawnPointListStatic.Length == 0)
            {
                Debug.LogWarning("No spawn points found - using origin.");
                return new SpawnPoint();
            }

            return spawnPointListStatic[Random.Range(0, spawnPointListStatic.Length)];
        }
        public void SetSpawnPoints
          ()
        {
            var spawnPoints = FindSpawnPoints();
            var worldOffset = transform.root.position;

            spawnPointListStatic = new SpawnPoint[spawnPoints.Length];
            spawnPointList = new SpawnPoint[spawnPoints.Length];
            for (var n = 0; n < spawnPoints.Length; n++)
            {
                var spawnPointTransform = spawnPoints[n].transform;
                var spawnPointPosition = spawnPointTransform.position;
                if (snapsToGround)
                {
                    spawnPointPosition = SnapToGround(spawnPointPosition);
                }

                spawnPointListStatic[n] = new SpawnPoint
                {
                    SpawnPosition = spawnPointPosition - worldOffset,
                    SpawnYaw = spawnPointTransform.eulerAngles.y,
                    SpawnPitch = 0
                };
                spawnPointList[n] = new SpawnPoint
                {
                    SpawnPosition = spawnPointPosition - worldOffset,
                    SpawnYaw = spawnPointTransform.eulerAngles.y,
                    SpawnPitch = 0
                };

                if (Application.isPlaying)
                {
                    Destroy(spawnPoints[n]);
                }
                else
                {
                    DestroyImmediate(spawnPoints[n]);
                }
            }
        }

        private GameObject[] FindSpawnPoints()
        {
            var spawnPointIndicators = transform.parent.GetComponentsInChildren<SpawnPointIndicator>();

            var gameObjects = new GameObject[spawnPointIndicators.Length];

            for (var i = 0; i < spawnPointIndicators.Length; i++)
            {
                gameObjects[i] = spawnPointIndicators[i].gameObject;
            }

            return gameObjects;
        }

        public static Vector3 SnapToGround(Vector3 position)
        {
            LayerMask allLayerMask = ~0;
            if (Physics.Raycast(new Ray(position, Vector3.down), out var hitInfo, 100f,
                allLayerMask))
            {
                return hitInfo.point;
            }

            return position;
        }
    }
}
