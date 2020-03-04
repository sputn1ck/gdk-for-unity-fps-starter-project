using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GamePotUI : MonoBehaviour
{
    public TextMeshProUGUI globalBountyText;
    public TextMeshProUGUI globalLootText;
    public TextMeshProUGUI globalPotText;

    private void Start()
    {
        ClientEvents.instance.onGlobalBountyUpdate.AddListener(OnUpdateGlobalBounty);
        ClientEvents.instance.onGlobalLootUpdate.AddListener(OnUpdateGlobalLoot);
        ClientEvents.instance.onGlobalPotUpdate.AddListener(OnUpdateGlobalPot);
    }

    void OnUpdateGlobalBounty(long sats){ UpdateField(globalBountyText, sats);}
    void OnUpdateGlobalLoot(long sats){ UpdateField(globalLootText, sats);}
    void OnUpdateGlobalPot(long sats){ UpdateField(globalPotText, sats); }

    public void UpdateField(TextMeshProUGUI textField, long sats)
    {
        string text = Utility.LongToShortString(sats)+ "<sprite name=\"sats\" color=#FFED92>";
        textField.text = text; 
    }

    
    public long testnumber;
    public bool test;

    private void Update()
    {
        if (test)
        {
            test = false;
            ClientEvents.instance.onGlobalBountyUpdate.Invoke(testnumber);
            ClientEvents.instance.onGlobalLootUpdate.Invoke(testnumber*2);
            ClientEvents.instance.onGlobalPotUpdate.Invoke(testnumber*3);
        }
    }
    

}
