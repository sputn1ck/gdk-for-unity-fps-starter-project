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

    public Task<Ranking[]> ListRankings()
    {
        return Task.FromResult(GetHighscores());
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
}
