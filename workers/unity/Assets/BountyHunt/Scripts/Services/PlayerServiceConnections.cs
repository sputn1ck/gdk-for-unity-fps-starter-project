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




    [HideInInspector] public bool ServicesReady = false;
    void Awake()
    {
        if(UseDummy && DummyServices == null)
        {
            var dummyGO = new GameObject("0_PlayerDummies");
            dummyGO.AddComponent<DummyLnd>();
            dummyGO.AddComponent<DummyBackendClientClient>();
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
        ServicesReady = true;
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
        DonnerDaemonClient?.Shutdown();
        lnd?.ShutDown();
        BackendPlayerClient?.Shutdown();
        Debug.Log("client quit cleanly");
        UrlMemory.OpenAllUrls();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            UrlMemory.OpenAllUrls();
        }
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
        var (highscores,totalElements) = await BackendPlayerClient.ListRankings(0,0,Bbhrpc.RankType.Global);
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
                ClientEvents.instance.onPlayerLifeTimeKillsUpdate.Invoke(player.Stats.Kills);

                ClientEvents.instance.onPlayerLifeTimeDeathsUpdate.Invoke(player.Stats.Deaths);
                ClientEvents.instance.onPlayerLifeTimeEarningsUpdate.Invoke(player.Stats.Earnings);
            }
            if (player.Stats.Earnings > highscores[highestEarningsPlayerIndex].Stats.Earnings)
            {
                highestEarningsPlayerIndex = i;
            }
            if (player.Stats.Kills > highscores[highestKillsPlayerIndex].Stats.Kills)
            {
                highestKillsPlayerIndex = i;
            }
            if (player.Stats.Deaths > highscores[highestDeathsPlayerIndex].Stats.Deaths)
            {
                highestDeathsPlayerIndex = i;
            }

            totalEarnings += player.Stats.Earnings;
            totalKills += player.Stats.Kills;
            totalDeaths += player.Stats.Deaths;

        }
        ClientEvents.instance.onAllTimeMostKillsUpdate.Invoke(new AllTimeScoreUpdateArgs { name = highscores[highestKillsPlayerIndex].Name, score = highscores[highestKillsPlayerIndex].Stats.Kills });
        ClientEvents.instance.onAllTimeMostDeathsUpdate.Invoke(new AllTimeScoreUpdateArgs { name = highscores[highestDeathsPlayerIndex].Name, score = highscores[highestDeathsPlayerIndex].Stats.Deaths });
        ClientEvents.instance.onAllTimeMostEarningsUpdate.Invoke(new AllTimeScoreUpdateArgs { name = highscores[highestEarningsPlayerIndex].Name, score = highscores[highestEarningsPlayerIndex].Stats.Earnings });

        ClientEvents.instance.onAllTimeKillsUpdate.Invoke(totalKills);
        ClientEvents.instance.onAllTimeDeathsUpdate.Invoke(totalDeaths);
        ClientEvents.instance.onAllTimeEarningsUpdate.Invoke(totalEarnings);
    }


}

public static class UrlMemory
{

    static List<string> urlQueue = new List<string>();

    public static void AddUrl(string url)
    {
        if (!urlQueue.Contains(url))
        {
            urlQueue.Add(url);
        }
    }

    public static bool UrlInQueue(string url)
    {
        return urlQueue.Contains(url);
    }

    public static void OpenAllUrls()
    {
        foreach (string link in urlQueue)
        {
            Application.OpenURL(link);
        }
        urlQueue.Clear();
    }

}

public delegate void StringFunc(string s);
