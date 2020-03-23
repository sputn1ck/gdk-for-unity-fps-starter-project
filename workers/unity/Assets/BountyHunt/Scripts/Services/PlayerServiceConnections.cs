using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Fps;
using Grpc.Core;
using Lnrpc;
using UnityEngine;

public class PlayerServiceConnections : MonoBehaviour
{
    public string confName;
    public string lndConnectString;
    public bool UseDummy;
    public static PlayerServiceConnections instance;
    public IClientLnd lnd;
    public bool UseApdata;

    public string BackendHost;
    public int BackendPort;
    public BackendPlayerClient BackendPlayerClient;

    public DonnerDaemonClient DonnerDaemonClient;

    public AuctionClient AuctionClient;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else
        {
            Destroy(this);
            return;
        }
        Setup();
    }


    public async void Setup()
    {
        SetupDonnerDaemon();
        await SetupLnd();
        SetupBackendServices();
        ClientEvents.instance.onServicesSetup.Invoke();
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
        await lnd.Setup(confName, false, UseApdata, "", lndConnectString);
    }

    

    public void SetupDonnerDaemon()
    {
        DonnerDaemonClient = new DonnerDaemonClient();
        DonnerDaemonClient.Setup();
    }


    public void SetupBackendServices()
    {
        var message = "DO_NOT_SIGN_DONNERDUNGEON_AUTHENTICATION_MESSAGE";
        var sig = lnd.SignMessage(message);


        // Player Client
        BackendPlayerClient = new BackendPlayerClient();
        BackendPlayerClient = new BackendPlayerClient();
        BackendPlayerClient.Setup(BackendHost, BackendPort, lnd.GetPubkey(), sig.Signature);

        // Auction Client
        AuctionClient = new AuctionClient();
        AuctionClient.Setup();
    }


    public void OnApplicationQuit()
    {
        DonnerDaemonClient.Shutdown();
        lnd.ShutDown();
        BackendPlayerClient.Shutdown();
        AuctionClient.Shutdown();
        Debug.Log("client quit cleanly");
    }


    public string GetPubkey()
    {

        Debug.Log("pubkey: " + lnd.GetPubkey());
        return lnd.GetPubkey();
    }

    public LnConf GetConfig()
    {
        return lnd.GetConfig();
    }

    

}
