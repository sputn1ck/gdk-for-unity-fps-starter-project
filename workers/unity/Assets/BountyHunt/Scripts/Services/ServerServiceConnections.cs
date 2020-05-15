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

            Prometheus = new PrometheusManager();
            Prometheus.Setup("");
        }

    }
    public async void Setup()
    {
        await SetupLnd();
        await SetupBackendServices();
    }
    public async Task SetupLnd()
    {

            lnd = new LndClient();

        await lnd.Setup(confName, true, false, "", lndConnectString);
        StartCoroutine(lnd.HandleInvoices(ct));
    }

    public async Task SetupBackendServices()
    {
        var message = "DO_NOT_SIGN_DONNERDUNGEON_AUTHENTICATION_MESSAGE";
        var sig = lnd.SignMessage(message);

        // Game Server Client
        BackendGameServerClient = new BackendGameserverClient();
        BackendGameServerClient.Setup(BackendHost,BackendPort, lnd.GetPubkey(), sig.Signature);
        BackendGameServerClient.StartListening();
        StartCoroutine(BackendGameServerClient.HandleBackendEvents(ct));
        // Player Client
        BackendPlayerClient = new BackendPlayerClient();
        await BackendPlayerClient.Setup(BackendHost, BackendPort, lnd.GetPubkey(), sig.Signature);

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
