using Bbhrpc;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class DummyBackendClientClient : MonoBehaviour, IBackendPlayerClient
{
    public string getUsernameResponse;
    public string setUsernameResponse;

    public long HighscoreAmount;

    public string[] allSkins = new string[] { "robot_default", "robot_2" };
    public List<string> ownedSkins = new List<string>{ "robot_default" };
    public string equippedSkin = "robot_default";
    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(string target, int port, string pubkey, string signature)
    {
        
    }

    public void Shutdown()
    {
        
    }

    public Task<string> GetUsername()
    {
        return Task.FromResult(this.getUsernameResponse);
    }

    public Task<string> SetUsername()
    {
        return Task.FromResult(this.setUsernameResponse);
    }


    Ranking[] GetHighscores()
    {
        var highscores =new Ranking[this.HighscoreAmount];
        for(int i = 0; i < highscores.Length;i++)
        {
            highscores[i] = new Ranking()
            {
                Deaths = Random.Range(0, 1000),
                Kills = Random.Range(0, 1000),
                Earnings = Random.Range(0, 1000000),
                Name = "Player: " + Random.Range(1, int.MaxValue),
                Pubkey = "Pubkey: " + Random.Range(1, int.MaxValue),
           
            };
        }
        return highscores;
    }

    public Task<Ranking[]> ListRankings(int length, int startIndex, RankType rankType)
    {
        return Task.FromResult(GetHighscores());
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

    public Task<string> SetUsername(string userName)
    {
        throw new System.NotImplementedException();
    }
}
