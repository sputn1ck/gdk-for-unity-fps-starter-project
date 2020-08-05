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
        //HandleKills(events);
        //SendBountyBoardUpdate();
        HandleRoomKills(events);
    }

    private void HandleRoomKills(MessagesSpan<ComponentEventReceived<GameStats.GainedKillEvent.Event>> events)
    {
        var roomPlayers = GetComponentDataFromEntity<RoomPlayer.Component>();
        for(int i =0; i < events.Count; i++)
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

            var roomPlayerKiller = roomPlayers[killer];
            var roomPlayerVictim = roomPlayers[victim];
            commandSystem.SendCommand(new RoomStats.AddKill.Request(roomPlayerKiller.RoomEntityid, new AddKillRequest(roomPlayerKiller.Pubkey, roomPlayerVictim.Pubkey)));

            //SendBackendUpdate(roomPlayerKiller.Pubkey, roomPlayerVictim.Pubkey);
        }
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
            int added = (int)(victimDonnerInfo.Bounty * ServerGameModeBehaviour.instance.currentGameMode.GameModeSettings.BountySettings.BountyDropPercentageDeath) % 2;
            var satsToDrop = (long)(victimDonnerInfo.Bounty * ServerGameModeBehaviour.instance.currentGameMode.GameModeSettings.BountySettings.BountyDropPercentageDeath);
            var victimModifiedInfo = new HunterComponent.Update()
            {
                Bounty = victimDonnerInfo.Bounty - satsToDrop,
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
            if (satsToDrop > 0)
            {
                commandSystem.SendCommand(new BountySpawner.SpawnBountyPickup.Request { TargetEntityId = new EntityId(2), Payload = new SpawnBountyPickupRequest { BountyValue = satsToDrop + added, Position = pos } });
            }

            //SendBackendUpdate(killerDonnerInfo.Pubkey, victimDonnerInfo.Pubkey);
            
            //PrometheusManager.TotalKills.Inc(1);
        }
    }

    private void SendBackendUpdate(string killer, string victim)
    {
        var backend = ServerServiceConnections.instance.BackendGameServerClient;
        if (backend != null)
        {
            if(ServerGameModeBehaviour.instance.currentGameMode.GameModeId != "lobby")
            {
                backend.AddKill(killer, victim);
            }
            
        }
    }

}
