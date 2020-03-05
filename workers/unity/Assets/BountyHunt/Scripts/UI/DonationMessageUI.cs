using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DonationMessageUI : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI satsText;

    void Start()
    {
        ClientEvents.instance.onDonationMessageUpdate.AddListener(UpdateMessage);
    }

    private void UpdateMessage(string msg, long sats)
    {
        messageText.text = msg;
        satsText.text = Utility.SatsToShortString(sats, UITinter.tintDict[TintColor.Sats]);
    }


}
