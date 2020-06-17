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
    public Button bookmarkLinkButton;
    public Button buyButton;
    public TextMeshProUGUI buttonInfoText;

    Animator animator;
    AdvertiserInvestment advertiserInvestment;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        buttonInfoText.text = "";
        openLinkButton.onClick.AddListener(OnLinkButtonClick);
        bookmarkLinkButton.onClick.AddListener(OnLaterButtonClick);
        buyButton.onClick.AddListener(OnBuyButtonClick);

        openLinkButton.GetComponent<HoverDescriptionUI>().descriptionString = GameText.OpenLinkButtonDescription;
        bookmarkLinkButton.GetComponent<HoverDescriptionUI>().descriptionString = GameText.BookmarkLinkButtonDescription;
        buyButton.GetComponent<HoverDescriptionUI>().descriptionString = GameText.BuyPlayerSatsButtonDescription;

    }

    public void setButtonInfoText(string text)
    {
        buttonInfoText.text = text;
    }

    public void removeButtonInfoText(string text)
    {
        if(buttonInfoText.text == text)
        {
            buttonInfoText.text = "";
        }
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
            buttonInfoText.gameObject.SetActive(false);
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
    void OnBuyButtonClick()
    {
        List<InputPopUpButtonArgs> actions = new List<InputPopUpButtonArgs>();
        actions.Add(new InputPopUpButtonArgs("pay with ingame wallet", IncreasePlayersatsInternal));
        actions.Add(new InputPopUpButtonArgs("pay with external wallet", IncreasePlayersatsExternal));

        InputFieldPopUpArgs args = new InputFieldPopUpArgs(GameText.IncreaseSponsorPlayerSatsPopupHeader, GameText.IncreaseSponsorPlayerSatsPopupText, actions, true, "", Utility.tintedSatsSymbol, "100", TextAlignmentOptions.MidlineRight, TMP_InputField.ContentType.IntegerNumber);
        PopUpManagerUI.instance.OperInputFieldPopUp(args);

    }

    void IncreasePlayersatsInternal(string input)
    {
        int sats = int.Parse(input);
        Utility.Log("paying internal sats: " + sats, Color.cyan);
        //PaymentUIHelper.
    }
    void IncreasePlayersatsExternal(string input)
    {
        int sats = int.Parse(input);
        Utility.Log("paying external sats: " + sats, Color.cyan);

        //PaymentUIHelper.
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
