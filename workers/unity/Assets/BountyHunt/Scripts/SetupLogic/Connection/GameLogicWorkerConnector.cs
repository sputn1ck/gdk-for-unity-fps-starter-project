using System.Collections;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.PlayerLifecycle;
using UnityEngine;
using Fps;
using Fps.Metrics;
using Fps.Health;
using Fps.Guns;
using Fps.WorkerConnectors;
using Fps.Config;

public class GameLogicWorkerConnector : WorkerConnectorBase
{
    public bool DisableRenderers = true;

    protected override async void Start()
    {
        base.Start();
        await AttemptConnect();
    }

    protected override IConnectionHandlerBuilder GetConnectionHandlerBuilder()
    {
        var builder = new SpatialOSConnectionHandlerBuilder()
            .SetConnectionParameters(CreateConnectionParameters(WorkerUtils.UnityGameLogic));

        if (Application.isEditor)
        {
            builder.SetConnectionFlow(new ReceptionistFlow(CreateNewWorkerId(WorkerUtils.UnityGameLogic)));
        }
        else
        {
            builder.SetConnectionFlow(new ReceptionistFlow(CreateNewWorkerId(WorkerUtils.UnityGameLogic),
                new CommandLineConnectionFlowInitializer()));
        }

        return builder;
    }

    protected override void HandleWorkerConnectionEstablished()
    {
        var world = Worker.World;

        PlayerLifecycleHelper.AddServerSystems(world);
        GameObjectCreationHelper.EnableStandardGameObjectCreation(world);

        // Shooting
        world.GetOrCreateSystem<ServerShootingSystem>();


        // Metrics
        world.GetOrCreateSystem<MetricSendSystem>();

        // Health
        world.GetOrCreateSystem<ServerHealthModifierSystem>();
        world.GetOrCreateSystem<HealthRegenSystem>();
        // Donner
        //world.GetOrCreateSystem<ServerBountyKillSystem>();
        //world.GetOrCreateSystem<ServerBountyBoardSystem>();
        //world.GetOrCreateSystem<ServerBountyCleanupSystem>();
        //world.GetOrCreateSystem<ServerPlayerDisconnectSystem>();

        base.HandleWorkerConnectionEstablished();
    }

    protected override IEnumerator LoadWorld()
    {
        yield return base.LoadWorld();

        if (DisableRenderers)
        {
            foreach (var childRenderer in LevelInstance.GetComponentsInChildren<Renderer>())
            {
                childRenderer.enabled = false;
            }
        }

    }
}
