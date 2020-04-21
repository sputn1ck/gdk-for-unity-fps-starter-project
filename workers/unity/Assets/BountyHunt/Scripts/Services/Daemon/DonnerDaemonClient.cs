using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daemon;
using Grpc.Core;
using System.Threading.Tasks;
using System;


// TODO try catch blocks every rpc call
public class DonnerDaemonClient : IDonnerDaemonClient
{
    public Channel rpcChannel;
    public DaemonService.DaemonServiceClient client;

    public static DonnerDaemonClient instance;
    public string command;
    public bool commandTrigger;


    public async Task Setup()
    {
        rpcChannel = new Channel("localhost:10101", ChannelCredentials.Insecure);
        client = new DaemonService.DaemonServiceClient(rpcChannel);
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

    public async Task<GetConnectionResponse> GetConnection()
    {
        return await client.GetConnectionAsync(new GetConnectionRequest());
    }

    public async Task<GetBalanceResponse> GetWalletBalance()
    {
        return await client.GetBalanceAsync(new GetBalanceRequest());
    }

    public void Shutdown()
    {
        Task t = Task.Run(async () => await rpcChannel.ShutdownAsync());
        t.Wait(5000);
    }
}
