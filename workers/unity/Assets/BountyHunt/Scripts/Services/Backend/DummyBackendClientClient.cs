using Bbhrpc;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

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
            int k = Random.Range(killsRange.x, killsRange.y);
            int d = Random.Range(deathsRange.x, deathsRange.y);
            int e = Random.Range(earningsRange.x, earningsRange.y);
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



    public Task<(Ranking[]rankings, int totalElements)> ListRankings(int length, int startIndex, RankType rankType)
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

        if (startIndex >= highscores.Length) return Task.FromResult((new Ranking[0],highscores.Length));
        length = Mathf.Min(length,highscores.Length-startIndex);
        Ranking[] ranks = highscores.ToList<Ranking>().GetRange(startIndex, length).ToArray();

        return Task.FromResult((ranks, highscores.Length));
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
        throw new System.NotImplementedException();
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

    public Task<Ranking[]> GetTop100EarningsRankings()
    {
        throw new System.NotImplementedException();
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
        await Task.Delay(Random.Range(100, 1000));
        return GameVersion;
    }
}
