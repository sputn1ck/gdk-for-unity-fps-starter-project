using Bbhrpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StatsMenuUI : MonoBehaviour
{
    public Transform PlayerStatsContainer;
    public Transform GameStatsContainer;
    public StatsValueUI statsValuePrefab;
    Dictionary<string, StatsValueUI> stats = new Dictionary<string, StatsValueUI>();

    public List<Badge> badges;
    public Image badgeImage;

    private void Awake()
    {
        badges = badges.OrderBy(o => o.maxPercantageAbove).ToList();
        ClientEvents.instance.onLeaderboardUpdate.AddListener(UpdateBadge);
    }


    void Start()
    {
        WritePlayerStats();
        WriteGameStats();
        AddListeners();
    }

    void WritePlayerStats()
    {
        Transform ctr = PlayerStatsContainer;

        NewLine("Rank", "0", ctr);
        NewLine("Relative Ranking", "0", ctr);
        NewLine("League", "0", ctr);
        NewLine("Lifetime Kills", "0", ctr);
        NewLine("Lifetime Earnings", "0", ctr);
        NewLine("Lifetime Deaths", "0", ctr);

    }

    void WriteGameStats()
    {
        Transform ctr = GameStatsContainer;

        NewLine("Total Players Lifetime Kills", "0", ctr);
        NewLine("Total Players Lifetime Earnings", "0", ctr);
    }

    void AddListeners()
    {
        /*
        //player
        ClientEvents.instance.onBalanceUpdate.AddListener(onBalanceUpdate);
        ClientEvents.instance.onBountyUpdate.AddListener(onBountyUpdate);
        ClientEvents.instance.onSessionEarningsUpdate.AddListener(onEarningsUpdate);

        ClientEvents.instance.onPlayerKillsAndDeathsUpdate.AddListener(onPlayerKillsAndDeathsUpdate);

        ClientEvents.instance.onPlayerLifeTimeKillsUpdate.AddListener(onPlayerLifeTimeKillsUpdate);
        ClientEvents.instance.onPlayerLifeTimeDeathsUpdate.AddListener(onPlayerLifeTimeDeathsUpdate);
        ClientEvents.instance.onPlayerLifeTimeEarningsUpdate.AddListener(onPlayerLifeTimeEarningsUpdate);

        //game
        ClientEvents.instance.onGlobalBountyUpdate.AddListener(onGlobalBountyUpdate);
        ClientEvents.instance.onGlobalLootUpdate.AddListener(onGlobalLootUpdate);
        ClientEvents.instance.onGlobalPotUpdate.AddListener(onGlobalPotUpdate);

        ClientEvents.instance.onAllTimeKillsUpdate.AddListener(onAllTimeKillsUpdate);
        ClientEvents.instance.onAllTimeMostKillsUpdate.AddListener(onAllTimeMostKillsUpdate);

        ClientEvents.instance.onAllTimeDeathsUpdate.AddListener(onAllTimeDeathsUpdate);
        ClientEvents.instance.onAllTimeMostDeathsUpdate.AddListener(onAllTimeMostDeathsUpdate);

        ClientEvents.instance.onAllTimeEarningsUpdate.AddListener(onAllTimeEarningsUpdate);
        ClientEvents.instance.onAllTimeMostEarningsUpdate.AddListener(onAllTimeMostEarningsUpdate);
        */
    }

    void NewLine(string name, string value, Transform container,string key = "")
    {
        if (key == "") key = name;
        stats[key] = Instantiate(statsValuePrefab, container);
        stats[key].Set(name, value);
    }

    void UpdateBadge(LeaderboardUpdateArgs args)
    {
        Ranking[] scores = args.highscores.OrderByDescending(o => o.Earnings).ToArray();
        int playerRank = Array.FindIndex(scores, o => o.Pubkey == args.PlayerPubKey);
        float factor = (float)playerRank / (float)scores.Length;

        Badge badge = badges[badges.Count - 1];

        foreach (Badge b in badges)
        {
            if (factor*100 <= b.maxPercantageAbove)
            {
                badge = b;
                break;
            }
        }

        badgeImage.sprite = badge.sprite;

        stats["Rank"].UpdateValue((playerRank+1).ToString());
        stats["Relative Ranking"].UpdateValue((int)(factor * 100) + "%");
        stats["League"].UpdateValue(badge.League);

    }

}

[System.Serializable]
public class Badge
{
    public string League;
    public Sprite sprite;
    [Range(0, 100)]
    public int maxPercantageAbove;
}
