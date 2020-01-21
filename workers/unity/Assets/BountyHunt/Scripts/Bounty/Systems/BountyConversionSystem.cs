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

    private float timeSum;
    protected override void OnCreate()
    {
        base.OnCreate();

        workerSystem = World.GetExistingSystem<WorkerSystem>();
        commandSystem = World.GetExistingSystem<CommandSystem>();
        componentUpdateSystem = World.GetExistingSystem<ComponentUpdateSystem>();

        conversionGroup = GetEntityQuery(
                ComponentType.ReadWrite<HunterComponent.Component>(),
                ComponentType.ReadWrite<BountyComponent.Component>(),
                ComponentType.ReadOnly<SpatialEntityId>()
            );

        timeSum = 0;
    }
    protected override void OnUpdate()
    {
        timeSum += Time.deltaTime;
        if (timeSum < 5)
            return;
        timeSum -= 5;
        BountyConversion();
        SendBountyBoardUpdate();
    }

    private void BountyConversion()
    {
        if (conversionGroup.IsEmptyIgnoreFilter)
        {
            return;
        }
        var percentage = FlagManager.instance.defaultBountyPerTick;
        Entities.With(conversionGroup).ForEach(
            (ref SpatialEntityId entityId,
            ref BountyComponent.Component bountyComponent,
            ref HunterComponent.Component hunterComponent) =>
            {
                if (bountyComponent.Bounty == 0)
                    return;
                var tick = calculateTick(bountyComponent.Bounty, percentage);
                bountyComponent.Bounty = bountyComponent.Bounty - tick;
                hunterComponent.Earnings = hunterComponent.Earnings + tick;
            });

    }

    private void SendBountyBoardUpdate()
    {
        //TODO implement me

    }

    private long calculateTick(long bounty, double percentage)
    {
        long sats = (long)System.Math.Round(bounty * percentage);
        return sats < 1 ? 1 : sats;
    }
}
