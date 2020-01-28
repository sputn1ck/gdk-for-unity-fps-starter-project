using Fps;
using Fps.Config;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Worker.CInterop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ClientWorkerConnectorLnd : DonnerWorkerConnectorBase
{
    protected string deployment;
    protected string loginToken;
    protected string playerIdentityToken;

    private string playerName;
    public int gunId;
    private bool isReadyToSpawn;
    private bool wantsSpawn;
    private Action<PlayerCreator.CreatePlayer.ReceivedResponse> onPlayerResponse;
    private AdvancedEntityPipeline entityPipeline;

    public bool HasConnected => Worker != null;
    protected bool UseSessionFlow => !string.IsNullOrEmpty(deployment);
    protected bool useLocal;
    public event Action OnLostPlayerEntity;

    public async Task Connect( string deployment, string loginToken, string playerIdentityToken, bool useLocal = false)
    {
        this.deployment = deployment.Trim();
        this.loginToken = loginToken;
        this.playerIdentityToken = playerIdentityToken;
        this.useLocal = useLocal;
        await AttemptConnect();
    }

    public void SpawnPlayer(string playerName, int gunId, Action<PlayerCreator.CreatePlayer.ReceivedResponse> onPlayerResponse)
    {
        this.onPlayerResponse = onPlayerResponse;
        this.playerName = playerName;
        this.gunId = gunId;
        wantsSpawn = true;
    }

    public void DisconnectPlayer()
    {
        StartCoroutine(PrepareDestroy());
    }

    protected virtual string GetAuthPlayerPrefabPath()
    {
        return "Prefabs/UnityClient/Authoritative/Player";
    }

    protected virtual string GetNonAuthPlayerPrefabPath()
    {
        return "Prefabs/UnityClient/NonAuthoritative/Player";
    }

    protected override void HandleWorkerConnectionEstablished()
    {
        var world = Worker.World;

        PlayerLifecycleHelper.AddClientSystems(world, autoRequestPlayerCreation: false);
        PlayerLifecycleConfig.MaxPlayerCreationRetries = 0;

        entityPipeline = new AdvancedEntityPipeline(Worker, GetAuthPlayerPrefabPath(), GetNonAuthPlayerPrefabPath());
        entityPipeline.OnRemovedAuthoritativePlayer += RemovingAuthoritativePlayer;

        // Set the Worker gameObject to the ClientWorker so it can access PlayerCreater reader/writers
        GameObjectCreationHelper.EnableStandardGameObjectCreation(world, entityPipeline, gameObject);


        base.HandleWorkerConnectionEstablished();
    }

    private void RemovingAuthoritativePlayer()
    {
        Debug.LogError($"Player entity got removed while still being connected. Disconnecting...");
        OnLostPlayerEntity?.Invoke();
    }

    protected override void HandleWorkerConnectionFailure(string errorMessage)
    {
        Debug.LogError($"Connection failed: {errorMessage}");
        Destroy(gameObject);
    }

    protected override IEnumerator LoadWorld()
    {
        yield return base.LoadWorld();
        isReadyToSpawn = true;
    }

    private void Update()
    {
        if (wantsSpawn && isReadyToSpawn)
        {
            wantsSpawn = false;
            SendRequest();
        }
    }

    public override void Dispose()
    {
        if (entityPipeline != null)
        {
            entityPipeline.OnRemovedAuthoritativePlayer -= RemovingAuthoritativePlayer;
        }

        base.Dispose();
    }

    private IEnumerator PrepareDestroy()
    {
        yield return DeferredDisposeWorker();
        Destroy(gameObject);
    }

    private void SendRequest()
    {
        // TODO readd message
        var loginArgs = new LoginData(this.playerName, "valid", this.gunId);
        var serializedArgs = Encoding.ASCII.GetBytes(UnityEngine.JsonUtility.ToJson(loginArgs));
        Worker.World.GetExistingSystem<SendCreatePlayerRequestSystem>()
            .RequestPlayerCreation(serializedArgs, onPlayerResponse);
    }

    protected override IConnectionHandlerBuilder GetConnectionHandlerBuilder()
    {
        var connectionParams = new ConnectionParameters
        {
            DefaultComponentVtable = new ComponentVtable(),
            WorkerType = WorkerUtils.UnityClient
        };
        
        var builder = new SpatialOSConnectionHandlerBuilder()
            .SetConnectionParameters(connectionParams);
        if (useLocal)
        {
            builder.SetConnectionFlow(new ReceptionistFlow(CreateNewWorkerId(WorkerUtils.UnityClient)));
        }else
        {
            connectionParams.Network.UseExternalIp = true;
            builder.SetConnectionFlow(new LndLocatorFlow(deployment, loginToken, playerIdentityToken, new LndConnectionFlowInitializer(new CommandLineConnectionFlowInitializer())));
        }
       
       

        return builder;
    }
   
}
