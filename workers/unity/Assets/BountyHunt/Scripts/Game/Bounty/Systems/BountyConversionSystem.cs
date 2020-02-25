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
                ComponentType.ReadOnly<GunComponent.Component>(),
                ComponentType.ReadOnly<TickComponent>()
            );
        gameStatsGroup = GetEntityQuery(
                ComponentType.ReadWrite<GameStats.Component>(),
                ComponentType.ReadOnly<SpatialEntityId>());

        timeSum = 0;
    }
    protected override void OnUpdate()
    {
        BountyConversion();
    }

    private void BountyConversion()
    {
        if (conversionGroup.IsEmptyIgnoreFilter)
        {
            return;
        }
        Entities.With(gameStatsGroup).ForEach((ref GameStats.Component gamestats) =>
        {

            Entities.With(conversionGroup).ForEach(
            (ref SpatialEntityId entityId,
            ref HunterComponent.Component hunterComponent, ref GunComponent.Component gun, ref TickComponent tickComponent) =>
            {
                if (hunterComponent.Bounty != 0)
                {
                    var tick = calculateTick(hunterComponent.Bounty, tickComponent.TickAmount);
                    Debug.Log("ticking with " + tick + " from component: " + tickComponent.TickAmount);
                    var newBounty = hunterComponent.Bounty - tick;
                    hunterComponent.Bounty = newBounty;
                    hunterComponent.Earnings = hunterComponent.Earnings + tick;
                    hunterComponent.SessionEarnings = hunterComponent.SessionEarnings + tick;
                    ServerServiceConnections.instance.BackendGameServerClient.AddEarnings(hunterComponent.Pubkey, tick);

                }
            });


            
        });




    }


    private long calculateTick(long bounty, double percentage)
    {
        long sats = (long)System.Math.Round(bounty * percentage);
        return sats < 1 ? 1 : sats;
    }
}
