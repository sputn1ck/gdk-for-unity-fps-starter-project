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
            text.text = adInv.advertiser.name+ " <sprite name=\"link\" tint=1> ";
            button.interactable = true;
        }
        else{
            text.GetComponent<UITinter>().updateColor(TintColor.Primary);
            text.text = GameText.AdOpenInfo;
            button.interactable = false;
        }
        image.texture = adInv.advertiser.GetRandomTexture(Advertiser.AdMaterialType.SQUARE);

    }

    void OnClick()
    {
        text.GetComponent<UITinter>().updateColor(TintColor.Primary);
        text.text = GameText.AdOpenInfo;
        UrlMemory.AddUrl(advertiserInvestment.advertiser.url);
        button.interactable = false;
    }

}
