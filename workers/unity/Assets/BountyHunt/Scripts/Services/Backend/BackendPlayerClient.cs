using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bbh;
using System.Collections.Concurrent;
using System.Threading;
using System;
using Grpc.Core;
using System.Threading.Tasks;
using System.Linq;

public class BackendPlayerClient
{
    public ClientService.ClientServiceClient client;

    private Grpc.Core.Channel rpcChannel;

    private string pubkey;
    private string signature;
    public void Setup(string target,int port, string pubkey, string signature)
    {
        rpcChannel = new Grpc.Core.Channel(target, port, Grpc.Core.ChannelCredentials.Insecure);
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

    public async void UpdateBackendStats(string playerpubkey)
    {
        var highscores = await GetHighscore();
        long totalEarnings = 0;
        int highestEarningsPlayerIndex = 0;
        int highestKillsPlayerIndex = 0;
        int highestDeathsPlayerIndex = 0;
        int totalKills = 0;
        int totalDeaths = 0;
        if (highscores.Length == 0)
            return;
        for (int i = 0; i < highscores.Length; i++)
        {
            var player = highscores[i];
            if(player.Pubkey == playerpubkey)
            {
                ClientEvents.instance.onPlayerLifeTimeKillsUpdate.Invoke(player.Kills);

                ClientEvents.instance.onPlayerLifeTimeDeathsUpdate.Invoke(player.Deaths);
                ClientEvents.instance.onPlayerLifeTimeEarningsUpdate.Invoke(player.Earnings);
            }
            if(player.Earnings > highscores[highestEarningsPlayerIndex].Earnings)
            {
                highestEarningsPlayerIndex = i;
            }
            if (player.Kills > highscores[highestKillsPlayerIndex].Kills)
            {
                highestKillsPlayerIndex = i;
            }
            if (player.Deaths > highscores[highestDeathsPlayerIndex].Deaths)
            {
                highestDeathsPlayerIndex = i;
            }

            totalEarnings += player.Earnings;
            totalKills += player.Kills;
            totalDeaths += player.Deaths;

        }
        ClientEvents.instance.onAllTimeMostKillsUpdate.Invoke(new AllTimeScoreUpdateArgs { name = highscores[highestKillsPlayerIndex].Name,score = highscores[highestKillsPlayerIndex].Kills});
        ClientEvents.instance.onAllTimeMostDeathsUpdate.Invoke(new AllTimeScoreUpdateArgs { name = highscores[highestDeathsPlayerIndex].Name,score = highscores[highestDeathsPlayerIndex].Deaths});
        ClientEvents.instance.onAllTimeMostEarningsUpdate.Invoke(new AllTimeScoreUpdateArgs { name = highscores[highestEarningsPlayerIndex].Name,score = highscores[highestEarningsPlayerIndex].Earnings});

        ClientEvents.instance.onAllTimeKillsUpdate.Invoke(totalKills);
        ClientEvents.instance.onAllTimeDeathsUpdate.Invoke(totalDeaths);
        ClientEvents.instance.onAllTimeEarningsUpdate.Invoke(totalEarnings);
    }
}
