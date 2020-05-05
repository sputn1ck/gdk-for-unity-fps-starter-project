using Bbhrpc;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Threading;

public class DummyBackendClientClient : MonoBehaviour, IBackendPlayerClient
{
    public string GameVersion;

    [Header("Name")]
    public string userName;
    public bool needsUserNameChange;

    [Header("Rankings")]
    public int HighscoresCount;
    public Vector2Int earningsRange;
    public Vector2Int killsRange;
    public Vector2Int deathsRange;
    public long playerEarnings;
    public int playerKills;
    public int playerDeaths;
    Ranking[] highscores;

    public float GoldThreshold;
    public float SilverThreshold;
    public float BronzeThreshold;


    [Header("Skins")]
    public string[] allSkins = new string[] { "robot_default", "robot_2" };
    public List<string> ownedSkins = new List<string>{ "robot_default" };
    public string equippedSkin = "robot_default";
    public Dictionary<string, string> activeSkinInvoices = new Dictionary<string, string>();


    [Header("Payments")]
    public bool paymentReturnValue;
    public bool returnPayment;
    public int expiryInSeconds;
    public bool triggerPaymentTest;
    // Update is called once per frame

    private void Awake()
    {
        highscores = new Ranking[this.HighscoresCount + 1];
        var playerKdDeaths = 1;
        if (playerDeaths > 0){
            playerKdDeaths = playerDeaths;
        }
        highscores[0] = new Ranking()
        {
            Name = userName,
            Pubkey = "Pubkey: 0",
            Stats = new Stats()
            {

                Deaths = playerDeaths,
                Kills = playerKills,
                Earnings = playerEarnings,

            },
            GlobalRanking = new LeagueRanking(),
            KdRanking = new LeagueRanking
            {
                Score = (int)((((float)playerKills) / (playerKdDeaths))*10000)
            },
            EarningsRanking = new LeagueRanking
            {
                Score = playerEarnings
            }
        };
        for (int i = 1; i < highscores.Length; i++)
        {
            int k = UnityEngine.Random.Range(killsRange.x, killsRange.y);
            int d = UnityEngine.Random.Range(deathsRange.x, deathsRange.y);
            int e = UnityEngine.Random.Range(earningsRange.x, earningsRange.y);
            int kdDeaths = 1;
            if (d > 0) {
                kdDeaths = d;
            }
            float kd = (float)k / kdDeaths;
            highscores[i] = new Ranking()
            {
                Name = "Player: " + i,
                Pubkey = "Pubkey: " + i,

                Stats = new Stats()
                {
                    Deaths = d,
                    Kills = k,
                    Earnings = e,
                },

                GlobalRanking = new LeagueRanking(),
                KdRanking = new LeagueRanking
                {
                    Score = (int)(kd*10000)
                },
                EarningsRanking = new LeagueRanking
                {
                    Score = e
                }
            };
        }
        highscores = highscores.OrderByDescending(h => h.KdRanking.Score).ToArray();
        for(int i = 0; i < highscores.Length; i++)
        {
            highscores[i].KdRanking.Rank = i + 1;
        }
        highscores = highscores.OrderByDescending(h => h.EarningsRanking.Score).ToArray();
        for (int i = 0; i < highscores.Length; i++)
        {
            highscores[i].EarningsRanking.Rank = i + 1;
            highscores[i].GlobalRanking.Score = highscores[i].KdRanking.Rank * highscores[i].EarningsRanking.Rank;
        }

        highscores = highscores.OrderBy(h => h.GlobalRanking.Score).ToArray();
        for (int i = 0; i < highscores.Length; i++)
        {
            highscores[i].GlobalRanking.Rank = i + 1;
            setBadge(highscores[i].GlobalRanking,highscores.Length);
            setBadge(highscores[i].KdRanking, highscores.Length);
            setBadge(highscores[i].EarningsRanking, highscores.Length);
        }
    }

    void setBadge(LeagueRanking ranking, int totalCount)
    {
        if ((float)ranking.Rank / (float)totalCount <= GoldThreshold) ranking.Badge = RankBadge.Gold;
        else if ((float)ranking.Rank / (float)totalCount <= SilverThreshold) ranking.Badge = RankBadge.Silver;
        else if ((float)ranking.Rank / (float)totalCount <= BronzeThreshold) ranking.Badge = RankBadge.Bronze;
        else ranking.Badge = RankBadge.Unranked;
    }

    void Update()
    {
        if (triggerPaymentTest)
        {
            triggerPaymentTest = false;
            TestPayment();
        }
    }

    public async Task Setup(string target, int port, string pubkey, string signature)
    {
        
    }

    public void Shutdown()
    {
        
    }

    void SortHighscores(RankType rankType)
    {
        switch (rankType)
        {
            case RankType.None:
                break;
            case RankType.Global:
                highscores = highscores.OrderBy(p => p.GlobalRanking.Rank).ToArray();
                break;
            case RankType.Kd:
                highscores = highscores.OrderBy(p => p.KdRanking.Rank).ToArray();
                break;
            case RankType.Earnings:
                highscores = highscores.OrderBy(p => p.EarningsRanking.Rank).ToArray();
                break;
            default:
                break;
        }
    }

