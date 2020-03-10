using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Bountyhunt;
using Improbable.Gdk.Core;

public class UIBlackBoardItem : MonoBehaviour
{
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI bountyText;
    public TextMeshProUGUI earningsText;
    public TextMeshProUGUI killsText;
    public TextMeshProUGUI deathsText;
    public Image backGroundImage;
    public Image highlightImage;

    //public TextMeshProUGUI lastSeenText;
    //public TextMeshProUGUI costText;
    //public Button teleportButton;

    private RoughPosition teleportPosition;

    public void Awake()
    {
        //teleportButton.onClick.AddListener(TeleportTo);
    }
    
    public void Disable()
    {
        gameObject.SetActive(false);
    }
    public void Enable()
    {
        gameObject.SetActive(true);
        
    }

    public void SetAsPlaceholder()
    {
        rankText.text = "";
        nameText.text = "...";
        bountyText.text = "";
        killsText.text = "";
        deathsText.text = "";

    }

    public void SetText(ScoreboardUIItem item)
    {
        //idText.text = item.Entity.Id.ToString();

        rankText.text = item.rank.ToString();
        nameText.text = item.name;
        bountyText.text = item.item.Bounty.ToString();
        killsText.text = item.item.Kills.ToString();
        deathsText.text = item.item.Deaths.ToString();
        earningsText.text = item.item.Earnings.ToString();

        if (item.highlight)
        {
            highlightImage.gameObject.SetActive(true);
            //backGroundImage.gameObject.SetActive(false);
        }
        else
        {
            highlightImage.gameObject.SetActive(false);
            //backGroundImage.gameObject.SetActive(true);
        }

        //nameText.text = ClientGameStats.instance.idToName(item.Entity);
        //bountyText.text = item.Bounty.ToString();
        //killsText.text = item.Kills.ToString();
        //deathsText.text = item.Deaths.ToString();


        //lastSeenText.text = item.LastSeen.X + ":" + item.LastSeen.Z;
        //costText.text = "~"+DonnerUtils.CalculateTeleportCost(DonnerPlayerAuthorative.instance.transform.position, new Vector3(item.LastSeen.X, DonnerPlayerAuthorative.instance.transform.position.y, item.LastSeen.Z)).ToString() + " sats";
    }


}


public class ScoreboardUIItem
{
    public string name;
    public ScoreboardItem item;
    public bool highlight;
    public int rank;

    public ScoreboardUIItem(string playerName, ScoreboardItem item)
    {
        name = playerName;
        this.item = item;
        highlight = false;
        rank = 0;
    }
}

public struct ScoreboardItem
{
    public EntityId Entity;
    public long Bounty;
    public long Earnings;
    public int Kills;
    public int Deaths;

    public ScoreboardItem(EntityId entity, long bounty, int kills, int deaths , long earnings)
    {
        Entity = entity;
        Bounty = bounty;
        Kills = kills;
        Deaths = deaths;
        Earnings = earnings;
    }
}

