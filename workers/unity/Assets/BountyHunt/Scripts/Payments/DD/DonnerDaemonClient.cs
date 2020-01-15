using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Donnerrpc;
using Grpc.Core;
using System.Threading.Tasks;
using System;

public class DonnerDaemonClient : MonoBehaviour
{
    public Channel rpcChannel;
    public DonnerDaemon.DonnerDaemonClient client;

    public static DonnerDaemonClient instance;
    public string command;
    public bool commandTrigger;
    // Start is called before the first frame update
    void Awake()
    {
        Setup();
        instance = this;
    }

    // Update is called once per frame
    async void Update()
    {
        if (commandTrigger)
        {
            commandTrigger = false;
            var s = await Lncli(command);
            Debug.Log(s);
        }
    }

    void Setup()
    {
        rpcChannel = new Channel("localhost:10101", ChannelCredentials.Insecure);
        client = new DonnerDaemon.DonnerDaemonClient(rpcChannel);
        
    }

    public async Task<string> Lncli(string command)
    {
        try
        {
            var res = await client.LncliAsync(new LncliRequest { Command = command });
            return res.TextResponse;
        }catch(Exception e)
        {
            Debug.Log(e);
            return e.Message;
        }
    }

    public async void OnApplicationQuit()
    {
        await rpcChannel.ShutdownAsync();
    }
}
