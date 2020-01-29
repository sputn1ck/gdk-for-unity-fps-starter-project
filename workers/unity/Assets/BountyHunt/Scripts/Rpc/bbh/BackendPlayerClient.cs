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

    public void Setup(string target)
    {
        rpcChannel = new Grpc.Core.Channel(target, 8899, Grpc.Core.ChannelCredentials.Insecure);
        client = new ClientService.ClientServiceClient(rpcChannel);
        

    }

    public async Task<string> GetUsername(string pubkey)
    {
        
        var res = await client.GetUsernameAsync(new GetUsernameRequest() { }, GetPubkeyCalloptions(pubkey));
        return res.Name;
    }

    public async Task<string> SetUsername(string pubkey, string userName)
    {
        var res = await client.SetUsernameAsync(new SetUsernameRequest() { Name = userName }, GetPubkeyCalloptions(pubkey));
        return res.Name;
    }

    public async Task<Highscore[]> GetHighscore()
    {
        var res = await client.GetHighscoreAsync(new GetHighscoreRequest() { });
        Highscore[] highscores = new Highscore[res.Highscores.Count];
        res.Highscores.CopyTo(highscores, 0);
        return highscores;
    }

    private CallOptions GetPubkeyCalloptions(string pubkey)
    {
        var md = new Metadata();
        md.Add("pubkey", pubkey);
        var co = new CallOptions(headers: md);
        return co;
    }

    public void Shutdown()
    {

        Debug.Log(rpcChannel.State + "state, shutdownToken: " + rpcChannel.ShutdownToken.IsCancellationRequested);
        Task t = Task.Run(async () => await rpcChannel.ShutdownAsync());
        t.Wait(5000);
        Debug.Log(rpcChannel.State + "state, shutdownToken: " + rpcChannel.ShutdownToken.IsCancellationRequested);
    }
}
