using Improbable.Gdk.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Bountyhunt;

[UpdateInGroup(typeof(SpatialOSUpdateGroup))]
public class BountyConversionSystem : ComponentSystem
{

    private WorkerSystem workerSystem;
    private CommandSystem commandSystem;
    private ComponentUpdateSystem componentUpdateSystem;

    private EntityQuery conversionGroup;
    private EntityQuery gameStatsGroup;

    private float timeSum;
    protected override void OnCreate()
    {
        base.OnCreate();

        workerSystem = World.GetExistingSystem<WorkerSystem>();
        commandSystem = World.GetExistingSystem<CommandSystem>();
        componentUpdateSystem = World.GetExistingSystem<ComponentUpdateSystem>();

        conversionGroup = GetEntityQuery(
                ComponentType.ReadWrite<HunterComponent.Component>(),
                ComponentType.ReadOnly<SpatialEntityId>()
            );
        gameStatsGroup = GetEntityQuery(
                ComponentType.ReadWrite<GameStats.Component>(),
                ComponentType.ReadOnly<SpatialEntityId>());

        timeSum = 0;
    }
    protected override void OnUpdate()
    {
        timeSum += Time.deltaTime;
        if (timeSum < 5)
            return;
        timeSum -= 5;
        BountyConversion();
    }

    private void BountyConversion()
    {
        if (conversionGroup.IsEmptyIgnoreFilter)
        {
            return;
        }
        var percentage = FlagManager.instance.defaultBountyPerTick;
        Entities.With(gameStatsGroup).ForEach((ref GameStats.Component gamestats) =>
        {
            Dictionary<EntityId, PlayerItem> newPairs = new Dictionary<EntityId, PlayerItem>();
            var newMap = gamestats.PlayerMap;
            Entities.With(conversionGroup).ForEach(
            (ref SpatialEntityId entityId,
            ref HunterComponent.Component hunterComponent) =>
            {
                if (hunterComponent.Bounty == 0)
                    return;
                var tick = calculateTick(hunterComponent.Bounty, percentage);
                var newBounty = hunterComponent.Bounty - tick;
                hunterComponent.Bounty = newBounty;
                hunterComponent.Earnings = hunterComponent.Earnings + tick;
                hunterComponent.SessionEarnings = hunterComponent.Earnings + tick;
                ServerServiceConnections.instance.BackendGameServerClient.AddEarnings(hunterComponent.Pubkey, tick);
                newPairs.Add(entityId.EntityId, new PlayerItem() { Bounty = newBounty });
            });
            foreach(var player in newPairs)
            {
                if (newMap.ContainsKey(player.Key))
                {
                    var newPlayer = newMap[player.Key];
                    newPlayer.Bounty = player.Value.Bounty;
                    newMap[player.Key] = newPlayer;
                }
            }
            componentUpdateSystem.SendUpdate(new GameStats.Update() { PlayerMap = newMap }, new EntityId(2));
        });

        
        

    }


    private long calculateTick(long bounty, double percentage)
    {
        long sats = (long)System.Math.Round(bounty * percentage);
        return sats < 1 ? 1 : sats;
    }
}
