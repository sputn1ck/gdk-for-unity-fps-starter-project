
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
        if (ServerGameModeBehaviour.instance == null)
            return;
        var endEvents = componentUpdateSystem.GetEventsReceived<GameModeManager.EndRound.Event>();
        var startEvents = componentUpdateSystem.GetEventsReceived<GameModeManager.NewRound.Event>();
        var gameMode = ServerGameModeBehaviour.instance.currentGameMode;
        if (endEvents.Count > 0 && gameMode.GameModeSettings.BaseSettings.ClearBountyOnEnd)
        {
            AddTicks(1);
            return;
        }
        if(gameMode.GameModeSettings == null)
        {
            return;
        }
        if (gameMode.GameModeSettings.BountySettings.BountyTickConversion == 0)
            return;
        if(startEvents.Count > 0)
        {
            timeSum = 0;
        }
        timeSum += Time.deltaTime;
        tickTime = gameMode.GameModeSettings.BountySettings.BountyTickTimeSeconds;
        if (timeSum < tickTime)
            return;
        timeSum = 0;
        AddTicks(gameMode.GameModeSettings.BountySettings.BountyTickConversion);
    }


    
    private void AddTicks(double tickamount)
    {
        if (tickGroup.IsEmptyIgnoreFilter)
                return;
        Entities.With(tickGroup).ForEach((Entity entity, ref HunterComponent.Component hunterComponent) =>
        {
            Debug.Log("adding bounty tick");
            var tickComponent = new TickComponent()
            {
                TickAmount = tickamount
            };
            entityManager.AddComponent(entity, tickComponent.GetType());
            entityManager.SetComponentData(entity, tickComponent);
        });
        
        
    }


}

    [RemoveAtEndOfTick]
    public struct TickComponent : IComponentData
    {
        public double TickAmount;
    }

