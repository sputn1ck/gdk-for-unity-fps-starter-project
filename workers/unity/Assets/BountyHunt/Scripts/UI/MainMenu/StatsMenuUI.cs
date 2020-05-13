using Bbhrpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StatsMenuUI : MonoBehaviour, IRefreshableUI
{
    public Transform PlayerStatsContainer;
    public Transform GameStatsContainer;
    public StatsValueUI statsValuePrefab;
    Dictionary<string, StatsValueUI> stats = new Dictionary<string, StatsValueUI>();

    public SimpleSliderUI sliderUI;

    public RankingStatsPanelUI GlobalRankingPanel;
    public RankingStatsPanelUI HunterRankingPanel;
    public RankingStatsPanelUI LooterRankingPanel;

    Ranking playerRanking;
    GetRankingInfoResponse rankingInfo;
    GetInfoResponse generalInfo;

    public void show()
    {
        gameObject.SetActive(true);
        Refresh();
    }

    public void hide()
    {
        gameObject.SetActive(false);
    }

    void Start()
    {
        sliderUI.GetSlideButtonEvents(0).onActivate.AddListener(show);
        sliderUI.GetSlideButtonEvents(0).onDeactivate.AddListener(hide);
        AddListeners();

    }

    private void OnEnable()
    {
        Refresh();
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

    void WriteLine(string name, string value, Transform container,string key = "")
    {
        if (key == "") key = name;
        if (!stats.ContainsKey(key))
        {
            stats[key] = Instantiate(statsValuePrefab, container);
        }
        stats[key].Set(name, value);

    }

    

    public async void Refresh()
    {
        if (!PlayerServiceConnections.instance.ServicesReady)
            return;
        try
        {
            playerRanking = await PlayerServiceConnections.instance.BackendPlayerClient.GetPlayerRanking();
            rankingInfo = await PlayerServiceConnections.instance.BackendPlayerClient.GetRankingInfo();
            generalInfo = await PlayerServiceConnections.instance.BackendPlayerClient.GetInfo();
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            var errArgs = new PopUpArgs("Error", e.Message);
            PopUpManagerUI.instance.OpenPopUp(errArgs);
            return;
        }

        RefreshPlayerstats(playerRanking, rankingInfo.TotalPlayers);
        RefreshGameStats();
    }

    void RefreshPlayerstats(Ranking rank,int playerCount)
    {
        Badge badge = BadgeManager.GetBadge(rank.GlobalRanking.Badge);
        GlobalRankingPanel.BadgeImage.sprite = badge.sprite;
        GlobalRankingPanel.BadgeImage.color = badge.color;
        GlobalRankingPanel.stats[0].valueText.text = rank.GlobalRanking.Rank.ToString();
        GlobalRankingPanel.HeadlineExtraValue.text = String.Format("(Top {0}%)", (100 * rank.GlobalRanking.Rank / playerCount));
        GlobalRankingPanel.stats[1].valueText.text = rank.GlobalRanking.Score.ToString();

        badge = BadgeManager.GetBadge(rank.KdRanking.Badge);
        HunterRankingPanel.BadgeImage.sprite = badge.sprite;
        HunterRankingPanel.BadgeImage.color = badge.color;
        HunterRankingPanel.stats[0].valueText.text = rank.KdRanking.Rank.ToString();
        HunterRankingPanel.HeadlineExtraValue.text = String.Format("(Top {0}%)",(100 * rank.KdRanking.Rank / playerCount));
        HunterRankingPanel.stats[1].valueText.text = rank.KdRanking.Score.ToString();
        HunterRankingPanel.stats[2].valueText.text = rank.Stats.Kills.ToString();
        HunterRankingPanel.stats[3].valueText.text = rank.Stats.Deaths.ToString();

        badge = BadgeManager.GetBadge(rank.EarningsRanking.Badge);
        LooterRankingPanel.BadgeImage.sprite = badge.sprite;
        LooterRankingPanel.BadgeImage.color = badge.color;
        LooterRankingPanel.stats[0].valueText.text = rank.EarningsRanking.Rank.ToString();
        LooterRankingPanel.HeadlineExtraValue.text = String.Format("(Top {0}%)", (100 * rank.EarningsRanking.Rank / playerCount)); 
        LooterRankingPanel.stats[1].valueText.text = rank.Stats.Earnings.ToString();

    }

    void RefreshGameStats()
    {
        
        Transform ctr = GameStatsContainer;
        WriteLine("---Player Stats---", "", ctr);
        WriteLine("Total Players", rankingInfo.TotalPlayers.ToString(), ctr);
        WriteLine("---Pool Stats---", "", ctr);
        WriteLine("Donation Pool", Utility.SatsToShortString(generalInfo.PoolInfo.DonationPool,true), ctr);
        WriteLine("Shop Pool", Utility.SatsToShortString(generalInfo.PoolInfo.ShopPool, true), ctr);
    }

}
