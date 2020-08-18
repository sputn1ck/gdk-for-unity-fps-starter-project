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
using Improbable.Worker.CInterop;
using Improbable.Gdk.Core.Representation;

public class BBHGameLogicWorkerConnector : DonnerWorkerConnectorBase
{

    [SerializeField] private EntityRepresentationMapping entityRepresentationMapping;

    public bool DisableRenderers = true;

    public async void Start()
    {
        Application.targetFrameRate = 30;
        await AttemptConnect();
    }

    protected override IConnectionHandlerBuilder GetConnectionHandlerBuilder()
    {
        IConnectionFlow connectionFlow;
        ConnectionParameters connectionParameters;

        var workerId = CreateNewWorkerId(WorkerUtils.UnityGameLogic);

        if (Application.isEditor)
        {
            connectionFlow = new ReceptionistFlow(workerId);
            connectionParameters = CreateConnectionParameters(WorkerUtils.UnityGameLogic);
        }
        else
        {
            connectionFlow = new ReceptionistFlow(workerId, new CommandLineConnectionFlowInitializer());
            connectionParameters = CreateConnectionParameters(WorkerUtils.UnityGameLogic,
                new CommandLineConnectionParameterInitializer());
        }

        return new SpatialOSConnectionHandlerBuilder()
            .SetConnectionFlow(connectionFlow)
            .SetConnectionParameters(connectionParameters);
    }

    protected override void HandleWorkerConnectionEstablished()
    {
        var world = Worker.World;

        PlayerLifecycleHelper.AddServerSystems(world);
        GameObjectCreationHelper.EnableStandardGameObjectCreation(world, entityRepresentationMapping);

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

        world.GetOrCreateSystem<BountyTickSystem>();
        world.GetOrCreateSystem<BountyConversionSystem>();
        world.GetOrCreateSystem<BountyKillSystem>();
       //world.GetOrCreateSystem<ServerScoreboardSystem>();
        world.GetOrCreateSystem<ServerDisconnectSystem>();
        world.GetOrCreateSystem<ServerGameModeSystem>();

        base.HandleWorkerConnectionEstablished();
    }

    protected override IEnumerator LoadWorld()
    {
        yield return null;
        /*
        yield return base.LoadWorld();

        if (DisableRenderers)
        {
            foreach (var childRenderer in LevelInstance.GetComponentsInChildren<Renderer>())
            {
                childRenderer.enabled = false;
            }
        }
        */
    }
}
