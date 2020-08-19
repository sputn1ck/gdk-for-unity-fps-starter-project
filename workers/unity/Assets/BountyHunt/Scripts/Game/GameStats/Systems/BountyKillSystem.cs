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
            // todo modify bounty (maybe the room should do that)
            commandSystem.SendCommand(new RoomStatsManager.AddKill.Request(roomPlayerKiller.RoomEntityid, new AddKillRequest(roomPlayerKiller.Pubkey, roomPlayerVictim.Pubkey)));

            //SendBackendUpdate(roomPlayerKiller.Pubkey, roomPlayerVictim.Pubkey);
        }
    }


    private void SendBackendUpdate(string killer, string victim, string gameModeId)
    {
        var backend = ServerServiceConnections.instance.BackendGameServerClient;
        if (backend != null)
        {
            
            if(gameModeId != "lobby")
            {
                backend.AddKill(killer, victim);
            }
            
        }
    }

}
