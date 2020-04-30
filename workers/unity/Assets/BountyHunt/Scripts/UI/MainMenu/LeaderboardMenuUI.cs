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
    public TextMeshProUGUI pageText;

    List<LeaderboardEntryUI> entries;

    Ranking[] highscores;
    string PlayerName;

    bool orderAscending = false;
    int pageSize;
    int currentPageIndex = 0;
    int lastPage;
    bool playervisible = false;

    LeaderBoardSet[] leaderboards;
    LeaderBoardSet selectedLeaderboard;

    private void Awake()
    {
        entries = entryContainer.GetComponentsInChildren<LeaderboardEntryUI>().ToList();
        pageSize = entries.Count;
        firstButton.onClick.AddListener(SetFirstPage);
        meButton.onClick.AddListener(SetMyPage);
        previousButton.onClick.AddListener(SetPreviousPage);
        nextButton.onClick.AddListener(SetNextPage);

        LeaderBoardSet GlobalLeague = new LeaderBoardSet();
        GlobalLeague.name = "Global League";
        GlobalLeague.rankType = RankType.Global;
        GlobalLeague.values.Add(("Global Score", r => r.GlobalRanking.Score.ToString()));
        GlobalLeague.values.Add(("Hunting Rank", r => r.KdRanking.Rank.ToString()));
        GlobalLeague.values.Add(("Looting Rank", r => r.EarningsRanking.Rank.ToString()));

        LeaderBoardSet HuntersLeague = new LeaderBoardSet();
        HuntersLeague.name = "Hunters League";
        HuntersLeague.rankType = RankType.Kd;
        HuntersLeague.values.Add(("Hunter Score", r => r.KdRanking.Score.ToString()));
        HuntersLeague.values.Add(("Kills", r => r.Stats.Kills.ToString()));
        HuntersLeague.values.Add(("Deaths", r => r.Stats.Deaths.ToString()));

        LeaderBoardSet LootersLeague = new LeaderBoardSet();
        LootersLeague.name = "Looters League";
        LootersLeague.rankType = RankType.Earnings;
        LootersLeague.values.Add(("Earnings", r => r.Stats.Earnings.ToString()));

        leaderboards = new LeaderBoardSet[] { GlobalLeague, HuntersLeague, LootersLeague };
        selectedLeaderboard = leaderboards[0];

    }
    // Start is called before the first frame update
    void Start()
    {
        
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
        if (PlayerServiceConnections.instance.ServicesReady)
        {
            Init();
        }
    }

    async void Init()
    {
        try
        {
            PlayerName = await PlayerServiceConnections.instance.BackendPlayerClient.GetUsername();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            PopUpManagerUI.instance.OpenPopUp(new PopUpArgs("Error", e.Message));
        }
        SetLeaderBoard(selectedLeaderboard);
    }

    void setEntry(LeaderboardEntryUI entry, Ranking ranking, long position)
    {
        List<string> strings = selectedLeaderboard.values.Select(e => e.value(ranking)).ToList();

        entry.Set(position, ranking.Name, strings);
    }

    public async void UpdateLeaderBoard()
    {

        (Ranking[] ranks, int count) ranks;
        try
        {
            ranks = await PlayerServiceConnections.instance.BackendPlayerClient.ListRankings(pageSize,currentPageIndex*pageSize,selectedLeaderboard.rankType);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            PopUpManagerUI.instance.OpenPopUp(new PopUpArgs("Error", e.Message));
            return;
        }

        UpdateList(ranks.ranks,currentPageIndex*pageSize);
        lastPage = (int)Mathf.Ceil(ranks.count / pageSize) - 1;
        UpdateNavigationButtons();

    }

    void UpdateList(Ranking[] ranks, int startIndex)
    {
        playervisible = false;

        int i = 0;
        foreach(LeaderboardEntryUI entry in entries)
        {
            if (i < ranks.Length)
            {
                setEntry(entry, ranks[i], startIndex + 1 + i);
                if(ranks[i].Name == PlayerName)
                {
                    entry.Highlight();
                    playervisible = true;
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
            int rank = await PlayerServiceConnections.instance.BackendPlayerClient.GetPlayerRank(PlayerName, selectedLeaderboard.rankType);

            SetPage(rank/pageSize);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            PopUpManagerUI.instance.OpenPopUp(new PopUpArgs("Error", e.Message));
            return;
        }
        
    }
    void SetFirstPage()
    {
        SetPage(0);
    }
    void SetPage(int page)
    {
        page = Mathf.Clamp(page, 0, lastPage);
        currentPageIndex = page;
        UpdateLeaderBoard();
        
    }

    void UpdateNavigationButtons()
    {
        if (currentPageIndex == 0) { previousButton.interactable = false; firstButton.interactable = false; }
        else { previousButton.interactable = true; firstButton.interactable = true; }

        if (currentPageIndex == lastPage) nextButton.interactable = false;
        else nextButton.interactable = true;

        if (playervisible) meButton.interactable = false;
        else meButton.interactable = true;

        pageText.text = (currentPageIndex + 1).ToString();
    }
}

public class LeaderBoardSet{
    public string name;
    public RankType rankType;
    public List<(string name, Func<Ranking, string> value)> values = new List<(string name, Func<Ranking, string> value)>();
}

