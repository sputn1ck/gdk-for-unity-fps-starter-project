using Improbable.Gdk.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Bountyhunt;
using Fps;

[UpdateAfter(typeof(BountyKillSystem))]
public class ServerScoreboardSystem : ComponentSystem
{

    private WorkerSystem workerSystem;
    private CommandSystem commandSystem;
    private ComponentUpdateSystem componentUpdateSystem;

    private EntityQuery hunterGroup;
    private EntityQuery statsGroup;

    protected override void OnCreate()
    {
        base.OnCreate();

        workerSystem = World.GetExistingSystem<WorkerSystem>();
        commandSystem = World.GetExistingSystem<CommandSystem>();
        componentUpdateSystem = World.GetExistingSystem<ComponentUpdateSystem>();

        hunterGroup = GetEntityQuery(
                ComponentType.ReadWrite<HunterComponent.Component>(),
                ComponentType.ReadOnly<SpatialEntityId>(),
                ComponentType.ReadOnly<GunComponent.Component>()
            );
        statsGroup = GetEntityQuery(ComponentType.ReadWrite<GameStats.Component>(),
            ComponentType.ReadOnly<SpatialEntityId>());


    }

    protected override void OnUpdate()
    {

        Entities.With(statsGroup).ForEach((ref GameStats.Component gamestats) =>
        {
            var updates = componentUpdateSystem.GetComponentUpdatesReceived<HunterComponent.Update>();
            if (updates.Count == 0)
                return;
            
            Dictionary<EntityId, PlayerItem> newPairs = new Dictionary<EntityId, PlayerItem>();
            for (int i = 0; i < updates.Count; i++)
            {
                if(!newPairs.ContainsKey(updates[i].EntityId))
                    newPairs.Add(updates[i].EntityId, new PlayerItem());
            }
            var newMap = gamestats.PlayerMap;
            long activeBounty = 0;
            var activeClasses = new int[]
            {
                0,0,0
            };
            Entities.With(hunterGroup).ForEach((ref SpatialEntityId entityId, ref HunterComponent.Component hunter,
                ref GunComponent.Component gun) =>
            {
                if (newPairs.ContainsKey(entityId.EntityId))
                {
                    newPairs[entityId.EntityId] = new PlayerItem() { Bounty = hunter.Bounty, Deaths = hunter.Deaths, Kills = hunter.Kills,Name = hunter.Name, Pubkey = hunter.Pubkey};
                }
                activeBounty += hunter.Bounty;
                activeClasses[gun.GunId]++;
            });
            foreach(var player in newPairs)
            {
                if (newMap.ContainsKey(player.Key))
                {
                    var newPlayer = newMap[player.Key];
                    newPlayer.Bounty = player.Value.Bounty;
                    newPlayer.Kills = player.Value.Kills;
                    newPlayer.Deaths = player.Value.Deaths;
                    newPlayer.Name = player.Value.Name;
                    newPlayer.Pubkey = player.Value.Pubkey;
                    newMap[player.Key] = newPlayer;
                }
            }

            gamestats.PlayerMap = newMap;
            gamestats.BountyOnPlayers = activeBounty;
            PrometheusManager.ActivePlayers.Set(newMap.Count);
            PrometheusManager.ActiveBounty.Set(activeBounty);
            PrometheusManager.ActiveSoldiers.Set(activeClasses[0]);
            PrometheusManager.ActiveSnipers.Set(activeClasses[1]);
            PrometheusManager.ActiveScouts.Set(activeClasses[2]);


        });
    }


}
