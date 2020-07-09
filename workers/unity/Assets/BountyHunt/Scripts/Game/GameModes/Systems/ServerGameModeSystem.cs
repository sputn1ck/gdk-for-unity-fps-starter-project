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
using Improbable.Gdk.Core.Commands;

[UpdateBefore(typeof(BountyTickSystem))][UpdateBefore(typeof(BountyConversionSystem))]
public class ServerGameModeSystem : ComponentSystem
{
    private WorkerSystem workerSystem;
    private CommandSystem commandSystem;
    private ComponentUpdateSystem componentUpdateSystem;
    

    private EntityQuery playerBountyGroup;
    private EntityQuery playerTeleportGroup;
    private EntityQuery bountyCubeGroup;
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
        bountyCubeGroup = GetEntityQuery(
            ComponentType.ReadOnly<SpatialEntityId>(),
            ComponentType.ReadOnly<BountyPickup.Component>()
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
        var gameMode = ServerGameModeBehaviour.instance.currentGameMode;
        if (gameMode.GameModeSettings.BaseSettings.ClearPickupsOnEnd)
        {
            ClearPickups();
        }
        if (gameMode.GameModeSettings.BaseSettings.ClearStatsOnEnd)
        {
            ClearStats();
        }
    }

    private void StartGameModeStuff()
    {
        var events = componentUpdateSystem.GetEventsReceived<GameModeManager.NewRound.Event>();
        if (events.Count < 1)
            return;
        if (GameModeDictionary.Get(events[0].Event.Payload.GameModeInfo.GameModeId).GameModeSettings.BaseSettings.TeleportPlayerOnStart)
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


            commandSystem.SendCommand(new HunterComponent.TeleportPlayer.Request()
            {
                TargetEntityId = entityId.EntityId,
                Payload = new TeleportRequest(randomPos.x, randomPos.y, randomPos.z, false)
            });
            commandSystem.SendCommand(new HealthComponent.ModifyHealth.Request()
            {
                TargetEntityId = entityId.EntityId,
                Payload = new HealthModifier()
                {
                    Amount = 100
                }
            });
        });
    }

    void ClearStats()
    {
        Entities.With(playerBountyGroup).ForEach((
            ref HunterComponent.Component hunter,
            ref SpatialEntityId entityId) =>
        {
            componentUpdateSystem.SendUpdate(new HunterComponent.Update() {
                SessionEarnings = 0,
                Kills = 0,
                Deaths = 0,
            }, entityId.EntityId);
        });
    }
    void ClearPickups()
    {
        long carryOverSats = 0;
        Entities.With(bountyCubeGroup).ForEach((
            ref SpatialEntityId entityId,
            ref BountyPickup.Component bountyPickup) =>
        {
            commandSystem.SendCommand(new WorldCommands.DeleteEntity.Request{
                EntityId = entityId.EntityId
            });
            carryOverSats += bountyPickup.BountyValue;
        });

        componentUpdateSystem.SendUpdate(new GameStats.Update {
            BountyInCubes = 0,
            CarryoverSats = carryOverSats
        }, new EntityId(2));
    }

    Vector3 getRandomPosition()
    {
        var pos = new Vector3(Random.Range(-140, 140), 50, Random.Range(-140, 140));
        pos = SpawnPoints.SnapToGround(pos);
        return pos;
    }

}


