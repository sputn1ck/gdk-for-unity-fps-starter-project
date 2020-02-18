using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Improbable.Gdk.Core;
using Improbable;
using Fps.Health;
using Bountyhunt;

[UpdateAfter(typeof(SpatialOSUpdateGroup))]
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
        //SendBountyBoardUpdate();
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
            var satsToDrop = (long)(victimDonnerInfo.Bounty - (victimDonnerInfo.Bounty * FlagManager.instance.GetSatoshiDropRate()));
            var victimModifiedInfo = new HunterComponent.Update()
            {
                Bounty = satsToDrop,
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
            if(victimDonnerInfo.Bounty > 0)
                commandSystem.SendCommand(new BountySpawner.SpawnBountyPickup.Request { TargetEntityId = new EntityId(2), Payload = new SpawnBountyPickupRequest { BountyValue = satsToDrop, Position = pos } });
            
            SendBackendUpdate(killerDonnerInfo.Pubkey, victimDonnerInfo.Pubkey);
            SetScoreboard(killerId, victimId);
            PrometheusManager.TotalKills.Inc(1);
        }
    }
    private void SetScoreboard(EntityId killerId, EntityId victimId)
    {
        var gameStats = componentUpdateSystem.GetComponent<GameStats.Snapshot>(new EntityId(2));
        var playerMap = gameStats.PlayerMap;
        if (playerMap.ContainsKey(killerId) && playerMap.ContainsKey(victimId))
        {
            var killer = playerMap[killerId];
            killer.Kills++;
            var victim = playerMap[victimId];
            victim.Deaths++;
            playerMap[killerId] = killer;
            playerMap[victimId] = victim;
            componentUpdateSystem.SendUpdate(new GameStats.Update() { PlayerMap = playerMap }, new EntityId(2));
        }

    }
    private void SendBountyBoardUpdate()
    {
        componentUpdateSystem.SendEvent(new GameStats.UpdateScoreboardEvent.Event(new Bountyhunt.Empty()), new EntityId(2));

    }

    private void SendBackendUpdate(string killer, string victim)
    {
        var backend = ServerServiceConnections.instance.BackendGameServerClient;
        if (backend != null)
        {
            backend.AddKill(killer, victim);
        }
    }

}
