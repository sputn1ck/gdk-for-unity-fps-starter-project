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
        PopUpEventArgs args = new PopUpEventArgs("pay invoice", "Here some info about invoices! Maybe Kon Has some suggestions for this text.");

        ClientEvents.instance.onPopUp.Invoke(args);
    }

    void OnPayButtonPress()
    {
        //TODO show error instead if invoice is invalid or not existent

        YesNoPopUpEventArgs args = new YesNoPopUpEventArgs("pay", "Do you really want to pay the invoice of XXx <sprite name=sats>?", OnPayRequest);
        ClientEvents.instance.onYesNoPopUp.Invoke(args);
    }

    void OnPayRequest(bool pay)
    {
        if (pay)
        {
            //TODO pay Invoice
        }
    }

}
