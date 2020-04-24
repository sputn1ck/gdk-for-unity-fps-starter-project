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
    public Button firstButton;
    public Button meButton;
    public Button previousButton;
    public Button nextButton;

    public SimpleSliderUI sliderUI;

    List<LeaderboardEntryUI> entries;

    Ranking[] highscores;
    string PlayerName;

    RankType priority = RankType.Earnings;
    bool orderAscending = false;
    int pageSize;
    int currentPageIndex = 0;

    LeaderBoardSet[] leaderboards;
    LeaderBoardSet selectedLeaderboard;

    // Start is called before the first frame update
    void Start()
    {
        
        entries = entryContainer.GetComponentsInChildren<LeaderboardEntryUI>().ToList();
        pageSize = entries.Count;
        firstButton.onClick.AddListener(SetFirstPage);
        meButton.onClick.AddListener(SetMyPage);
        previousButton.onClick.AddListener(SetPreviousPage);
        nextButton.onClick.AddListener(SetNextPage);

        LeaderBoardSet GlobalLeague = new LeaderBoardSet();
        GlobalLeague.name = "Global League";
        GlobalLeague.values.Add(("Score", r => r.Ranking_.ToString()));
        GlobalLeague.values.Add(("Hunting Rank", r => r.KDRanking.ToString()));
        GlobalLeague.values.Add(("Looting Rank", r => r.EarningsRanking.ToString()));

        LeaderBoardSet HuntersLeague = new LeaderBoardSet();
        HuntersLeague.name = "Hunters League";
        HuntersLeague.values.Add(("Hunter Score", r => ((r.Kills+1)/(r.Deaths+1)).ToString()));
        HuntersLeague.values.Add(("Kills", r => r.Kills.ToString()));
        HuntersLeague.values.Add(("Deaths", r => r.Deaths.ToString()));

        LeaderBoardSet LootersLeague = new LeaderBoardSet();
        LootersLeague.name = "Looters League";
        LootersLeague.values.Add(("Earnings", r => r.Earnings.ToString()));

        leaderboards = new LeaderBoardSet[] { GlobalLeague, HuntersLeague, LootersLeague };

        sliderUI.GetSlideButtonEvents(0).onActivate.AddListener(() => gameObject.SetActive(false));
        for (int i = 1; i<sliderUI.buttons.Count; i++)
        {
            Button btn = sliderUI.buttons[i];
            int j = i - 1;
            if (j < leaderboards.Length)
            {
                btn.GetComponentInChildren<TextMeshProUGUI>().text = leaderboards[j].name;
                sliderUI.buttonEvents[btn].onActivate.AddListener(() => SetLeaderBoard(leaderboards[j]));
                sliderUI.buttonEvents[btn].onActivate.AddListener(() => gameObject.SetActive(true));
            }
            else
            {
                btn.gameObject.SetActive(false);
            }
        }

        gameObject.SetActive(false);
    }
    void OnEnable()
    {
        Init();
    }

    async void Init()
    {
        try
        {
            PlayerName = await PlayerServiceConnections.instance.BackendPlayerClient.GetUsername();
            SetLeaderBoard(leaderboards[0]);
        }
        catch (Exception e)
        {
            ClientEvents.instance.onPopUp.Invoke(new PopUpEventArgs("Error", e.Message));
        }
    }

    void setEntry(LeaderboardEntryUI entry, Ranking ranking, long position)
    {
        List<string> strings = selectedLeaderboard.values.Select(e => e.value(ranking)).ToList();

        entry.Set(position, ranking.Name, strings);
    }

    public async void UpdateLeaderBoard()
    {
        try
        {
            (Ranking[] ranks,int count) = await PlayerServiceConnections.instance.BackendPlayerClient.ListRankings(pageSize,currentPageIndex*pageSize,priority);
            UpdateList(ranks,currentPageIndex*pageSize);
        }
        catch (Exception e)
        {
            ClientEvents.instance.onPopUp.Invoke(new PopUpEventArgs("Error", e.Message));
        }
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

    void SetLeaderBoard(LeaderBoardSet set)
    {
        List<string> strings = set.values.Select(e => e.name).ToList();
        headLine.Set(0, "Player", strings);
        selectedLeaderboard = set;
        UpdateLeaderBoard(); ;
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
    async void SetMyPage()
    {
        try
        {
            int rank = await PlayerServiceConnections.instance.BackendPlayerClient.GetPlayerRank(PlayerName, priority);

            SetPage(rank/pageSize);
        }
        catch (Exception e)
        {
            ClientEvents.instance.onPopUp.Invoke(new PopUpEventArgs("Error", e.Message));
            return;
        }
        
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
                Stats = new Stats()
                {
                    Earnings = UnityEngine.Random.Range(0, 130000000),
                    Kills = UnityEngine.Random.Range(0, 200),
                    Deaths = UnityEngine.Random.Range(0, 200)
                },
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

public class LeaderBoardSet{
    public string name;
    public List<(string name, Func<Ranking, string> value)> values = new List<(string name, Func<Ranking, string> value)>();
}

