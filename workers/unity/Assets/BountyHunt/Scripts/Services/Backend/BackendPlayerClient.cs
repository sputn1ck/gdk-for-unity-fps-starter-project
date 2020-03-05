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

    public async void UpdateBackendStats(string playerpubkey)
    {
        var highscores = await GetHighscore();
        /*
        var myScore = highscores.FirstOrDefault(h => h.Pubkey == playerpubkey);
        if(myScore != null)
        {
            // TODO Invoke player Events
            ClientEvents.instance.onPlayerLifeTimeKillsUpdate.Invoke(myScore.Kills);
            ClientEvents.instance.onPlayerLifeTimeEarningsUpdate.Invoke(myScore.Earnings);
        }
        
        var topKills = highscores.OrderBy(h => h.Kills).ToList()[0];
        var topDeaths = highscores.OrderBy(h => h.Deaths).ToList()[0];
        var topEarnings = highscores.OrderBy(h => h.Earnings).ToList()[0];
        // TODO invoke total events
       var totalDeaths= highscores.Sum(h => h.Deaths);
        var totalKills = highscores.Sum(h => h.Kills);
        var totalEarnings = highscores.Sum(h => h.Earnings);
        */
        long totalEarnings = 0;
        int highestEarningsPlayerIndex = 0;
        int highestKillsPlayerIndex = 0;
        int highestDeathsPlayerIndex = 0;
        int totalKills = 0;
        int totalDeaths = 0;

        for (int i = 0; i > highscores.Length; i++)
        {
            var player = highscores[0];
            if(player.Pubkey == playerpubkey)
            {
                ClientEvents.instance.onPlayerLifeTimeKillsUpdate.Invoke(player.Kills);

                ClientEvents.instance.onPlayerLifeTimeDeathsUpdate.Invoke(player.Deaths);
                ClientEvents.instance.onPlayerLifeTimeEarningsUpdate.Invoke(player.Earnings);
            }
            if(player.Earnings > highscores[highestKillsPlayerIndex].Earnings)
            {
                highestEarningsPlayerIndex = i;
            }
            if (player.Kills > highscores[highestKillsPlayerIndex].Kills)
            {
                highestKillsPlayerIndex = i;
            }
            if (player.Deaths > highscores[highestKillsPlayerIndex].Deaths)
            {
                highestDeathsPlayerIndex = i;
            }

            totalEarnings += player.Earnings;
            totalKills += player.Kills;
            totalDeaths += player.Deaths;

        }

        ClientEvents.instance.onAllTimeKillsUpdate.Invoke(totalKills);
        ClientEvents.instance.onAllTimeDeathsUpdate.Invoke(totalDeaths);
        ClientEvents.instance.onAllTimeEarningsUpdate.Invoke(totalEarnings);
    }
}
