using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bbhrpc;
using System.Collections.Concurrent;
using System.Threading;
using System;
using Grpc.Core;
using System.Threading.Tasks;
using System.Linq;

public class BackendPlayerClient : IBackendPlayerClient
{
    public GameClientService.GameClientServiceClient client;
    public PublicService.PublicServiceClient publicCLient;

    private Channel rpcChannel;

    private string pubkey;
    private string signature;
    public void Setup(string target,int port, string pubkey, string signature)
    {
        rpcChannel = new Grpc.Core.Channel(target, port, Grpc.Core.ChannelCredentials.Insecure);
        client = new GameClientService.GameClientServiceClient(rpcChannel);
        this.pubkey = pubkey;
        this.signature = signature;

    }

    public async Task<string> GetUsername(string pubkey)
    {
        
        var res = await client.GetUsernameAsync(new GetUsernameRequest() { }, GetPubkeyCalloptions());
        return res.Name;
    }

    public async Task<string> SetUsername(string pubkey, string userName)
    {
        var res = await client.SetUsernameAsync(new SetUsernameRequest() { Name = userName }, GetPubkeyCalloptions());
        return res.Name;
    }

    public async Task<Ranking[]> ListRankings(int length, int startIndex, RankType rankType)
    {
        var res = await publicCLient.ListRankingsAsync(new ListRankingsRequest
        {
            RankType = rankType,
            Length = length,
            StartIndex = startIndex
        });
        Ranking[] rankings = new Ranking[res.Rankings.Count];
        res.Rankings.CopyTo(rankings, 0);
        return rankings;
    }
   

    private CallOptions GetPubkeyCalloptions()
    {
        var md = new Metadata();
        md.Add("pubkey", pubkey);
        md.Add("sig", signature);
        var co = new CallOptions(headers: md);
        return co;
    }

    public void Shutdown()
    {

        //Debug.Log(rpcChannel.State + "state, shutdownToken: " + rpcChannel.ShutdownToken.IsCancellationRequested);
        Task t = Task.Run(async () => await rpcChannel.ShutdownAsync());
        t.Wait(5000);
        //Debug.Log(rpcChannel.State + "state, shutdownToken: " + rpcChannel.ShutdownToken.IsCancellationRequested);
    }

   
}
