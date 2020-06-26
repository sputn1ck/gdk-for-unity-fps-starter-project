using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using Lnrpc;
using System.Threading.Tasks;

public class SponsorTileUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RawImage image;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI satsText;
    public Button openLinkButton;
    public Button bookmarkLinkButton;
    public Button buyButton;

    Animator animator;
    AdvertiserInvestment advertiserInvestment;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        openLinkButton.onClick.AddListener(OnLinkButtonClick);
        bookmarkLinkButton.onClick.AddListener(OnLaterButtonClick);
        buyButton.onClick.AddListener(OnBuyButtonClick);

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
            bookmarkLinkButton.gameObject.SetActive(false);
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
    async void OnBuyButtonClick()
    {

        float playerSatsprice;

        try
        {
            var res = await PlayerServiceConnections.instance.BackendPlayerClient.GetInfo();
            playerSatsprice = res.SponsorFeeInfo.PlayerSatoshiCost;

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            PopUpArgs errArgs = new PopUpArgs("Error", e.Message);
            PopUpManagerUI.instance.OpenPopUp(errArgs);
            return;
        }

        List<IPopupElement> elements = new List<IPopupElement>();
        elements.Add(new TextPopupElement { text = GameText.IncreaseSponsorPlayerSatsPopupText });
        elements.Add(new PlayerSatsSettingsPopupElement {
            defaultSats = 1000,
            ingamePayAction = IncreasePlayersatsInternal,
            walletPayAction = IncreasePlayersatsExternal,
            playersatsPrice = playerSatsprice,
            priceLabelText = "price",

        });

        PopUpManagerUI.instance.OpenModularPopUp(GameText.IncreaseSponsorPlayerSatsPopupHeader, elements);

    }

    async void IncreasePlayersatsInternal(long input)
    {

        (string invoice, PayReq payreq) payInfo;

        try
        {
            payInfo = await GetAddPlayerSatsPayReq(input);

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            PopUpArgs errArgs = new PopUpArgs("Error", e.Message);
            PopUpManagerUI.instance.OpenPopUp(errArgs);
            return;
        }

        PaymentUIHelper.IngamePayment(payInfo.invoice,payInfo.payreq);
        

    }
    async void IncreasePlayersatsExternal(long input)
    {
        (string invoice, PayReq payreq) payInfo;

        try
        {
            payInfo = await GetAddPlayerSatsPayReq(input);

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            PopUpArgs errArgs = new PopUpArgs("Error", e.Message);
            PopUpManagerUI.instance.OpenPopUp(errArgs);
            return;
        }

        PaymentUIHelper.ExternalPayment(payInfo.invoice, payInfo.payreq);
        
    }

    async Task<(string invoice,PayReq payReq)> GetAddPlayerSatsPayReq(long sats)
    {
        string invoice;
        try
        {
            invoice = await PlayerServiceConnections.instance.BackendPlayerClient.GetPlayerSatsInvoice(advertiserInvestment.advertiser.hash, sats);
        }
        catch (Exception e)
        {
            throw (e);
        }

        PayReq payreq;
        try
        {
            payreq = await PlayerServiceConnections.instance.lnd.DecodePayreq(invoice);

        }
        catch (Exception e)
        {
            throw e;
        }
        return (invoice,payreq);
    }

    void ShowLaterButton(bool show)
    {
        bookmarkLinkButton.interactable = show;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus && advertiserInvestment!=null)
        {
            ShowLaterButton(!UrlMemory.UrlInQueue(advertiserInvestment.advertiser.url));
        }
    }

}
