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
    List<ScoreboardUIItem> ScoreboardUIItems;

    bool reverseSorting = false;

    public Toggle sortBtnBounty;
    public Toggle sortBtnKills;
    public Toggle sortBtnDeaths;

    EntityId playerID;

    protected override void Awake()
    {
        
        base.Awake();
        instance = this;


        ClientEvents.instance.onPlayerSpawn.AddListener(OnPlayerspawn);
        ClientEvents.instance.onScoreboardUpdate.AddListener(UpdateScoreboard);
    }


    void OnPlayerspawn(GameObject g)
    {
        playerID = g.GetComponent < FpsDriver> ().getEntityID();
    }

    private void Start()
    {

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

    public void UpdateScoreboard(List<ScoreboardUIItem> update, EntityId playerID)
    {
        this.playerID = playerID;  
        ScoreboardUIItems = update;
        RefreshScoreBoard();
    }

    public void UpdateSorting(ScoreBoardSorting sorting)
    {
        if (sorting == this.sorting) reverseSorting = !reverseSorting;
        else reverseSorting = false;

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

        if (!reverseSorting)
        {
            switch (sorting)
            {
                case ScoreBoardSorting.BOUNTY:
                    ScoreboardUIItems = ScoreboardUIItems.OrderByDescending(o => o.item.Bounty).ToList();
                    break;
                case ScoreBoardSorting.KILLS:
                    ScoreboardUIItems = ScoreboardUIItems.OrderByDescending(o => o.item.Kills).ToList();
                    break;
                case ScoreBoardSorting.DEATHS:
                    ScoreboardUIItems = ScoreboardUIItems.OrderByDescending(o => o.item.Deaths).ToList();
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (sorting)
            {
                case ScoreBoardSorting.BOUNTY:
                    ScoreboardUIItems = ScoreboardUIItems.OrderBy(o => o.item.Bounty).ToList();
                    break;
                case ScoreBoardSorting.KILLS:
                    ScoreboardUIItems = ScoreboardUIItems.OrderBy(o => o.item.Kills).ToList();
                    break;
                case ScoreBoardSorting.DEATHS:
                    ScoreboardUIItems = ScoreboardUIItems.OrderBy(o => o.item.Deaths).ToList();
                    break;
                default:
                    break;
            }
        }

        for (int i = 0;i < ScoreboardUIItems.Count; i++)
        {
            ScoreboardUIItems[i].rank=i + 1;
        }
        
        int topListLength = uIBlackBoardItems.Count;
        ScoreboardUIItem playerBBI = ScoreboardUIItems.Find(x => x.item.Entity.Id == playerID.Id);
        if (playerBBI != null)
        {
            playerBBI.highlight = true;
            int playerRank = ScoreboardUIItems.IndexOf(playerBBI);

            if(playerRank >= uIBlackBoardItems.Count)
            {
                UIBlackBoardItem bbItem;

                if(playerRank == ScoreboardUIItems.Count - 1)
                {
                    topListLength = uIBlackBoardItems.Count - 3;

                    bbItem = uIBlackBoardItems[uIBlackBoardItems.Count - 3];
                    bbItem.Enable();
                    bbItem.SetAsPlaceholder();

                    bbItem = uIBlackBoardItems[uIBlackBoardItems.Count - 2];
                    bbItem.Enable();
                    bbItem.SetText(ScoreboardUIItems[playerRank-1]);

                    bbItem = uIBlackBoardItems[uIBlackBoardItems.Count - 1];
                    bbItem.Enable();
                    bbItem.SetText(playerBBI);
                }

                else
                {

                    topListLength = uIBlackBoardItems.Count - 4;

                    bbItem = uIBlackBoardItems[uIBlackBoardItems.Count - 4];
                    bbItem.Enable();
                    bbItem.SetAsPlaceholder();

                    bbItem = uIBlackBoardItems[uIBlackBoardItems.Count - 3];
                    bbItem.Enable();
                    bbItem.SetText(ScoreboardUIItems[playerRank - 1]);

                    bbItem = uIBlackBoardItems[uIBlackBoardItems.Count - 2];
                    bbItem.Enable();
                    bbItem.SetText(playerBBI);

                    bbItem = uIBlackBoardItems[uIBlackBoardItems.Count - 1];
                    bbItem.Enable();
                    bbItem.SetText(ScoreboardUIItems[playerRank + 1]);
                }

            }
        
        }

        for(int i = 0; i < topListLength; i++) {
            if(i>=ScoreboardUIItems.Count)
            {
                uIBlackBoardItems[i].Disable();
            }
            else
            {
                
                uIBlackBoardItems[i].Enable();
                uIBlackBoardItems[i].SetText(ScoreboardUIItems[i]);
            }
        }

    }



}

public enum ScoreBoardSorting
{
    BOUNTY,KILLS,DEATHS
}
