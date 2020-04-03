using Bbh;
using Improbable.Gdk.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeaderboardMenuUI : MonoBehaviour
{
    public LeaderboardEntryUI headLine;
    public Transform entryContainer;
    List<LeaderboardEntryUI> entries;

    Highscore[] highscores;
    string PlayerPubKey;

    HighscoreOrderPriority priority = HighscoreOrderPriority.EARNIGNS;
    bool orderDescending = false;

    // Start is called before the first frame update
    void Start()
    {
        entries = entryContainer.GetComponentsInChildren<LeaderboardEntryUI>().ToList();
        headLine.Set(0, "Player", new List<string> { "earnings", "kills", "deaths", "kills/deaths" });

        int counter = 0;
        foreach (LeaderboardEntryUI entry in entries)
        {
            counter++;
            entry.Set(counter, "player" + counter, new List<string> { Utility.SatsToShortString(Random.Range(0, 130000000), true), Random.Range(0, 100).ToString(), Random.Range(0, 100).ToString(), "something" });
        }

        ClientEvents.instance.onLeaderboardUpdate.AddListener(UpdateLeaderBoard);

    }

    void setEntry(LeaderboardEntryUI entry, Highscore score, long rank)
    {
        entry.Set(rank, score.Name, new List<string> { Utility.SatsToShortString(score.Earnings, true), score.Kills.ToString(), score.Deaths.ToString(), "?" });
    }

    public void UpdateLeaderBoard(LeaderboardUpdateArgs args)
    {
        highscores = args.highscores;
        PlayerPubKey = args.PlayerPubKey;
        UpdateList();
    }

    void UpdateList()
    {
        SortScores();

        for (int i = 0; i < entries.Count; i++)
        {
            if (i < highscores.Length)
            {
                setEntry(entries[i], highscores[i], i);
            }
            else
            {
                entries[i].Hide();
            }
        }
    }

    public void SetOrderPriority(HighscoreOrderPriority priority)
    {
        if (this.priority == priority)
        {
            orderDescending = !orderDescending;
        }
        else
        {
            orderDescending = false;
            this.priority = priority;
        }

    }

    public void SortScores()
    {
        highscores = SortScores(highscores, priority, orderDescending);
    }
    public static Highscore[] SortScores(Highscore[] scores, HighscoreOrderPriority priority, bool descending)
    {
        System.Func<Highscore, long> order;


        switch (priority)
        {
            case HighscoreOrderPriority.RANK:
            default:
                order = o => o.Earnings*o.Kills/o.Deaths;
                break;
            case HighscoreOrderPriority.EARNIGNS:
                order = o => o.Earnings;
                break;
            case HighscoreOrderPriority.KILLS:
                order = o => o.Kills;
                break;
            case HighscoreOrderPriority.DEATHS:
                order = o => o.Deaths;
                break;
            case HighscoreOrderPriority.KDRATIO:
                order = o => o.Kills / o.Deaths;
                break;
        }

        if (descending)
        {
            return scores.OrderByDescending(order).ToArray();
        }
        else
        {
            return scores.OrderBy(order).ToArray();
        }
    }


}

public enum HighscoreOrderPriority
    {
        RANK = 0,
        EARNIGNS = 1,
        KILLS = 2,
        DEATHS = 3,
        KDRATIO = 4
    }
