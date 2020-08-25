using System.Collections;
using Fps.Guns;
using Fps.SchemaExtensions;
using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using UnityEngine;
using Bountyhunt;
using Empty = Improbable.Gdk.Core.Empty;

namespace Fps.Respawning
{
    public class RespawnHandler : MonoBehaviour
    {
        [Require] private HealthComponentCommandReceiver respawnRequests;
        [Require] private HealthComponentWriter health;
        [Require] private ServerMovementWriter serverMovement;
        [Require] private PositionWriter spatialPosition;
        [Require] private GunStateComponentReader gunState;
        [Require] private GunComponentWriter gun;
        [Require] private RoomManagerCommandSender RoomManagerCommandSender;
        private LinkedEntityComponent spatial;

        private void OnEnable()
        {
            respawnRequests.OnRequestRespawnRequestReceived += OnRequestRespawn;
            spatial = GetComponent<LinkedEntityComponent>();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void OnSpawnPositionResponse(RoomManager.GetSpawnPosition.ReceivedResponse res)
        {
            // Reset the player's health.
            var healthUpdate = new HealthComponent.Update
            {
                Health = health.Data.MaxHealth
            };
            health.SendUpdate(healthUpdate);
            // Move to a spawn point (position and rotation)
            var resData = res.ResponsePayload.Value;
            var resVec = Utility.Vector3FloatToVector3(resData.Position);
            var newLatest = new ServerResponse
            {
                Position = resVec.ToVector3Int(),
                IncludesJump = false,
                Timestamp = serverMovement.Data.Latest.Timestamp,
                TimeDelta = 0
            };

            var serverMovementUpdate = new ServerMovement.Update
            {
                Latest = newLatest
            };
            serverMovement.SendUpdate(serverMovementUpdate);

            transform.position = resVec + spatial.Worker.Origin;

            var forceRotationRequest = new RotationUpdate
            {
                Yaw = resData.Yaw.ToInt1k(),
                Pitch = resData.Pitch.ToInt1k(),
                TimeDelta = 0
            };
            serverMovement.SendForcedRotationEvent(forceRotationRequest);

            // Set new Gun
            var newGunId = gunState.Data.NewGunId;
            if (newGunId > GunDictionary.Count - 1 || newGunId < 0)
                newGunId = 0;
            gun.SendUpdate(new GunComponent.Update() { GunId = newGunId });

            // Trigger the respawn event.
            health.SendRespawnEvent(new Empty());
            switch (newGunId)
            {
                case (0):
                    PrometheusManager.TotalSoldiersChosen.Inc();
                    break;
                case (1):
                    PrometheusManager.TotalSnipersChosen.Inc();
                    break;
                case (2):
                    PrometheusManager.TotalScoutsChosen.Inc();
                    break;
                default:
                    break;
            }

            // Update spatial position in the next frame.
            StartCoroutine(SetSpatialPosition(resVec));
        }
        private void OnRequestRespawn(HealthComponent.RequestRespawn.ReceivedRequest request)
        {
            var roomId = GetComponent<RoomPlayerServerBehaviour>().CurrentRoom.EntityId;
            RoomManagerCommandSender.SendGetSpawnPositionCommand(roomId, new Bountyhunt.Empty(), OnSpawnPositionResponse);
            
        }

        private IEnumerator SetSpatialPosition(Vector3 position)
        {
            yield return null;
            var spatialPositionUpdate = new Position.Update
            {
                Coords = Coordinates.FromUnityVector(position)
            };
            spatialPosition.SendUpdate(spatialPositionUpdate);
        }
    }
}
