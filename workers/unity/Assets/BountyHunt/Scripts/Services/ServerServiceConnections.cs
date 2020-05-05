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

    public bool UseDummy;

    public GameObject DummyServices;
    public string PlatformPubkey;
    public string confName;
    public string lndConnectString;
    public static ServerServiceConnections instance;
    public IClientLnd lnd;

    public string BackendHost;
    public int BackendPort;
    public string PrometheusHost;
    public IAuctionController AuctionController;


    public IBackendServerClient BackendGameServerClient;
    public IBackendPlayerClient BackendPlayerClient;
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
        if (UseDummy)
        {
            SetupDummies();
        }else {
            Setup();
        }

    }
    public async void SetupDummies()
    {
        if(DummyServices == null)
        {

            var dummyGO = new GameObject("1_ServerDummies");
            lnd = dummyGO.AddComponent<DummyLnd>();
            await lnd.Setup(confName, true, false, "", lndConnectString);
            BackendGameServerClient = dummyGO.AddComponent<DummyBackendServerClient>();
            BackendGameServerClient.Setup("", 0, "", "");
            BackendPlayerClient = dummyGO.AddComponent<DummyBackendClientClient>();
            BackendPlayerClient.Setup("", 0, "", "");
            AuctionController = dummyGO.AddComponent<DummyAuctionController>();
            AuctionController.Setup();
            Prometheus = new PrometheusManager();
            Prometheus.Setup("");

            Instantiate(dummyGO, this.transform);
        } else
        {
            lnd = DummyServices.GetComponent<DummyLnd>();
            if (lnd == null)
            {
                lnd = DummyServices.AddComponent<DummyLnd>();
            }
            await lnd.Setup(confName, true, false, "", lndConnectString);

            BackendGameServerClient = DummyServices.GetComponent<DummyBackendServerClient>();
            if (BackendGameServerClient == null)
            {
                BackendGameServerClient = DummyServices.AddComponent<DummyBackendServerClient>();
            }
            BackendGameServerClient.Setup("", 0, "", "");

            BackendPlayerClient = DummyServices.GetComponent<DummyBackendClientClient>();
            if (BackendPlayerClient == null)
            {
                BackendPlayerClient = DummyServices.AddComponent<DummyBackendClientClient>();
            }
            BackendPlayerClient.Setup("", 0, "", "");

            AuctionController = DummyServices.GetComponent<DummyAuctionController>();
            if (AuctionController == null)
            {
                AuctionController = DummyServices.AddComponent<DummyAuctionController>();
            }
            AuctionController.Setup();
            Prometheus = new PrometheusManager();
            Prometheus.Setup("");
        }

    }
    public async void Setup()
    {
        await SetupLnd();
        SetupBackendServices();
    }
    public async Task SetupLnd()
    {

            lnd = new LndClient();

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
        lnd.ShutDown();
        BackendGameServerClient.Shutdown();
        BackendPlayerClient.Shutdown();
        AuctionController.Shutdown();
        Prometheus.Shutdown();
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
