using Improbable.Gdk.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Bountyhunt;
using Fps;
using Fps.Respawning;
using Fps.SchemaExtensions;
using Improbable;

[UpdateAfter(typeof(BountyTickSystem))][UpdateBefore(typeof(BountyConversionSystem))]
public class ServerGameModeSystem : ComponentSystem
{
    private WorkerSystem workerSystem;
    private CommandSystem commandSystem;
    private ComponentUpdateSystem componentUpdateSystem;

    private EntityQuery playerBountyGroup;
    private EntityQuery playerTeleportGroup;
    private EntityManager entityManager;

    protected override void OnCreate()
    {
        base.OnCreate();

        workerSystem = World.GetExistingSystem<WorkerSystem>();
        commandSystem = World.GetExistingSystem<CommandSystem>();
        componentUpdateSystem = World.GetExistingSystem<ComponentUpdateSystem>();
        entityManager = World.EntityManager;
        playerBountyGroup = GetEntityQuery(
            ComponentType.ReadWrite<HunterComponent.Component>(),
            ComponentType.ReadOnly<SpatialEntityId>()
        );
        playerTeleportGroup = GetEntityQuery(

            ComponentType.ReadWrite<Position.Component>(),
            ComponentType.ReadWrite<ServerMovement.Component>(),
            ComponentType.ReadWrite<HunterComponent.Component>(),
            ComponentType.ReadOnly<SpatialEntityId>()
        );
    }

    protected override void OnUpdate()
    {
        StartGameModeStuff();
        EndGameModeStuff();
    }

    private void EndGameModeStuff()
    {
        var events = componentUpdateSystem.GetEventsReceived<GameModeManager.EndRound.Event>();
        if (events.Count < 1)
            return;
        var gameMode = GameModeDictionary.Get(events[0].Event.Payload.GameModeInfo.GameModeId);
        Entities.With(playerBountyGroup).ForEach((Entity entity) =>
        {
            entityManager.AddComponent(entity, new ResetRoundEarnings().GetType());
            if (gameMode.PlayerSettings.ClearBountyOnEnd)
            {
                var tickComponent = new TickComponent()
                {
                    TickAmount = 1
                };
                entityManager.AddComponentData(entity, tickComponent);
            }
        });
    }

    private void StartGameModeStuff()
    {
        var events = componentUpdateSystem.GetEventsReceived<GameModeManager.NewRound.Event>();
        if (events.Count < 1)
            return;
        if (GameModeDictionary.Get(events[0].Event.Payload.GameModeInfo.GameModeId).PlayerSettings.TeleportPlayerOnStart)
        {
            TeleportPlayers();
        }



    }

    void TeleportPlayers()
    {
        Entities.With(playerTeleportGroup).ForEach((
            ref HunterComponent.Component hunter,
            ref SpatialEntityId entityId,
            ref Position.Component pos,
            ref ServerMovement.Component serverMovement) =>
        {
            var randomPos = getRandomPosition();


            var newLatest = new ServerResponse
            {
                Position = randomPos.ToVector3Int(),
                IncludesJump = false,
                Timestamp = serverMovement.Latest.Timestamp,
                TimeDelta = 0
            };

            var serverMovementUpdate = new ServerMovement.Update
            {
                Latest = newLatest
            };

            var spatialPositionUpdate = new Position.Update
            {
                Coords = Coordinates.FromUnityVector(randomPos)
            };

            componentUpdateSystem.SendUpdate(serverMovementUpdate, entityId.EntityId);
            componentUpdateSystem.SendUpdate(spatialPositionUpdate,entityId.EntityId);
        });
    }

    Vector3 getRandomPosition()
    {
        var pos = new Vector3(Random.Range(-140, 140), 50, Random.Range(-140, 140));
        pos = SpawnPoints.SnapToGround(pos);
        return pos;
    }

}


[RemoveAtEndOfTick]
public struct ResetRoundEarnings : IComponentData{}

