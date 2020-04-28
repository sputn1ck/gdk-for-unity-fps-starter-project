using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VaultMenuUI : MonoBehaviour
{
    public TextMeshProUGUI balanceText;
    public TMP_InputField invoiceInpput;
    public Button InvoiceInfoButton;
    public Button PayButton;

    private void Start()
    {
        InvoiceInfoButton.onClick.AddListener(OnInvoiceInfoButtonPress);
        PayButton.onClick.AddListener(OnPayButtonPress);
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

        YesNoPopUpArgs args = new YesNoPopUpArgs("pay", "Do you really want to pay the invoice of XXx <sprite name=sats>?", OnPayRequest);
        PopUpManagerUI.instance.OpenYesNoPopUp(args);
    }

    void OnPayRequest(bool pay)
    {
        if (pay)
        {
            //TODO pay Invoice
        }
    }

}
