using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Improbable.Gdk.Core;
using Improbable;
using Fps.Health;
using Bountyhunt;

[UpdateInGroup(typeof(SpatialOSUpdateGroup))]
[UpdateAfter(typeof(ServerHealthModifierSystem))]
public class BountyKillSystem : ComponentSystem
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
        var events = componentUpdateSystem.GetEventsReceived<GameStats.GainedKillEvent.Event>();
        if (events.Count == 0)
            return;
        HandleKills(events);
        SendBountyBoardUpdate();
    }

    private void HandleKills(MessagesSpan<ComponentEventReceived<GameStats.GainedKillEvent.Event>> events)
    {
        var hunters = GetComponentDataFromEntity<HunterComponent.Component>();
        for (var i = 0; i < events.Count; i++)
        {
            ref readonly var request = ref events[i];
            var killerId = request.Event.Payload.Killer;
            var victimId = request.Event.Payload.Victim;
            if (!workerSystem.TryGetEntity(killerId, out var killer))
            {
                continue;
            }
            if (!workerSystem.TryGetEntity(victimId, out var victim))
            {
                continue;
            }
            var killerDonnerInfo = hunters[killer];
            var victimDonnerInfo = hunters[victim];


            var killerModifiedInfo = new HunterComponent.Update()
            {
                Kills = killerDonnerInfo.Kills + 1
            };
            var victimModifiedInfo = new HunterComponent.Update()
            {
                Bounty = 0,
                Deaths = victimDonnerInfo.Deaths + 1
            };
            var posSpatial = componentUpdateSystem.GetComponent<Position.Snapshot>(victimId);
            componentUpdateSystem.SendUpdate(killerModifiedInfo, killerId);
            componentUpdateSystem.SendUpdate(victimModifiedInfo, victimId);
            var pos = new Vector3Float
            {
                X = (float)posSpatial.Coords.X,
                Y = (float)posSpatial.Coords.Y,
                Z = (float)posSpatial.Coords.Z,
            };
            commandSystem.SendCommand(new BountySpawner.SpawnBountyPickup.Request { TargetEntityId = new EntityId(2), Payload = new SpawnBountyPickupRequest { BountyValue = victimDonnerInfo.Bounty, Position = pos } });
            //PrometheusManager.TotalKills.Inc(1);
        }
    }
    private void SendBountyBoardUpdate()
    {
        componentUpdateSystem.SendEvent(new GameStats.UpdateScoreboardEvent.Event(new Bountyhunt.Empty()), new EntityId(2));

    }

}
