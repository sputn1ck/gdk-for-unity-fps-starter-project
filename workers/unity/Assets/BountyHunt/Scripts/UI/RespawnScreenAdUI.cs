using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class RespawnScreenAdUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RawImage image;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI satsText;
    public Button button;
    public Animator animator;
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

        nameText.text = advertiserInvestment.advertiser.name;
        satsText.text = Utility.SatsToShortString(advertiserInvestment.investment,true,UITinter.tintDict[TintColor.Sats]);

        if (advertiserInvestment.advertiser.url != "")
        {
            button.gameObject.SetActive(true);
            if (!UrlMemory.UrlInQueue(adInv.advertiser.url))
            {
                button.interactable = true;
            }
            else
            {
                button.interactable = false;
            }
        }
        else{
            button.gameObject.SetActive(false);
        }
        image.texture = adInv.advertiser.GetRandomTexture(Advertiser.AdMaterialType.SQUARE);

    }

    void OnClick()
    {
        if (advertiserInvestment.advertiser.url == "")
            return;
        button.interactable = false;
        UrlMemory.AddUrl(advertiserInvestment.advertiser.url);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetBool("showInfo", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetBool("showInfo", false);

    }

}
