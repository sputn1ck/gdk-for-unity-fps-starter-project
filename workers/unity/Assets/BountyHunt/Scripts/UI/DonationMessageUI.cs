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
    List<AdvertiserInvestment> advertiserInvestments;
    int nextID;


    int advertiserCount
    {
        get
        {
            if (advertiserInvestments == null) { return 0; }
            return advertiserInvestments.Count;
        }
    }

    void Start()
    {
        nextID = 0;
        ClientEvents.instance.onUpdateBillboardAdvertisers.AddListener(UpdateAdvertiserMessages);
        InvokeRepeating("Next", 0, secondsPerAd);
    }

    private void UpdateAdvertiserMessages(List<AdvertiserInvestment> adInvs)
    {
        this.advertiserInvestments = adInvs;
        nextID = 0;
    }

    void Next()
    {
        if (advertiserInvestments == null || advertiserInvestments.Count == 0) {
            nextID = 0;
            messageText.text = "";
            satsText.text = "";
            return;
        }

        if (nextID >= advertiserInvestments.Count) nextID = 0;
        messageText.text = advertiserInvestments[nextID].advertiser.name + "    " + advertiserInvestments[nextID].advertiser.url;
        satsText.text = Utility.SatsToShortString(advertiserInvestments[nextID].investment, UITinter.tintDict[TintColor.Sats]);
        
        nextID++;
    }

}
