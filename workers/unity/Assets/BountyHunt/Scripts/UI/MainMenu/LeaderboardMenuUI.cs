using Bbhrpc;
using Improbable.Gdk.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardMenuUI : MonoBehaviour
{
    public LeaderboardEntryUI headLine;
    public Transform entryContainer;
    public SimpleSliderUI slideLine;

    public Button firstButton;
    public Button meButton;
    public Button previousButton;
    public Button nextButton;


    List<LeaderboardEntryUI> entries;

    Ranking[] highscores;
    string PlayerName;

    RankType priority = RankType.Earnings;
    bool orderAscending = false;
    int pageSize;
    int currentPageIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        
        entries = entryContainer.GetComponentsInChildren<LeaderboardEntryUI>().ToList();
        pageSize = entries.Count;
        headLine.Set(0, "Player", new List<string> { "earnings", "kills", "deaths", "k/d" });
        slideLine.GetSlideButtonEvents(0).onClick.AddListener( () => SetPriority(RankType.Earnings));
        //slideLine.GetSlideButtonEvents(1).onClick.AddListener(() => priority = RankType.Kills ));
        //slideLine.GetSlideButtonEvents(2).onClick.AddListener(() => priority = RankType.Deaths));
        slideLine.GetSlideButtonEvents(3).onClick.AddListener(() => SetPriority(RankType.Kd)) ;

        firstButton.onClick.AddListener(SetFirstPage);
        meButton.onClick.AddListener(SetMyPage);
        previousButton.onClick.AddListener(SetPreviousPage);
        nextButton.onClick.AddListener(SetNextPage);

        /*
        int counter = 0;
        foreach (LeaderboardEntryUI entry in entries)
        {
            counter++;
            entry.Set(counter, "player" + counter, new List<string> { Utility.SatsToShortString(UnityEngine.Random.Range(0, 130000000), true), UnityEngine.Random.Range(0, 100).ToString(), UnityEngine.Random.Range(0, 100).ToString(), "something" });
        }
        */
        UpdateLeaderBoard();
    }

    void setEntry(LeaderboardEntryUI entry, Ranking score, long position)
    {
        entry.Set(position, score.Name, new List<string> { Utility.SatsToShortString(score.Earnings, true), score.Kills.ToString(), score.Deaths.ToString(), score.KDRanking.ToString() });
    }

    public async void UpdateLeaderBoard()
    {
        PlayerName = await PlayerServiceConnections.instance.BackendPlayerClient.GetUsername();
        (Ranking[] ranks,int count) = await PlayerServiceConnections.instance.BackendPlayerClient.ListRankings(pageSize,currentPageIndex*pageSize,priority);
        UpdateList(ranks,currentPageIndex*pageSize);
    }

    void UpdateList(Ranking[] ranks, int startIndex)
    {
        int i = 0;
        foreach(LeaderboardEntryUI entry in entries)
        {
            if (i < ranks.Length)
            {
                setEntry(entry, ranks[i], startIndex + 1 + i);
                if(ranks[i].Name == PlayerName)
                {
                    entry.Highlight();
                }
                
            }
            else
            {
                entry.Hide();
            }

            i++;
        }

    }

    void SetPriority(RankType type)
    {
        priority = type;
        UpdateLeaderBoard();
    }

    void SetPreviousPage()
    {
        SetPage(Mathf.Max(currentPageIndex -1,0));
    }
    void SetNextPage()
    {
        SetPage(currentPageIndex + 1);
    }
    void SetMyPage()
    {
        Debug.LogError("implementation missing");
    }
    void SetFirstPage()
    {
        SetPage(0);
    }
    void SetPage(int page)
    {
        currentPageIndex = page;
        UpdateLeaderBoard();
    }



    //TEST

    public int testPlayers = 20;
    public bool test;

    void onTest()
    {
        test = false;

        List<Ranking> hs = new List<Ranking>();
        for (int i = 0; i < testPlayers; i++)
        {
            hs.Add(new Ranking
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

