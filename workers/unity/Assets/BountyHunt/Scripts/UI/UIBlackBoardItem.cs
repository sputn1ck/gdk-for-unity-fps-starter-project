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
    public TextMeshProUGUI killsText;
    public TextMeshProUGUI deathsText;
    public Image backGround;
    Color defaultBackgroundColor;

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

        if (item.highlight) backGround.color = ScoreboardUI.instance.BlackBoardItemColorHighlight;
        else backGround.color = defaultBackgroundColor;

        //nameText.text = ClientGameStats.instance.idToName(item.Entity);
        //bountyText.text = item.Bounty.ToString();
        //killsText.text = item.Kills.ToString();
        //deathsText.text = item.Deaths.ToString();


        //lastSeenText.text = item.LastSeen.X + ":" + item.LastSeen.Z;
        //costText.text = "~"+DonnerUtils.CalculateTeleportCost(DonnerPlayerAuthorative.instance.transform.position, new Vector3(item.LastSeen.X, DonnerPlayerAuthorative.instance.transform.position.y, item.LastSeen.Z)).ToString() + " sats";
    }

    public void setDefaultBackgroundColor(Color c)
    {
        defaultBackgroundColor = c;
        backGround.color = defaultBackgroundColor;
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
    public int Kills;
    public int Deaths;

    public ScoreboardItem(EntityId entity, long bounty, int kills, int deaths)
    {
        Entity = entity;
        Bounty = bounty;
        Kills = kills;
        Deaths = deaths;
    }
}

