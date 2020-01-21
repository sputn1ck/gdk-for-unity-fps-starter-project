using System.Collections;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using UnityEngine;
using Improbable.Gdk.Core.Commands;
using Fps.Config;
using Bountyhunt;

namespace Fps
{
    [WorkerType(WorkerUtils.UnityGameLogic)]
    public class BountyPickupServerBehaviour : MonoBehaviour
    {
        [Require] private BountyPickupWriter bountyPickupWriter;

        [Require] private HunterComponentCommandSender hunterComponentCommandSender;

        [Require] private WorldCommandSender commandSender;
        [Require] private EntityId entityId;

        private Coroutine respawnCoroutine;
        private bool hasCollided;
        private void Awake()
        {
            hasCollided = false;
        }
        private void OnEnable()
        {
            // If the pickup is inactive on initial checkout - turn off collisions and start the respawning process.
            if (!bountyPickupWriter.Data.IsActive)
            {
                //respawnCoroutine = StartCoroutine(RespawnHealthPackRoutine());
            }
            //PrometheusManager.ActiveSats.Inc(bountyPickupWriter.Data.BountyValue);
            //PrometheusManager.ActiveCubes.Inc(1);


        }

        private void OnDisable()
        {
            if (respawnCoroutine != null)
            {
                StopCoroutine(respawnCoroutine);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // OnTriggerEnter is fired regardless of whether the MonoBehaviour is enabled/disabled.
            if (bountyPickupWriter == null)
            {
                return;
            }

            if (!other.CompareTag("Player"))
            {
                return;
            }

            HandleCollisionWithPlayer(other.gameObject);
        }

        private void SetIsActive(bool isActive)
        {
            bountyPickupWriter?.SendUpdate(new BountyPickup.Update
            {
                IsActive = new Option<bool>(isActive)
            });
        }

        private void HandleCollisionWithPlayer(GameObject player)
        {
            if (hasCollided)
            {
                return;
            }
            hasCollided = true;
            var playerSpatialOsComponent = player.GetComponent<LinkedEntityComponent>();

            if (playerSpatialOsComponent == null)
            {
                return;
            }
            
            hunterComponentCommandSender.SendAddBountyCommand(playerSpatialOsComponent.EntityId, new AddBountyRequest {
                Reason = BountyReason.PICKUP,
                Amount = bountyPickupWriter.Data.BountyValue,
            });
            //PrometheusManager.ActiveSats.Dec(bountyPickupWriter.Data.BountyValue);
            //PrometheusManager.ActiveCubes.Dec(1);
            // Toggle health pack to its "consumed" state
            SetIsActive(false);
            Invoke("DeleteEntity", 1f);
            
            // Begin cool-down period before re-activating health pack
            //respawnCoroutine = StartCoroutine(RespawnHealthPackRoutine());
        }

        private void DeleteEntity()
        {
            Debug.Log("deleting entity");
            var request = new WorldCommands.DeleteEntity.Request(entityId);
            commandSender.SendDeleteEntityCommand(request);
        }
    }
}
