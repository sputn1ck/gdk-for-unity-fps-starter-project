using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Bountyhunt;

public class DonationMessageUI : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI satsText;
    public float secondsPerAd = 5;
    List<Advertiser> advertisers;
    int nextID;


    int advertiserCount
    {
        get
        {
            if (advertisers == null) { return 0; }
            return advertisers.Count;
        }
    }

    void Start()
    {
        nextID = 0;
        ClientEvents.instance.onUpdateAdvertisers.AddListener(UpdateAdvertiserMessages);
        InvokeRepeating("Next", 0, secondsPerAd);
    }

    private void UpdateAdvertiserMessages(List<Advertiser> advertisers)
    {
        this.advertisers = advertisers;
        nextID = 0;
    }

    void Next()
    {
        if (advertisers == null || advertisers.Count == 0) {
            nextID = 0;
            messageText.text = "";
            satsText.text = "";
            return;
        }

        if (nextID >= advertisers.Count) nextID = 0;
        messageText.text = advertisers[nextID].name + "    " + advertisers[nextID].url;
        satsText.text = Utility.SatsToShortString(advertisers[nextID].investment, UITinter.tintDict[TintColor.Sats]);
        
        nextID++;
    }

}
