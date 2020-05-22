using Bbhrpc;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class DummyBackendServerClient : MonoBehaviour, IBackendServerClient
{

    public int ssDuration = 30;
    public int lobbyDuration = 10;
    public int bbDuration = 30;
    public long subsidy = 10000;
    public double bountyDropOnDeath = 1.0;
    public double bountyConversion = 0.1;
    public float bountyConversionTimeSeconds = 10;
    public string[] PlayerSkins;

    public bool invokeKick;
    public string nameKick;
    public string pubkeyKick;

    public string sender;
    public string message;
    public bool announce;
    public bool messageTrigger;
    public void Setup(string target, int port, string pubkey, string message)
    {
        
    }
    public void Update()
    {
        if (invokeKick)
        {
            invokeKick = false;
            ServerEvents.instance.OnBackendKickEvent.Invoke(
                new KickEvent
                {
                    UserName = nameKick,
                    UserPubkey = pubkeyKick,
                }
            );
        }
        if(messageTrigger)
        {
            messageTrigger = false;
            ServerEvents.instance.OnBackendChatEvent.Invoke(new ChatEvent
            {
                Announce = announce,
                Sender = sender,
                Message = message
            });
        }
    }
    public void Shutdown()
    {
        
    }

    public void StartListening()
    {
        
    }

    public Task<User> GetUser(string pubkey)
    {
        return Task.FromResult(new User
        {
            Name = "" + Random.Range(1, 200),
            Pubkey = "pk" + Random.Range(1, int.MaxValue),
        });
    }

    public void AddKill(string killer, string victim)
    {
       
    }

    public void AddEarnings(string user, long earnings)
    {
        
    }

    public void AddPlayerHeartbeat(string user, long bounty, int kills, int deaths)
    {
    }

    public void AddPlayerDisconnect(string user)
    {
        
    }

    public Task<GetRoundInfoResponse> GetRoundInfo(GetRoundInfoRequest request)
    {
        return Task.FromResult(getRes(request));
    }

    private GetRoundInfoResponse getRes(GetRoundInfoRequest req)
    {
        var advertisers = new Google.Protobuf.Collections.RepeatedField<AdvertiserInfo>();
        advertisers.Add(new AdvertiserInfo()
        {
            Discription = "",
            Name = "Become a BBH sponsor",
            Url = " https://bitcoinbountyhunt.com/sponsors",
            Sponsoring = 1000,
        });
        advertisers[0].SquareBannerUrls.Add("https://pics.donnerlab.com/pics/get/1005662206048b51adfd181ba63bfb9c1f0647a3e641442907ff45114999e9f8/e804db7d-5cf0-4d63-8da6-aa2f13548e3f.png");
        var roundinfo = new GetRoundInfoResponse()
        {
            Subsidy = subsidy,
            Settings = new GameModeSettings
            {
                SecondDuration = GetGameModeDuration(req.GameModeId),
                BaseSettings = new BaseSettings { ClearBountyOnEnd = true, ClearPickupsOnEnd = true, ClearStatsOnEnd = true, TeleportPlayerOnStart = true },
                BountySettings = new BountySettings { BountyDropPercentageDeath = bountyDropOnDeath, BountyTickConversion = bountyConversion, BountyTickTimeSeconds = bountyConversionTimeSeconds },
                SpawnSettings = new SpawnSettings { MaxSpawnsPerSpawn = 10, MinSpawnsPerSpawn = 20, Distribution = BountyDistribution.Uniform, TimeBetweenSpawns = 100 },
            },
            
        };
        roundinfo.Advertisers.Add(advertisers);
        return roundinfo;
    }
    public int GetGameModeDuration(string gameModeId)
    {
        switch (gameModeId) {
            case "satsstacker":
                return ssDuration;
            case "bountyboss":
                return bbDuration;
            case "lobby":
                return lobbyDuration;
        }
        return lobbyDuration;
    }
    public Task<string> GetUserSkin(string pubkey)
    {
        if (PlayerSkins.Length == 0)
        {
            return Task.FromResult("");
        }
        return Task.FromResult(PlayerSkins[UnityEngine.Random.Range(0, PlayerSkins.Length - 1)]);
    }

    public IEnumerator HandleBackendEvents(CancellationTokenSource ct)
    {
        return null;
    }
}
