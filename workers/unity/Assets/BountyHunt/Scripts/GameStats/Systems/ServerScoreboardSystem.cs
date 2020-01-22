using Improbable.Gdk.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Bountyhunt;

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
                ComponentType.ReadOnly<SpatialEntityId>()
            );
        statsGroup = GetEntityQuery(ComponentType.ReadWrite<GameStats.Component>());


    }

    protected override void OnUpdate()
    {
        var events = componentUpdateSystem.GetEventsReceived<GameStats.UpdateScoreboardEvent.Event>();
        if (events.Count == 0)
            return;
        Debug.Log("events=" + events.Count);
        if (hunterGroup.IsEmptyIgnoreFilter)
        {
            return;
        }
        List<ScoreboardItem> scoreboardItems = new List<ScoreboardItem>();
        Entities.With(hunterGroup).ForEach((ref SpatialEntityId entityId, ref HunterComponent.Component hunter) =>
        {
            var item = new ScoreboardItem(entityId.EntityId, hunter.Bounty, hunter.Kills, hunter.Deaths);
            scoreboardItems.Add(item);
        });
        componentUpdateSystem.SendUpdate(new GameStats.Update() { Scoreboard = new Scoreboard(scoreboardItems) }, new EntityId(2));

    }


}
