using Improbable.Gdk.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Bountyhunt;
using Fps;

[UpdateAfter(typeof(SpatialOSUpdateGroup))]
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
                ComponentType.ReadOnly<SpatialEntityId>(),
                ComponentType.ReadOnly<GunComponent.Component>()
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
        TimedBountyConversion();
    }

    private void TimedBountyConversion()
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

            double activeBounty = 0;
            int[] activeClasses = new int[]
            {
            0,0,0
            };
            Entities.With(conversionGroup).ForEach(
            (ref SpatialEntityId entityId,
            ref HunterComponent.Component hunterComponent, ref GunComponent.Component gun) =>
            {
                if (hunterComponent.Bounty == 0)
                    return;
                var tick = calculateTick(hunterComponent.Bounty, percentage);
                var newBounty = hunterComponent.Bounty - tick;
                hunterComponent.Bounty = newBounty;
                hunterComponent.Earnings = hunterComponent.Earnings + tick;
                hunterComponent.SessionEarnings = hunterComponent.SessionEarnings + tick;
                ServerServiceConnections.instance.BackendGameServerClient.AddEarnings(hunterComponent.Pubkey, tick);
                newPairs.Add(entityId.EntityId, new PlayerItem() { Bounty = newBounty });

                activeBounty += hunterComponent.Bounty;
                activeClasses[gun.GunId] += 1;
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

            PrometheusManager.ActivePlayers.Set(newMap.Count);
            PrometheusManager.ActiveBounty.Set(activeBounty);
            PrometheusManager.ActiveSoldiers.Set(activeClasses[0]);
            PrometheusManager.ActiveSnipers.Set(activeClasses[1]);
            PrometheusManager.ActiveScouts.Set(activeClasses[2]);
        });




    }


    private long calculateTick(long bounty, double percentage)
    {
        long sats = (long)System.Math.Round(bounty * percentage);
        return sats < 1 ? 1 : sats;
    }
}
