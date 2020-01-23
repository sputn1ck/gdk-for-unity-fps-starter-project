using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using Improbable.Gdk.Core;
using Bountyhunt;
using Fps.Movement;

public class ScoreboardUI : ScreenUI
{
    public List<UIBlackBoardItem> uIBlackBoardItems;
    public static ScoreboardUI instance;
    public ScoreBoardSorting sorting = ScoreBoardSorting.BOUNTY;
    public UIBlackBoardItem playerItem;
    List<ScoreboardItem> ScoreboardItems;

    public Toggle sortBtnBounty;
    public Toggle sortBtnKills;
    public Toggle sortBtnDeaths;

    EntityId playerID;

    void OnPlayerspawn(GameObject g)
    {
        playerID = g.GetComponent < FpsDriver> ().getEntityID();
    }

    private void Start()
    {
        ClientEvents.instance.onPlayerSpawn.AddListener(OnPlayerspawn);
        ClientEvents.instance.onScoreboardUpdate.AddListener(UpdateScoreboard);
        for (int i = 0; i < uIBlackBoardItems.Count; i++)
        {
            if (i % 2 == 0) uIBlackBoardItems[i].backGround.gameObject.SetActive(false);
            else uIBlackBoardItems[i].backGround.gameObject.SetActive(true);
        }

        sortBtnBounty.onValueChanged.AddListener(SetSortingToBounty);
        sortBtnKills.onValueChanged.AddListener(SetSortingToKills);
        sortBtnDeaths.onValueChanged.AddListener(SetSortingToDeaths);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void UpdateScoreboard(List<ScoreboardItem> update)
    {
        ScoreboardItems = update;
        RefreshScoreBoard();
    }

    public void UpdateSorting(ScoreBoardSorting sorting)
    {
        this.sorting = sorting;
        RefreshScoreBoard();
    }

    void SetSortingToBounty(bool value)
    {
        if (value)
        {
            UpdateSorting(ScoreBoardSorting.BOUNTY);
        }
    }

    void SetSortingToKills(bool value)
    {
        if (value)
        {
            UpdateSorting(ScoreBoardSorting.KILLS);
        }
    }

    void SetSortingToDeaths(bool value)
    {
        if (value)
        {
            UpdateSorting(ScoreBoardSorting.DEATHS);
        }
    }


    public void RefreshScoreBoard()
    {
        switch (sorting)
        {
            case ScoreBoardSorting.BOUNTY:
                ScoreboardItems = ScoreboardItems.OrderByDescending(o => o.Bounty).ToList();
                break;
            case ScoreBoardSorting.KILLS:
                ScoreboardItems = ScoreboardItems.OrderByDescending(o => o.Kills).ToList();
                break;
            case ScoreBoardSorting.DEATHS:
                ScoreboardItems = ScoreboardItems.OrderByDescending(o => o.Deaths).ToList();
                break;
            default:
                break;
        }
        
        ScoreboardItem playerBBI = ScoreboardItems.Find(x => x.Entity.Id == playerID.Id);
        playerItem.SetText(playerBBI);

        int i = 0;
        foreach (var item in ScoreboardItems)
        {
            if (i == uIBlackBoardItems.Count) break;
            uIBlackBoardItems[i].Enable();
            uIBlackBoardItems[i].SetText(item);
            i++;
        }
        for (int j = i; j < uIBlackBoardItems.Count; j++)
        {
            uIBlackBoardItems[j].Disable();
        }
    }



}

public enum ScoreBoardSorting
{
    BOUNTY,KILLS,DEATHS
}
