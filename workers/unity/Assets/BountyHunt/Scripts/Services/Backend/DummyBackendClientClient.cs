using Bbhrpc;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Threading;
using Lnrpc;

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
    public Vector2Int gameDonationSqrtRange;
    public Vector2Int devsDonationSqrtRange;
    public long playerEarnings;
    public int playerKills;
    public int playerDeaths;
    public long playerDonationsGame;
    public long playerDonationsDevs;
    Ranking[] highscores;

    public float GoldThreshold;
    public float SilverThreshold;
    public float BronzeThreshold;


    [Header("Skins")]
    public string[] allSkins = new string[] { "robot_default", "robot_2" };
    public List<string> ownedSkins = new List<string>{ "robot_default" };
    public string equippedSkin = "robot_default";
    public Dictionary<string, string> activeSkinInvoices = new Dictionary<string, string>();
    List<ShopSkin> shopSkins = new List<ShopSkin>();


    [Header("Payments")]
    public bool paymentReturnValue;
    public bool returnPayment;
    public int expiryInSeconds;
    public bool triggerPaymentTest;

    [Header("Sponsors")]
    [SerializeField]public List<AdvertiserInvestmentInfo> AdvertiserInvestmentInfos;

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
                DonatedGame = playerDonationsGame,
                DonatedDev = playerDonationsDevs

            },
            GlobalRanking = new LeagueRanking(),
            KdRanking = new LeagueRanking
            {
                Score = (int)((((float)playerKills) / (playerKdDeaths)) * 10000)
            },
            EarningsRanking = new LeagueRanking
            {
                Score = playerEarnings
            },
            DonorsRanking = new LeagueRanking
            {
                Score = playerDonationsDevs + playerDonationsGame,
            }
        };
        for (int i = 1; i < highscores.Length; i++)
        {
            int k = UnityEngine.Random.Range(killsRange.x, killsRange.y);
            int d = UnityEngine.Random.Range(deathsRange.x, deathsRange.y);
            int e = UnityEngine.Random.Range(earningsRange.x, earningsRange.y);
            int dg = UnityEngine.Random.Range(0, 2) * (int)Mathf.Pow(UnityEngine.Random.Range(gameDonationSqrtRange.x, gameDonationSqrtRange.y),2);
            int dd = UnityEngine.Random.Range(0, 2) * (int)Mathf.Pow(UnityEngine.Random.Range(devsDonationSqrtRange.x,devsDonationSqrtRange.y),2);

            float kd =(k+5)/(d+5);
            highscores[i] = new Ranking()
            {
                Name = "Player: " + i,
                Pubkey = "Pubkey: " + i,

                Stats = new Stats()
                {
                    Deaths = d,
                    Kills = k,
                    Earnings = e,
                    DonatedGame = dg,
                    DonatedDev = dd
                },

                GlobalRanking = new LeagueRanking(),
                KdRanking = new LeagueRanking
                {
                    Score = (int)(kd * 10000)
                },
                EarningsRanking = new LeagueRanking
                {
                    Score = e
                },
                DonorsRanking = new LeagueRanking
                {
                    Score = dg + dd
                }

            };
        }
        highscores = highscores.OrderByDescending(h => h.KdRanking.Score).ToArray();
        for(int i = 0; i < highscores.Length; i++)
        {
            highscores[i].KdRanking.Rank = i + 1;
        }

        highscores = highscores.OrderByDescending(h => h.DonorsRanking.Score).ToArray();
        for (int i = 0; i < highscores.Length; i++)
        {
            highscores[i].DonorsRanking.Rank = i + 1;
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


        foreach (var id in allSkins)
        {
            shopSkins.Add(new ShopSkin
            {
                Id = id,
                Price = UnityEngine.Random.Range(1, 1000)
            });
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
            case RankType.Donations:
                highscores = highscores.OrderBy(p => p.DonorsRanking.Rank).ToArray();
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

    public async Task<ShopSkin[]> GetAllSkins()
    {
        return shopSkins.ToArray();
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
        long price = 0;
        ShopSkin shopskin = shopSkins.FirstOrDefault(s => s.Id == skinId);
        if (shopskin != null) price = shopskin.Price;

        string invoice = await PlayerServiceConnections.instance.lnd.GetInvoice(price, "Buy skin " + skinId, expiryInSeconds);
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

    public async Task WaitForPayment(string invoice, long timestamp, CancellationToken cancellationTokens)
    {
        await Task.Run(() =>
        {

            while (DateTimeOffset.UtcNow.ToUnixTimeSeconds() < timestamp)
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
            await WaitForPayment(testInvoice, DateTimeOffset.UtcNow.ToUnixTimeSeconds()+expiryInSeconds, new CancellationToken());
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

    public async Task<Ranking> GetSpecificPlayerRanking(string pubkey)
    {
        string rndname = "random" + UnityEngine.Random.Range(100, 1000);
        RankBadge globalBadge = (RankBadge)UnityEngine.Random.Range(0,Enum.GetNames(typeof(RankBadge)).Length);
        LeagueRanking globalRanking = new LeagueRanking { Badge = globalBadge };

        Ranking ranking = new Ranking { Name = rndname, GlobalRanking = globalRanking };

        return ranking;

    }

    public async Task<string> GetDonationInvoice(long gameDonation, long devsDonation)
    {
        string invoice = await PlayerServiceConnections.instance.lnd.GetInvoice(gameDonation + devsDonation, "Donation " + gameDonation + "/" + devsDonation, expiryInSeconds);

        return invoice;
    }

    public async Task<Bbhrpc.GetInfoResponse> GetInfo()
    {
        await Task.Delay(UnityEngine.Random.Range(50, 500));
        return new Bbhrpc.GetInfoResponse
        {
            GameInfo = new GameInfo
            {
                GameVersion = "alpha-1"
            },
            LndInfo = new LndInfo
            {
                LndHost = "lndhost",
                LndListenPort = 1337,
                LndPubkey = "backend-pubkey"
            },
            SponsorFeeInfo = new SponsorFeeInfo
            {
                ActivationFeeRate = 500,
                CreationCost = 1000,
                PlayerSatoshiCost = 2
            },
            PoolInfo = new PoolInfo
            {
                DonationPayoutPercentage = 0.5,
                DonationPool = UnityEngine.Random.Range(100, int.MaxValue),
                ShopPayoutPercentage = 0.01,
                ShopPool = UnityEngine.Random.Range(100, int.MaxValue),
            }
        };
    }

    public Task<ListAdvertiserResponse> ListAdvertisers()
    {

        var res = new ListAdvertiserResponse();

        foreach (AdvertiserInvestmentInfo aii in AdvertiserInvestmentInfos)
        {
            var advertiser = new Bbhrpc.Advertiser()
            {
                Name = aii.Name,
                Balance = aii.Investment,
                Phash = aii.Hash,
                Url = aii.Url
            };
            foreach(string url in aii.SquareImageUrls)
            {
                advertiser.PicUrls.Add(url);
            }
            res.Advertisers.Add(advertiser);
        }

        return Task.FromResult(res);
    }

    public async Task<string> GetPlayerSatsInvoice(string pHash, long psats)
    {
        AdvertiserInvestmentInfo ai = new AdvertiserInvestmentInfo { Hash = pHash };
        Advertiser advertiser = await PlayerServiceConnections.instance.AdvertiserStore.GetAdvertiser(ai);

        string invoice = await PlayerServiceConnections.instance.lnd.GetInvoice(psats*2, advertiser.name + " " + psats, expiryInSeconds);

        return invoice;
    }

    public Task<string> GetBountyInvoice(string pubkey, long amount)
    {
        return Task.FromResult("bountyinvoice");
    }
}
