using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Lnrpc;
using UnityEngine;

public class LnServer : MonoBehaviour
{
    public string confName;
    public bool UseDummy;
    public static LnServer instance;
    public IClientLnd lnd;
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
        if (UseDummy)
        {
            lnd = new DummyLnd();
        }
        else
        {
            lnd = new LndClient();
        }

        lnd.Setup(confName, true, false);
    }

    public void Update()
    {
        
    }

    public void OnApplicationQuit()
    {
        lnd.ShutDown();
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
