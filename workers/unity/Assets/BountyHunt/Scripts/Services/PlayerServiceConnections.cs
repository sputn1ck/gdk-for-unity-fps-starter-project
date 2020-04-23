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
    public string GameVersion;

    public bool UseDummy;
    public GameObject DummyServices;
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
        if(UseDummy && DummyServices == null)
        {
            var dummyGO = new GameObject("0_PlayerDummies");
            dummyGO.AddComponent<DummyLnd>();
            dummyGO.AddComponent<DummyBackendClientClient>();
            dummyGO.AddComponent<DummyAuctionClient>();
            dummyGO.AddComponent<DummyDaemonClient>();
            DummyServices = Instantiate(dummyGO, this.transform);
        }
        if(instance == null)
        {
            instance = this;
        }else
        {
            Destroy(this);
            return;
        }
        
    }

    

    public async Task<bool> CheckName()
    {
        var getinfo = await lnd.GetInfo();
        var pubkey = getinfo.IdentityPubkey;
        var name = await BackendPlayerClient.GetUsername();
        bool setName;
        if (pubkey == name)
        {
            setName = true;
        }
        else
        {
            setName = false;

        }
        return setName;
    }


    public async Task SetupServices(StringFunc stringFunc)
    {
        stringFunc("Setting up connections");
        // DonnerDaemon
        stringFunc("Connecting to daemon");
        try
        {
            await SetupDonnerDaemon();
        } catch(Exception e)
        {
            throw new Exception( "Daemon connection failed: " + e.Message,e);
        }
        

        // Lnd
        stringFunc("Connecting to lnd");
        
        try
        {
            await SetupLnd();
        }
        catch (Exception e)
        {
            throw new Exception("Lnd connection failed: " + e.Message,e);
        }

        // Backend
        stringFunc("Connecting to game server");
        try
        {

            await SetupBackend();
        }
        catch (Exception e)
        {
            throw new Exception("Backend connection failed: " + e.Message,e);
        }
        // Check Game Version
        stringFunc("Checking game version");
        try
        {
            var gv = await BackendPlayerClient.GetGameVersion();
            if(gv != this.GameVersion)
            {
                throw new Exception("Invalid Game Version");
            }
        }
        catch (Exception e)
        {
            throw new Exception("Checking game version failed: " + e.Message, e);
        }
        // Auction
        stringFunc("Connecting to payment server");
        try
        {
            await SetupAuctionClient();
        }
        catch (Exception e)
        {
            throw new Exception ("Payment connection failed: " + e.Message,e);
        }
        ClientEvents.instance.onServicesSetup.Invoke();
    }

    private async Task SetupBackend()
    {
        SignMessageResponse sig = new SignMessageResponse() { Signature = "" };
        if (UseDummy)
        {
            BackendPlayerClient = DummyServices.GetComponent<DummyBackendClientClient>();
            if (BackendPlayerClient == null)
            {
                BackendPlayerClient = DummyServices.AddComponent<DummyBackendClientClient>();
            }
        }
        else
        {

            var message = "DO_NOT_SIGN_DONNERDUNGEON_AUTHENTICATION_MESSAGE";
            sig = lnd.SignMessage(message);
            // Player Client
            BackendPlayerClient = new BackendPlayerClient();
            BackendPlayerClient = new BackendPlayerClient();
        }
        await BackendPlayerClient.Setup(BackendHost, BackendPort, lnd.GetPubkey(), sig.Signature);
    }

    private async Task SetupAuctionClient()
    {
        if (UseDummy)
        {

            AuctionClient = DummyServices.GetComponent<DummyAuctionClient>();
            if (AuctionClient == null)
            {
                AuctionClient = DummyServices.AddComponent<DummyAuctionClient>();
            }
        }
        else
        {
            AuctionClient = new AuctionClient();
        }
        await AuctionClient.Setup();
    }

    private async Task SetupDonnerDaemon()
    {
        if (UseDummy)
        {
            DonnerDaemonClient = DummyServices.GetComponent<DummyDaemonClient>();
            if (DonnerDaemonClient == null)
            {
                DonnerDaemonClient = DummyServices.AddComponent<DummyDaemonClient>();
            }
        }
        else
        {
            DonnerDaemonClient = new DonnerDaemonClient();
        }
        await DonnerDaemonClient.Setup();
    }

    private async Task SetupLnd()
    {
        // Lnd
        if (UseDummy)
        {
            lnd = DummyServices.GetComponent<DummyLnd>();
            if (lnd == null)
            {
                lnd = DummyServices.AddComponent<DummyLnd>();
            }
        }
        else
        {
            lnd = new LndClient();
        }
        await lnd.Setup(confName, false, UseApdata, "", lndConnectString);
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
        var highscores = await BackendPlayerClient.ListRankings(0,0,Bbhrpc.RankType.Global);
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

public delegate void StringFunc(string s);
