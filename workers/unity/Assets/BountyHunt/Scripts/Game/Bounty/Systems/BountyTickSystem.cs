
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
            ComponentType.ReadOnly<RoomPlayer.Component>(),
            ComponentType.ReadOnly<SpatialEntityId>(),
            ComponentType.ReadWrite<BountyTickComponent.Component>()
        );

        timeSum = 0;
        tickTime = 1;
    }


    //TODO für rooms überarbeiten
    protected override void OnUpdate()
    {
        if (tickGroup.IsEmptyIgnoreFilter)
            return;
        Entities.With(tickGroup).ForEach((Entity entity, ref RoomPlayer.Component RoomPlayer, ref BountyTickComponent.Component bountyTick) =>
        {
            if (bountyTick.IsActive)
                return;
            bountyTick.CurrentTickTime -= Time.DeltaTime;
            if(bountyTick.CurrentTickTime <= 0)
            {
                bountyTick.CurrentTickTime = bountyTick.TickInterval;
                commandSystem.SendCommand<RoomGameModeManager.BountyTick.Request>(new RoomGameModeManager.BountyTick.Request
                {
                    TargetEntityId = RoomPlayer.RoomEntityid,
                    Payload = new BountyTickRequest(RoomPlayer.Pubkey)
                });
            }
        });
        /*
        if (ServerGameModeBehaviour.instance == null)
            return;
        var endEvents = componentUpdateSystem.GetEventsReceived<GameModeManager.EndRound.Event>();
        var startEvents = componentUpdateSystem.GetEventsReceived<GameModeManager.NewRound.Event>();
        //var gameMode = ServerGameModeBehaviour.instance.currentGameMode;
     

        if(gameMode == null || gameMode.GameModeSettings == null)
        {
            return;
        }
        if (gameMode.GameModeSettings.BountySettings.BountyTickConversion == 0) {
            return;
        }
        if (endEvents.Count > 0 && gameMode.GameModeSettings.BaseSettings.ClearBountyOnEnd)
        {
            AddTicks(1);
            return;
        }
        if (startEvents.Count > 0)
        {
            timeSum = 0;
        }
        timeSum += UnityEngine.Time.deltaTime;
        tickTime = gameMode.GameModeSettings.BountySettings.BountyTickTimeSeconds;
        if (timeSum < tickTime)
            return;
        timeSum = 0;
        AddTicks(gameMode.GameModeSettings.BountySettings.BountyTickConversion);
        */
    }

    
    private void AddTicks(double tickamount)
    {
        if (tickGroup.IsEmptyIgnoreFilter)
                return;
        Entities.With(tickGroup).ForEach((Entity entity, ref HunterComponent.Component hunterComponent) =>
        {
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

