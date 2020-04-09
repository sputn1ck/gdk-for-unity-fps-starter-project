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

    string auctionText = "";
    long auctionSats = 0;


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
        nextID = -2;
        ClientEvents.instance.onDonationMessageUpdate.AddListener(UpdateDonationMessage);
        ClientEvents.instance.onUpdateAdvertisers.AddListener(UpdateAdvertiserMessages);
        InvokeRepeating("Next", 0, secondsPerAd);
    }

    private void UpdateDonationMessage(string msg, long sats)
    {
        auctionText = msg;
        auctionSats = sats;
    }

    private void UpdateAdvertiserMessages(List<Advertiser> advertisers)
    {
        this.advertisers = advertisers;
        nextID = -2;
    }

    void Next()
    {
        if (nextID >= advertiserCount) nextID = -2;

        if(nextID < 0)
        {
            messageText.text = auctionText;
            satsText.text = Utility.SatsToShortString( auctionSats, UITinter.tintDict[TintColor.Sats]);
        }
        else
        {
            messageText.text = advertisers[nextID].name + "    " + advertisers[nextID].url;
            satsText.text = Utility.SatsToShortString(advertisers[nextID].investment, UITinter.tintDict[TintColor.Sats]);
        }
        nextID++;
    }

}
