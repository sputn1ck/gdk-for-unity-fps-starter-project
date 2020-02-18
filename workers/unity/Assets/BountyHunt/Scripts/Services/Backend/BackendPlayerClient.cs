using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bbh;
using System.Collections.Concurrent;
using System.Threading;
using System;
using Grpc.Core;
using System.Threading.Tasks;

public class BackendPlayerClient
{
    public ClientService.ClientServiceClient client;

    private Grpc.Core.Channel rpcChannel;

    private string pubkey;
    private string signature;
    public void Setup(string target, string pubkey, string signature)
    {
        rpcChannel = new Grpc.Core.Channel(target, 8899, Grpc.Core.ChannelCredentials.Insecure);
        client = new ClientService.ClientServiceClient(rpcChannel);
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

    public async Task<Highscore[]> GetHighscore()
    {
        var res = await client.GetHighscoreAsync(new GetHighscoreRequest() { });
        Highscore[] highscores = new Highscore[res.Highscores.Count];
        res.Highscores.CopyTo(highscores, 0);
        return highscores;
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
