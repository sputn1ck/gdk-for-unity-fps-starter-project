using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daemon;
using Grpc.Core;
using System.Threading.Tasks;
using System;


// TODO try catch blocks every rpc call
public class DonnerDaemonClient
{
    public Channel rpcChannel;
    public DaemonService.DaemonServiceClient client;

    public static DonnerDaemonClient instance;
    public string command;
    public bool commandTrigger;


    public void Setup()
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
    public async Task<string> GetInvoice(string memo, long amount)
    {
        var res = await client.CreateInvoiceAsync(new CreateInvoiceRequest() { Amt = amount, Description = memo });
        return res.Invoice;
    }

    public async Task<GetInfoResponse> GetInfo()
    {
        return await client.GetInfoAsync(new GetInfoRequest());
    }

    public async Task<GetConnectionResponse> GetConnection()
    {
        return await client.GetConnectionAsync(new GetConnectionRequest());
    }

    public async Task<WalletBalanceResponse> GetWalletBalance()
    {
        return await client.WalletBalanceAsync(new WalletBalanceRequest());
    }

    public void Shutdown()
    {
        Task t = Task.Run(async () => await rpcChannel.ShutdownAsync());
        t.Wait(5000);
    }
}
