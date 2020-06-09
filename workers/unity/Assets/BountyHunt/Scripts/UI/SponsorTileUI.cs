using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SponsorTileUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI satsText;
    public Button openLinkButton;
    public Button openLaterButton;
    public TextMeshProUGUI savedForLaterText;

    Advertiser advertiser;

    void Set(Advertiser advertiser)
    {
        advertiser = advertiser;
    }


}
