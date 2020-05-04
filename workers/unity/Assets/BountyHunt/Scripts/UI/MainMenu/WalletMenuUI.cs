using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class WalletMenuUI : MonoBehaviour
{
    public TextMeshProUGUI balanceText;
    public TMP_InputField invoiceInput;
    public Button InvoiceInfoButton;
    public Button PayButton;

    long balance;

    private void Start()
    {
        InvoiceInfoButton.onClick.AddListener(OnInvoiceInfoButtonPress);
        PayButton.onClick.AddListener(OnPayButtonPress);
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
        PopUpArgs args = new PopUpArgs("pay invoice", "Here some info about invoices! Maybe Kon Has some suggestions for this text.");
        PopUpManagerUI.instance.OpenPopUp(args);
    }

    void OnPayButtonPress()
    {
        //TODO show error instead if invoice is invalid or not existent
        //TODO show invoice value
        
        YesNoPopUpArgs args = new YesNoPopUpArgs("pay", "Do you really want to pay the invoice of XXx <sprite name=sats>?", OnPayRequest);
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
        try
        {
            balance = (await PlayerServiceConnections.instance.DonnerDaemonClient.GetWalletBalance()).DaemonBalance;
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            PopUpArgs errArgs = new PopUpArgs("error", e.Message);
            PopUpManagerUI.instance.OpenPopUp(errArgs);
            return;
        }
        balanceText.text = balance + Utility.tintedSatsSymbol;

    }

}
