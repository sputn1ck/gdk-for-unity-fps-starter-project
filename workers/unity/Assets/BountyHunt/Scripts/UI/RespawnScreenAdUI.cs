using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class RespawnScreenAdUI : MonoBehaviour
{
    public RawImage image;
    public Button button;
    public TextMeshProUGUI text;
    AdvertiserInvestment advertiserInvestment;

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
    }

    public void Set(AdvertiserInvestment adInv)
    {
        if (adInv == null)
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            gameObject.SetActive(true);
        }

        this.advertiserInvestment = adInv;
        if (!UrlMemory.UrlInQueue(adInv.advertiser.url))
        {
            text.GetComponent<UITinter>().updateColor(TintColor.Link);
            if (advertiserInvestment.advertiser.url != "")
            {
                text.text = adInv.advertiser.name + " <sprite name=\"link\" tint=1> ";
                button.interactable = true;
            } else
            {
                text.text = adInv.advertiser.name;
                button.interactable = false;
            }
        }
        else{
            text.GetComponent<UITinter>().updateColor(TintColor.Primary);
            text.text = GameText.linkBookmarkedInfo;
            button.interactable = false;
        }
        image.texture = adInv.advertiser.GetRandomTexture(Advertiser.AdMaterialType.SQUARE);

    }

    void OnClick()
    {
        if (advertiserInvestment.advertiser.url == "")
            return;
        text.GetComponent<UITinter>().updateColor(TintColor.Primary);
        text.text = GameText.linkBookmarkedInfo;
        UrlMemory.AddUrl(advertiserInvestment.advertiser.url);
        button.interactable = false;
    }

}
