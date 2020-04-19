using Bbhrpc;
using System.Threading.Tasks;
using UnityEngine;

public class DummyBackendClientClient : MonoBehaviour, IBackendPlayerClient
{
    public string getUsernameResponse;
    public string setUsernameResponse;

    public long HighscoreAmount;
    
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

    public Task<string> GetUsername(string pubkey)
    {
        return Task.FromResult(this.getUsernameResponse);
    }

    public Task<string> SetUsername(string pubkey, string userName)
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

    public Task<SkinInventory> GetSkinInventory()
    {
        throw new System.NotImplementedException();
    }

    public void EquipSkin(string skinId)
    {
        throw new System.NotImplementedException();
    }

    public Task<ShopSkin[]> GetAllSkins()
    {
        throw new System.NotImplementedException();
    }

    public Task<string> GetSkinInvoice(string skinId)
    {
        throw new System.NotImplementedException();
    }
}
