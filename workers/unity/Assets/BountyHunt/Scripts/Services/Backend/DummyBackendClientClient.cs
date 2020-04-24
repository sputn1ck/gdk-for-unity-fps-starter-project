using Bbhrpc;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public class DummyBackendClientClient : MonoBehaviour, IBackendPlayerClient
{
    public string GameVersion;
    public string getUsernameResponse;
    public string setUsernameResponse;
    public string userName;

    [Header("Rankings")]
    public int HighscoresCount;
    public Vector2Int earningsRange;
    public Vector2Int killsRange;
    public Vector2Int deathsRange;
    public long playerEarnings;
    public int playerKills;
    public int playerDeaths;
    Ranking[] highscores;

    public string[] allSkins = new string[] { "robot_default", "robot_2" };
    public List<string> ownedSkins = new List<string>{ "robot_default" };
    public string equippedSkin = "robot_default";
    // Update is called once per frame

    private void Awake()
    {
        highscores = new Ranking[this.HighscoresCount + 1];
        highscores[0] = new Ranking()
        {
            Deaths = playerDeaths,
            Kills = playerKills,
            Earnings = playerEarnings,
            Name = userName,
            Pubkey = "Pubkey: 0"
        };
        for (int i = 1; i < highscores.Length; i++)
        {
            int k = UnityEngine.Random.Range(killsRange.x, killsRange.y);
            int d = UnityEngine.Random.Range(deathsRange.x, deathsRange.y);
            int e = UnityEngine.Random.Range(earningsRange.x, earningsRange.y);
            int kd = (k + 1) / (d + 1);
            highscores[i] = new Ranking()
            {
                Deaths = d,
                Kills = k,
                Earnings = e,
                Name = "Player: " + i,
                Pubkey = "Pubkey: " + i,
                KDRanking = kd
            };
        }
    }
    void Update()
    {
        
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
                break;
            case RankType.Kd:
                highscores = highscores.OrderByDescending(p => p.KDRanking).ToArray();
                break;
            case RankType.Earnings:
                highscores = highscores.OrderByDescending(p => p.Earnings).ToArray();
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

    public void EquipSkin(string skinId)
    {
        if(!ownedSkins.Contains(skinId))
        {
            equippedSkin = "robot_default";
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
                Price = Random.Range(1, 100000)
            });
        }
        return Task.FromResult(shopSkins.ToArray());
    }

    public async Task<string> GetSkinInvoice(string skinId)
    {
        Debug.Log("Want to buy skin: " + skinId);
        if(ownedSkins.Contains(skinId))
        {
            return "already owned";
        }
        if (!allSkins.Contains(skinId))
        {
            return "unknown skin";
        }
        ownedSkins.Add(skinId);
        return "invoice";
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
        highscores[0].Name = userName;
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
}
