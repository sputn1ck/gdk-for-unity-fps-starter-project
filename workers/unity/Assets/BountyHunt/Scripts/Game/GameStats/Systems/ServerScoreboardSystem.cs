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
        statsGroup = GetEntityQuery(ComponentType.ReadWrite<GameStats.Component>());


    }

    protected override void OnUpdate()
    {
        var events = componentUpdateSystem.GetEventsReceived<GameStats.UpdateScoreboardEvent.Event>();
        if (events.Count == 0)
            return;
        if (hunterGroup.IsEmptyIgnoreFilter)
        {
            return;
        }
        List<ScoreboardItem> scoreboardItems = new List<ScoreboardItem>();
        double activeBounty = 0;
        int[] activeClasses = new int[]
        {
            0,0,0
        };
        Entities.With(hunterGroup).ForEach((ref SpatialEntityId entityId, ref HunterComponent.Component hunter, ref GunComponent.Component gun) =>
        {
            var item = new ScoreboardItem(entityId.EntityId, hunter.Bounty, hunter.Kills, hunter.Deaths);
            activeBounty += hunter.Bounty;
            activeClasses[gun.GunId] += 1;

            scoreboardItems.Add(item);
        });
        PrometheusManager.ActivePlayers.Set(scoreboardItems.Count);
        PrometheusManager.ActiveBounty.Set(activeBounty);
        PrometheusManager.ActiveSoldiers.Set(activeClasses[0]);
        PrometheusManager.ActiveSnipers.Set(activeClasses[1]);
        PrometheusManager.ActiveScouts.Set(activeClasses[2]);

        

    }


}
