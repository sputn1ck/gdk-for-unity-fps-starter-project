using Bbhrpc;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class DummyBackendServerClient : MonoBehaviour, IBackendServerClient
{

    public int bbhDuration = 30;
    public int lobbyDuration = 10;

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
        return new GetRoundInfoResponse()
        {
            Subsidy = 100,
            Settings = new GameModeSettings
            {
                SecondDuration = req.GameMode == Bbhrpc.GameMode.Bountyhunt ? bbhDuration: lobbyDuration,
                BaseSettings = new BaseSettings { ClearBountyOnEnd = true, ClearPickupsOnEnd = true, ClearStatsOnEnd = true, TeleportPlayerOnStart = true },
                BountySettings = new BountySettings { BountyDropPercentageDeath = 1, BountyTickConversion = 0.05, BountyTickTimeSeconds = 5},
                SpawnSettings = new SpawnSettings { MaxSpawnsPerSpawn = 40, MinSpawnsPerSpawn = 10, Distribution = BountyDistribution.Uniform, TimeBetweenSpawns = 10},
            },
            
        };
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
