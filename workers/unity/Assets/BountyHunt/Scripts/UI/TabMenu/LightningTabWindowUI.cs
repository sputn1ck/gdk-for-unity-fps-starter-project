using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LightningTabWindowUI : TabMenuWindowUI
{

    //settings
    public TMP_InputField minPayoutInput;
    public TMP_InputField keepAmountInput;

    //donation
    public Toggle qrCodeToggle;
    //public DonationFieldUI donationField;
    public Button copyToClipboardButton;

    //info
    public TextMeshProUGUI syncedStateText;
    public TextMeshProUGUI channelStateText;
    public TextMeshProUGUI CanRecieveCountText;
    public TextMeshProUGUI BalanceCountText;


    void Start()
    {
        minPayoutInput.onEndEdit.AddListener(SetMinPayout);
        keepAmountInput.onEndEdit.AddListener(SetMinKeep);
        //copyToClipboardButton.onClick.AddListener(DonationCodeToClipboard);
        ClientEvents.instance.onUpdateSyncedState.AddListener(UpdateSyncedState);
        ClientEvents.instance.onUpdateHasChannelState.AddListener(UpdateChannelState);
        ClientEvents.instance.onUpdateCanRecieveBalance.AddListener(UpdateCanReceiveCount);
        ClientEvents.instance.onUpdateCurrentBalance.AddListener(UpdateBalanceCount);

        qrCodeToggle.isOn = PlayerPrefs.GetInt("showQRCode", 0) > 0;
        //qrCodeToggle.onValueChanged.AddListener(ToggleQRCode);
    }

    public void SetMinPayout(string inputString)
    {
        int value = int.Parse(inputString);
        //...
    }
    public void SetMinKeep(string inputString)
    {
        int value = int.Parse(inputString);
        //...
    }

    public void UpdateSyncedState(string state)
    {
        syncedStateText.text = state;
    }

    public void UpdateChannelState(string state)
    {
        channelStateText.text = state;
    }
    public void UpdateCanReceiveCount(long count)
    {
        CanRecieveCountText.text = "" + count;
    }
    public void UpdateBalanceCount(long count)
    {
        BalanceCountText.text = "" + count;
    }

    /*
    public void ToggleQRCode(bool show)
    {
        PlayerPrefs.SetInt("showQRCode", show ? 1 : 0);
        PlayerPrefs.Save();
        donationField.gameObject.SetActive(show);
    }

    public void DonationCodeToClipboard()
    {
        TextEditor te = new TextEditor();
        te.text = donationField.code;
        te.SelectAll();
        te.Copy();
    }
    */
}
