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
    public bool UseDummy;
    public static PlayerServiceConnections instance;
    public IClientLnd lnd;
    public bool UseApdata;

    public string BackendHost;
    public BackendPlayerClient BackendPlayerClient;

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
        await lnd.Setup(confName, false, UseApdata);
    }


    public void SetupBackendServices()
    {
        var message = "DO_NOT_SIGN_DONNERDUNGEON_AUTHENTICATION_MESSAGE";
        var sig = lnd.SignMessage(message);


        // Player Client
        BackendPlayerClient = new BackendPlayerClient();
        BackendPlayerClient = new BackendPlayerClient();
        BackendPlayerClient.Setup(BackendHost, lnd.GetPubkey(), sig.Signature);
    }


    public void OnApplicationQuit()
    {

        lnd.ShutDown();
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
