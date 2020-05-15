using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Lnrpc;
using Daemon;
using System.Threading;

public class WalletMenuUI : MonoBehaviour, IRefreshableUI
{
    public TextMeshProUGUI balanceText;
    public TMP_InputField invoiceInput;
    public Button InvoiceInfoButton;
    public Button PayButton;

    public Button DonationInfoButton;
    public Slider DonationSlider;
    public TMP_InputField DonationInput;
    public Button DonateButton;
    public TextMeshProUGUI GameDonationPercentageText;
    public TextMeshProUGUI DeveloperDonationPercentageText;

    public GameObject LicenceMissingPanel;
    public TextMeshProUGUI missingValueText;
    public Button LicenceMissingInfoButton;

    long balance;

    private void Start()
    {
        InvoiceInfoButton.onClick.AddListener(OnInvoiceInfoButtonPress);
        PayButton.onClick.AddListener(OnPayButtonPress);

        DonationInfoButton.onClick.AddListener(OnDonationInfoButtonPress);
        DonationSlider.onValueChanged.AddListener(OnDonationSliderValueChange);
        DonateButton.onClick.AddListener(OnDonateButtonPress);
        LicenceMissingInfoButton.onClick.AddListener(OnLicenceMissingInfoButtonPress);

    }

    private void OnEnable()
    {
        if (PlayerServiceConnections.instance.ServicesReady)
        {
            RefreshBalance();
        }
    }

    void OnInvoiceInfoButtonPress()
    {
        //TODO change text
        PopUpArgs args = new PopUpArgs("pay invoice", GameText.PayInvoiceInfo);
        PopUpManagerUI.instance.OpenPopUp(args);
    }

    async void OnPayButtonPress()
    {
        //TODO show error instead if invoice is invalid or not existent
        //TODO show invoice value
        string invoice = invoiceInput.text;
        PayReq decodedInvoice;
        try
        {
            decodedInvoice = await PlayerServiceConnections.instance.lnd.DecodePayreq(invoice);

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            PopUpArgs errArgs = new PopUpArgs("error", e.Message);
            PopUpManagerUI.instance.OpenPopUp(errArgs);
            return;
        }
        string text = String.Format(GameText.PayInvoicePopup, decodedInvoice.Description, decodedInvoice.NumSatoshis.ToString() + Utility.tintedSatsSymbol);

        YesNoPopUpArgs args = new YesNoPopUpArgs("pay invoice ", text, OnPayRequest);
        PopUpManagerUI.instance.OpenYesNoPopUp(args);
    }
    
    async void OnPayRequest(bool pay)
    {

        if (pay)
        {
            string invoice = invoiceInput.text;
            try
            {
                await PlayerServiceConnections.instance.lnd.PayInvoice(invoice);
            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
                PopUpArgs errArgs = new PopUpArgs("error", e.Message);
                PopUpManagerUI.instance.OpenPopUp(errArgs);
                return;
            }
            invoiceInput.text = "";
            PopUpArgs args = new PopUpArgs("info", "payment successfull");
            PopUpManagerUI.instance.OpenPopUp(args);
            RefreshBalance();
        }
    }
    async void RefreshBalance()
    {
        GetBalanceResponse res;
        
        try
        {
            res = await PlayerServiceConnections.instance.DonnerDaemonClient.GetWalletBalance();
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            PopUpArgs errArgs = new PopUpArgs("error", e.Message);
            PopUpManagerUI.instance.OpenPopUp(errArgs);
            return;
        }

        if(res.ChannelMissingBalance > 0)
        {
            LicenceMissingPanel.SetActive(true);
            balanceText.text = "0" + Utility.tintedSatsSymbol;
            missingValueText.text = (res.ChannelMissingBalance-res.BufferBalance) + Utility.tintedSatsSymbol;
        }
        else
        {
            LicenceMissingPanel.SetActive(false);

            balanceText.text = res.DaemonBalance + Utility.tintedSatsSymbol;

        }



    }

    void OnLicenceMissingInfoButtonPress()
    {
        PopUpArgs args = new PopUpArgs("Licence Missing", GameText.LicenceMissingInfo);
        PopUpManagerUI.instance.OpenPopUp(args);
    }


    void OnDonationInfoButtonPress()
    {
        PopUpArgs args = new PopUpArgs("donation",GameText.DonationInfo);
        PopUpManagerUI.instance.OpenPopUp(args);
    }

    void OnDonationSliderValueChange(float value)
    {
        int devs = (int)value;
        int game = 100-devs;
        GameDonationPercentageText.text = game + "%";
        DeveloperDonationPercentageText.text = devs + "%";
    }

    async void OnDonateButtonPress()
    {
        int devs = (int)DonationSlider.value;
        int game = 100 - devs;
        long totalSats = long.Parse(DonationInput.text);
        long gameSats = (long)((float)totalSats * (float)game / (100f));
        long devsSats = totalSats - gameSats;
        string invoice;

        try
        {
            invoice = await PlayerServiceConnections.instance.BackendPlayerClient.GetDonationInvoice(gameSats, devsSats);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            PopUpArgs errArgs = new PopUpArgs("error", e.Message);
            PopUpManagerUI.instance.OpenPopUp(errArgs);
            return;
        }

        GetBalanceResponse balanceResponse;
        try
        {
            balanceResponse = await PlayerServiceConnections.instance.DonnerDaemonClient.GetWalletBalance();

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            PopUpArgs errArgs = new PopUpArgs("Error", e.Message);
            PopUpManagerUI.instance.OpenPopUp(errArgs);
            return;
        }

        PayReq payreq;
        try
        {
            payreq = await PlayerServiceConnections.instance.lnd.DecodePayreq(invoice);

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            PopUpArgs errArgs = new PopUpArgs("Error", e.Message);
            PopUpManagerUI.instance.OpenPopUp(errArgs);
            return;
        }



        long balance = balanceResponse.DaemonBalance;

        string text = String.Format("You are going to Donate {0}{1} to the game pot and {2}{1} to the developers.",gameSats,Utility.tintedSatsSymbol,devsSats);



        if (balance < payreq.NumSatoshis) text += "\n \n Your Ingame Wallet doesent cover the required amount!";

        
        List<PopUpButtonArgs> actions = new List<PopUpButtonArgs>();
        actions.Add(new PopUpButtonArgs("ingame Wallet", () => PaymentUIHelper.IngamePayment(invoice, payreq,() => {
            RefreshBalance();
        })));
        actions.Add(new PopUpButtonArgs("external Wallet", () => PaymentUIHelper.ExternalPayment(invoice, payreq)));

        PopUpArgs args = new PopUpArgs("Donation", text, actions, false);
        PopUpUI popup = PopUpManagerUI.instance.OpenPopUp(args);

        if (balance < totalSats) popup.buttons[0].interactable = false;
        
    }

    public void Refresh()
    {
        RefreshBalance();
    }
}
