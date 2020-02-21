
    using Bountyhunt;
    using Fps;
    using Improbable.Gdk.Core;
    using Unity.Entities;
    using UnityEngine;

    [UpdateBefore(typeof(BountyConversionSystem))]
    public class BountyTickSystem : ComponentSystem
    {

        private WorkerSystem workerSystem;
        private CommandSystem commandSystem;
        private ComponentUpdateSystem componentUpdateSystem;
        private EntityManager entityManager;

        private EntityQuery tickGroup;
        private EntityQuery gameStatsGroup;

        private float timeSum;
        private float tickTime;
        protected override void OnCreate()
        {
            base.OnCreate();

            workerSystem = World.GetExistingSystem<WorkerSystem>();
            commandSystem = World.GetExistingSystem<CommandSystem>();
            componentUpdateSystem = World.GetExistingSystem<ComponentUpdateSystem>();
            entityManager = World.EntityManager;
            tickGroup = GetEntityQuery(
                ComponentType.ReadWrite<HunterComponent.Component>(),
                ComponentType.ReadOnly<SpatialEntityId>(),
                ComponentType.ReadOnly<GunComponent.Component>()
            );
            gameStatsGroup = GetEntityQuery(
                ComponentType.ReadWrite<GameModeManager.Component>(),
                ComponentType.ReadOnly<SpatialEntityId>());

            timeSum = 0;
            tickTime = 1;
        }


        protected override void OnUpdate()
        {
            timeSum += Time.deltaTime;
            if (timeSum < tickTime)
                return;
            timeSum -= tickTime;
            Entities.With(gameStatsGroup).ForEach((ref GameModeManager.Component gameModeComponent) =>
            {
                var gameMode = GameModeDictionary.Get(gameModeComponent.CurrentRound.GameModeInfo.GameModeId);
                tickTime = gameMode.PlayerSettings.bountyTickTime;
                if (gameMode.PlayerSettings.BountyTickConversion == 0)
                    return;
                Entities.With(tickGroup).ForEach((Entity entity, ref HunterComponent.Component hunterComponent) =>
                {
                    Debug.Log("adding bounty tick");
                    var tickComponent = new TickComponent()
                    {
                        TickAmount = gameMode.PlayerSettings.BountyTickConversion
                    };
                    entityManager.AddComponent(entity, tickComponent.GetType());
                    entityManager.SetComponentData(entity, tickComponent);
                });
            });
        }
    }

    [RemoveAtEndOfTick]
    public struct TickComponent : IComponentData
    {
        public double TickAmount;
    }

