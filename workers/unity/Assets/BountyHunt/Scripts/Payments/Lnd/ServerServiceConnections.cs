using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Lnrpc;
using UnityEngine;

public class ServerServiceConnections : MonoBehaviour
{
    public string confName;
    public bool UseDummy;
    public static ServerServiceConnections instance;
    public IClientLnd lnd;

    public string BackendHost;




    public BackendGameserverClient BackendGameServerClient;
    public BackendPlayerClient BackendPlayerClient;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        Setup();
    }

    public async void Setup()
    {
        await SetupLnd();
        SetupBackendServices();
    }
    public async Task SetupLnd()
    {
        if (UseDummy)
        {
            lnd = new DummyLnd();
        }
        else
        {
            lnd = new LndClient();
        }

        await lnd.Setup(confName, true, false);
    }

    public void SetupBackendServices()
    {
        var message = "DO_NOT_SIGN_DONNERDUNGEON_AUTHENTICATION_MESSAGE";
        var sig = lnd.SignMessage(message);

        // Game Server Client
        BackendGameServerClient = new BackendGameserverClient();
        BackendGameServerClient.Setup(BackendHost, lnd.GetPubkey(), sig.Signature);
        BackendGameServerClient.StartListening();

        // Player Client
        BackendPlayerClient = new BackendPlayerClient();
        BackendPlayerClient = new BackendPlayerClient();
        BackendPlayerClient.Setup(BackendHost, lnd.GetPubkey(), sig.Signature);
    }

    public void Update()
    {
        
    }

    public void OnApplicationQuit()
    {
        lnd.ShutDown();
        BackendGameServerClient.Shutdown();
        BackendPlayerClient.Shutdown();
        Debug.Log("server quit cleanly");
    }

    public string GetPubkey()
    {
        return lnd.GetPubkey();
    }

    public LnConf GetConfig()
    {
        return lnd.GetConfig();
    }

   

}
