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
    public string PlatformPubkey;
    public string confName;
    public string lndConnectString;
    public bool UseDummy;
    public static ServerServiceConnections instance;
    public IClientLnd lnd;

    public string BackendHost;
    public int BackendPort;
    public string PrometheusHost;
    public AuctionController AuctionController;


    public BackendGameserverClient BackendGameServerClient;
    public BackendPlayerClient BackendPlayerClient;
    public PrometheusManager Prometheus;

    public static CancellationTokenSource ct;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            ct = new CancellationTokenSource();
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

        await lnd.Setup(confName, true, false, "", lndConnectString);
        StartCoroutine(lnd.HandleInvoices(ct));
    }

    public void SetupBackendServices()
    {
        var message = "DO_NOT_SIGN_DONNERDUNGEON_AUTHENTICATION_MESSAGE";
        var sig = lnd.SignMessage(message);

        // Game Server Client
        BackendGameServerClient = new BackendGameserverClient();
        BackendGameServerClient.Setup(BackendHost,BackendPort, lnd.GetPubkey(), sig.Signature);
        BackendGameServerClient.StartListening();

        // Player Client
        BackendPlayerClient = new BackendPlayerClient();
        BackendPlayerClient = new BackendPlayerClient();
        BackendPlayerClient.Setup(BackendHost, BackendPort, lnd.GetPubkey(), sig.Signature);

        // Auction Controller
        AuctionController = new AuctionController();
        AuctionController.Setup();

        //Prometheus
        Prometheus = new PrometheusManager();
        Prometheus.Setup(PrometheusHost);
    }



    public void Update()
    {

    }

    public void OnApplicationQuit()
    {
        ct.Cancel();
        Prometheus.Shutdown();
        lnd.ShutDown();
        BackendGameServerClient.Shutdown();
        BackendPlayerClient.Shutdown();
        AuctionController.Shutdown();
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
