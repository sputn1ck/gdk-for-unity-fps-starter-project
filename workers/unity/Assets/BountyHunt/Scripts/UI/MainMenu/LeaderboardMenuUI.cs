using Bbh;
using Improbable.Gdk.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LeaderboardMenuUI : MonoBehaviour
{
    public LeaderboardEntryUI headLine;
    public Transform entryContainer;
    public SimpleSliderUI slideLine;
    List<LeaderboardEntryUI> entries;

    Highscore[] highscores;
    string PlayerPubKey;

    HighscoreOrderPriority priority = HighscoreOrderPriority.EARNIGNS;
    bool orderAscending = false;

    // Start is called before the first frame update
    void Start()
    {
        entries = entryContainer.GetComponentsInChildren<LeaderboardEntryUI>().ToList();
        headLine.Set(0, "Player", new List<string> { "earnings", "kills", "deaths", "k/d" });
        slideLine.GetSlideButtonEvents(0).onClick.AddListener( () => SetOrderPriority(HighscoreOrderPriority.EARNIGNS));
        slideLine.GetSlideButtonEvents(1).onClick.AddListener(() => SetOrderPriority(HighscoreOrderPriority.KILLS));
        slideLine.GetSlideButtonEvents(2).onClick.AddListener(() => SetOrderPriority(HighscoreOrderPriority.DEATHS));
        slideLine.GetSlideButtonEvents(3).onClick.AddListener(() => SetOrderPriority(HighscoreOrderPriority.KDRATIO));


        int counter = 0;
        foreach (LeaderboardEntryUI entry in entries)
        {
            counter++;
            entry.Set(counter, "player" + counter, new List<string> { Utility.SatsToShortString(UnityEngine.Random.Range(0, 130000000), true), UnityEngine.Random.Range(0, 100).ToString(), UnityEngine.Random.Range(0, 100).ToString(), "something" });
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

        int playerID = Array.FindIndex(highscores, h => h.Pubkey == PlayerPubKey);

        int occupiedSlots = 0;
        if(playerID  >= entries.Count)
        {
            if (playerID == highscores.Length - 1)
            {
                occupiedSlots = 3;
            }
            else occupiedSlots = 4;
        }


        for (int i = 0; i < entries.Count - occupiedSlots; i++)
        {
            if (i < highscores.Length)
            {
                setEntry(entries[i], highscores[i], i+1);
            }
            else
            {
                entries[i].Hide();
            }
        }

        if (occupiedSlots > 0)
        {
            int eID = entries.Count - occupiedSlots;
            int pID = playerID - 2;
            entries[eID].Hide();
            for(int i = 1; i < occupiedSlots;i++)
            {
                setEntry(entries[eID+i],highscores[pID+i], pID+i+1);
            }
        }

    }

    public void SetOrderPriority(HighscoreOrderPriority priority)
    {
        if (this.priority == priority)
        {
            orderAscending = !orderAscending;
        }
        else
        {
            orderAscending = false;
            this.priority = priority;
        }
        UpdateList();
    }

    public void SortScores()
    {
        highscores = SortScores(highscores, priority, !orderAscending);
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


    //TEST

    public int testPlayers = 20;
    public bool test;

    void onTest()
    {
        test = false;

        List<Highscore> hs = new List<Highscore>();
        for (int i = 0; i < testPlayers; i++)
        {
            hs.Add(new Highscore
            {
                Pubkey = "fakeKey" + i,
                Name = "fakePlayer_" + i,
                Earnings = UnityEngine.Random.Range(0, 130000000),
                Kills = UnityEngine.Random.Range(0,200),
                Deaths = UnityEngine.Random.Range(0, 200)
            }) ;
        }

        LeaderboardUpdateArgs args = new LeaderboardUpdateArgs { highscores = hs.ToArray(), PlayerPubKey = "fakeKey0"};

        ClientEvents.instance.onLeaderboardUpdate.Invoke(args);
    }

    private void Update()
    {
        if (Time.time>0.1f && test) onTest();
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

