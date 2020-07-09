using Improbable.Gdk.Core;
using Unity.Entities;

namespace Fps.Guns
{
    [UpdateInGroup(typeof(SpatialOSUpdateGroup))]
    public class ServerShootingSystem : ComponentSystem
    {
        private WorkerSystem workerSystem;
        private CommandSystem commandSystem;
        private ComponentUpdateSystem componentUpdateSystem;

        protected override void OnCreate()
        {
            base.OnCreate();

            workerSystem = World.GetExistingSystem<WorkerSystem>();
            commandSystem = World.GetExistingSystem<CommandSystem>();
            componentUpdateSystem = World.GetExistingSystem<ComponentUpdateSystem>();
        }

        protected override void OnUpdate()
        {
            var events = componentUpdateSystem.GetEventsReceived<ShootingComponent.Shots.Event>();
            if (events.Count == 0)
            {
                return;
            }
            /*
            if (ServerGameModeBehaviour.instance.currentGameMode.GameModeId == "lobby")
            {
                return;
            }
            */
            var gunDataForEntity = GetComponentDataFromEntity<GunComponent.Component>();

            for (var i = 0; i < events.Count; ++i)
            {
                ref readonly var shotEvent = ref events[i];
                var shotInfo = shotEvent.Event.Payload;
                if (!shotInfo.HitSomething || !shotInfo.EntityId.IsValid())
                {
                    continue;
                }

                var shooterSpatialID = shotEvent.EntityId;
                if (!workerSystem.TryGetEntity(shooterSpatialID, out var shooterEntity))
                {
                    continue;
                }

                if (!gunDataForEntity.Exists(shooterEntity))
                {
                    continue;
                }

                var gunComponent = gunDataForEntity[shooterEntity];
                var damage = GunDictionary.Get(gunComponent.GunId).ShotDamage * shotInfo.DmgMultiplier;

                var modifyHealthRequest = new HealthComponent.ModifyHealth.Request(
                    shotInfo.EntityId,
                    new HealthModifier
                    {
                        Amount = -damage,
                        Origin = shotInfo.HitOrigin,
                        AppliedLocation = shotInfo.HitLocation,
                        Owner = shotEvent.EntityId,
                    }
                );

                commandSystem.SendCommand(modifyHealthRequest);
            }
        }
    }
}