    public async Task<(Ranking[]rankings, int totalElements)> ListRankings(int length, int startIndex, RankType rankType)
    {
        SortHighscores(rankType);

        if (startIndex >= highscores.Length) return (new Ranking[0],highscores.Length);
        length = Mathf.Min(length,highscores.Length-startIndex);
        Ranking[] ranks = highscores.ToList<Ranking>().GetRange(startIndex, length).ToArray();

        return (ranks, highscores.Length);
    }

    public async Task<SkinInventory> GetSkinInventory()
    {
        var skinInventory = new SkinInventory { EquippedSkin = equippedSkin };
        skinInventory.OwnedSkins.Add(ownedSkins);
        return skinInventory;
    }

    public async Task EquipSkin(string skinId)
    {
        if(!ownedSkins.Contains(skinId))
        {
            equippedSkin = "robot_default";
            throw new Exception("skin not owned!");
        }
        equippedSkin = skinId;
    }

    public Task<ShopSkin[]> GetAllSkins()
    {
        List<ShopSkin> shopSkins = new List<ShopSkin>();
        foreach (var id in allSkins)
        {
            shopSkins.Add(new ShopSkin
            {
                Id = id,
                Price = UnityEngine.Random.Range(1, 100000)
            });
        }
        return Task.FromResult(shopSkins.ToArray());
    }

    public async Task<string> GetSkinInvoice(string skinId)
    {
        Debug.Log("Want to buy skin: " + skinId);
        if(ownedSkins.Contains(skinId))
        {
            throw new Exception( "skin already owned");
        }
        if (!allSkins.Contains(skinId))
        {
            throw new Exception("unknown skin");

        }
        string invoice = "RandomInvoice:" + UnityEngine.Random.Range(0, 100000000);
        activeSkinInvoices[invoice] = skinId;
        return invoice;
    }


    public async Task<string[]> GetAllSkinIds()
    {
        return allSkins;
    }

    public async Task<string> GetUsername()
    {
        return userName;
    }

    public async Task<string> SetUsername(string userName)
    {
        this.userName = userName;
        highscores.FirstOrDefault(h => h.Pubkey == "Pubkey: 0").Name = userName;
        return userName;
    }

    public async Task<string> GetGameVersion()
    {
        await Task.Delay(UnityEngine.Random.Range(100, 1000));
        return GameVersion;
    }

    public async Task<int> GetPlayerRank(string playername, RankType rankType)
    {
        SortHighscores(rankType);
        Ranking ranking = highscores.FirstOrDefault(r => r.Name == playername);
        if (ranking == null) throw new Exception("player with name "+ playername +" not found");
        else
        {
            int id = Array.IndexOf(highscores, ranking);
            return id;
        }
    }

    public Task<bool> NeedsUsernameChange()
    {
        return Task.FromResult(needsUserNameChange);
    }

    public async Task WaitForPayment(string invoice, long expiry, CancellationToken cancellationTokens)
    {
        await Task.Run(() =>
        {
            var startTime = DateTime.Now.ToFileTimeUtc();
            var endTime = DateTime.Now.AddSeconds(expiry).ToFileTimeUtc();
            while (DateTime.Now.ToFileTimeUtc() < endTime)
            {

                if (returnPayment)
                {
                    returnPayment = false;
                    if (!paymentReturnValue)
                        throw new Exception("other error");

                    OnDummyInvoicePaied(invoice);

                    return;
                }
            }
            throw new ExpiredException();
           
        });
    }

    public void OnDummyInvoicePaied(string invoice){
        if (activeSkinInvoices.ContainsKey(invoice))
        {
            ownedSkins.Add(activeSkinInvoices[invoice]);
            activeSkinInvoices.Remove(invoice);
        }
    }

    public async void TestPayment()
    {
        var testInvoice = "testp" + UnityEngine.Random.Range(0, 100000).ToString();
        try
        {
            await WaitForPayment(testInvoice, expiryInSeconds, new CancellationToken());
            Debug.Log(testInvoice + " succeeded");
        } catch(Exception e)
        {
            Debug.Log(testInvoice + " failed: " + e.Message);
        }
    }

    public async Task<Ranking> GetPlayerRanking()
    {
        Ranking r;
        try
        {
            r = GetPlayerRanking(userName);
            return r;
        }
        catch(Exception e)
        {
            throw e;
        }
    }

    private Ranking GetPlayerRanking(string playername)
    {
        Ranking r = highscores.FirstOrDefault(h => h.Name == playername);
        if(r == null)
        {
            throw new Exception("playername doeas not exist!");
        }
        return r;
    }

    public async Task<GetRankingInfoResponse> GetRankingInfo()
    {
        GetRankingInfoResponse res = new GetRankingInfoResponse
        {
            BronzeThreshold = (int)(this.BronzeThreshold * 100 * highscores.Length),
            SilverThreshold = (int)(this.SilverThreshold * 100 * highscores.Length),
            GoldThreshold = (int)(this.GoldThreshold * 100 * highscores.Length),
            TotalPlayers = highscores.Length
        };
        return res;
    }

    public Task<Ranking> GetSpecificPlayerRanking(string pubkey)
    {
        throw new NotImplementedException();
    }
}
