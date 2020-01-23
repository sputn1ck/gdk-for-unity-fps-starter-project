using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Bountyhunt;

public class UIBlackBoardItem : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI bountyText;
    public TextMeshProUGUI killsText;
    public TextMeshProUGUI deathsText;
    public Image backGround;

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

    public void SetText(ScoreboardItem item)
    {
        //idText.text = item.Entity.Id.ToString();
        nameText.text = ClientGameStats.instance.idToName(item.Entity);
        bountyText.text = item.Bounty.ToString();
        killsText.text = item.Kills.ToString();
        deathsText.text = item.Deaths.ToString();
        //lastSeenText.text = item.LastSeen.X + ":" + item.LastSeen.Z;
        //costText.text = "~"+DonnerUtils.CalculateTeleportCost(DonnerPlayerAuthorative.instance.transform.position, new Vector3(item.LastSeen.X, DonnerPlayerAuthorative.instance.transform.position.y, item.LastSeen.Z)).ToString() + " sats";
    }

}
