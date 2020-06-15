using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SponsorTileUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RawImage image;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI satsText;
    public Button openLinkButton;
    public Button openLaterButton;
    public TextMeshProUGUI savedForLaterText;

    Animator animator;
    AdvertiserInvestment advertiserInvestment;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        savedForLaterText.text = GameText.AdOpenInfo;
        openLinkButton.onClick.AddListener(OnLinkButtonClick);
        openLaterButton.onClick.AddListener(OnLaterButtonClick);
    }

    public void Set(AdvertiserInvestment advertiserInvestment)
    {
        this.advertiserInvestment = advertiserInvestment;
        nameText.text = advertiserInvestment.advertiser.name;
        satsText.text = Utility.SatsToShortString(advertiserInvestment.investment, true, UITinter.tintDict[TintColor.Sats]);
        if (advertiserInvestment.advertiser.url != "")
        {
            ShowLaterButton(!UrlMemory.UrlInQueue(advertiserInvestment.advertiser.url));
        } else
        {
            openLaterButton.gameObject.SetActive(false);
            savedForLaterText.gameObject.SetActive(false);
            openLinkButton.gameObject.SetActive(false);
        }
        image.texture = advertiserInvestment.advertiser.GetRandomTexture(Advertiser.AdMaterialType.SQUARE);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetBool("showInfo", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetBool("showInfo", false);

    }

    void OnLinkButtonClick()
    {
        UrlMemory.DoNotOpenAllLinksNextTime = true;
        Application.OpenURL(advertiserInvestment.advertiser.url);
    }

    void OnLaterButtonClick()
    {
        UrlMemory.AddUrl(advertiserInvestment.advertiser.url);
        ShowLaterButton(false);
    }

    void ShowLaterButton(bool show)
    {
        openLaterButton.gameObject.SetActive(show);
        savedForLaterText.gameObject.SetActive(!show);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus && advertiserInvestment!=null)
        {
            ShowLaterButton(!UrlMemory.UrlInQueue(advertiserInvestment.advertiser.url));
        }
    }

}
