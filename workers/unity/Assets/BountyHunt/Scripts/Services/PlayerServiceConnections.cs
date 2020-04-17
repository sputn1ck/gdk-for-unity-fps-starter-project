using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Fps;
using Grpc.Core;
using Lnrpc;
using UnityEngine;

public class PlayerServiceConnections : MonoBehaviour
{
    public bool UseDummy;
    public string confName;
    public string lndConnectString;
    public static PlayerServiceConnections instance;
    public IClientLnd lnd;
    public bool UseApdata;

    public string BackendHost;
    public string BackendPubkey;
    public int BackendPort;
    public IBackendPlayerClient BackendPlayerClient;

    public IDonnerDaemonClient DonnerDaemonClient;

    public IAuctionClient AuctionClient;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else
        {
            Destroy(this);
            return;
        }
        if (UseDummy)
        {
            SetupDummies();
        } else
        {
            Setup();
        }
        
    }

    public async void SetupDummies()
    {

        var dummyGO = new GameObject("0_PlayerDummies");
        lnd = dummyGO.AddComponent<DummyLnd>();
        await lnd.Setup(confName, true, false, "", lndConnectString);
        BackendPlayerClient = dummyGO.AddComponent<DummyBackendClientClient>();
        BackendPlayerClient.Setup("", 0, "", "");
        AuctionClient = dummyGO.AddComponent<DummyAuctionClient>();
        AuctionClient.Setup();
        DonnerDaemonClient = dummyGO.AddComponent<DummyDaemonClient>();
        DonnerDaemonClient.Setup();

        Instantiate(dummyGO, this.transform);
    }
    public async void Setup()
    {
        SetupDonnerDaemon();
        await SetupLnd();
        SetupBackendServices();
        ClientEvents.instance.onServicesSetup.Invoke();
    }


    public async Task SetupLnd()
    {

        lnd = new LndClient();
        await lnd.Setup(confName, false, UseApdata, "", lndConnectString);
    }

    

    public void SetupDonnerDaemon()
    {
        DonnerDaemonClient = new DonnerDaemonClient();
        DonnerDaemonClient.Setup();
    }


    public void SetupBackendServices()
    {
        var message = "DO_NOT_SIGN_DONNERDUNGEON_AUTHENTICATION_MESSAGE";
        var sig = lnd.SignMessage(message);


        // Player Client
        BackendPlayerClient = new BackendPlayerClient();
        BackendPlayerClient = new BackendPlayerClient();
        BackendPlayerClient.Setup(BackendHost, BackendPort, lnd.GetPubkey(), sig.Signature);

        // Auction Client
        AuctionClient = new AuctionClient();
        AuctionClient.Setup();
    }


    public void OnApplicationQuit()
    {
        DonnerDaemonClient.Shutdown();
        lnd.ShutDown();
        BackendPlayerClient.Shutdown();
        AuctionClient.Shutdown();
        Debug.Log("client quit cleanly");
    }


    public string GetPubkey()
    {

        Debug.Log("pubkey: " + lnd.GetPubkey());
        return lnd.GetPubkey();
    }

    public LnConf GetConfig()
    {
        return lnd.GetConfig();
    }

    public async void UpdateBackendStats(string playerpubkey)
    {
        var highscores = await BackendPlayerClient.ListRankings();
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
            if (player.Pubkey == playerpubkey)
            {
                ClientEvents.instance.onPlayerLifeTimeKillsUpdate.Invoke(player.Kills);

                ClientEvents.instance.onPlayerLifeTimeDeathsUpdate.Invoke(player.Deaths);
                ClientEvents.instance.onPlayerLifeTimeEarningsUpdate.Invoke(player.Earnings);
            }
            if (player.Earnings > highscores[highestEarningsPlayerIndex].Earnings)
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
        ClientEvents.instance.onAllTimeMostKillsUpdate.Invoke(new AllTimeScoreUpdateArgs { name = highscores[highestKillsPlayerIndex].Name, score = highscores[highestKillsPlayerIndex].Kills });
        ClientEvents.instance.onAllTimeMostDeathsUpdate.Invoke(new AllTimeScoreUpdateArgs { name = highscores[highestDeathsPlayerIndex].Name, score = highscores[highestDeathsPlayerIndex].Deaths });
        ClientEvents.instance.onAllTimeMostEarningsUpdate.Invoke(new AllTimeScoreUpdateArgs { name = highscores[highestEarningsPlayerIndex].Name, score = highscores[highestEarningsPlayerIndex].Earnings });

        ClientEvents.instance.onAllTimeKillsUpdate.Invoke(totalKills);
        ClientEvents.instance.onAllTimeDeathsUpdate.Invoke(totalDeaths);
        ClientEvents.instance.onAllTimeEarningsUpdate.Invoke(totalEarnings);
    }

}
