using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Donnerrpc;
using Grpc.Core;
using System.Threading.Tasks;
using System;

public class DonnerDaemonClient
{
    public Channel rpcChannel;
    public DonnerDaemon.DonnerDaemonClient client;

    public static DonnerDaemonClient instance;
    public string command;
    public bool commandTrigger;


    public void Setup()
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


    public void Shutdown()
    {
        Task t = Task.Run(async () => await rpcChannel.ShutdownAsync());
        t.Wait(5000);
    }
}
