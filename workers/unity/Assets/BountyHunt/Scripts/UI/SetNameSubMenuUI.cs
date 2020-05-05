using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Grpc.Core;
using System;

public class SetNameSubMenuUI : SubMenuUI
{
    public TextMeshProUGUI ErrorText;
    public TMP_InputField PlayerNameInputField;

    public SubMenuUI mainMenu;
    public SubMenuUI spawnMenu;
    public SubMenuUI serverMenu;
    public Button SetNameButton;
    public Button BackButton;

    string playerName;
    string pubkey;
    private void Awake()
    {
        SetNameButton.onClick.AddListener(SetName);
        PlayerNameInputField.onValueChanged.AddListener(SetPlayerName);
    }

    private async void SetName()
    {
        if (playerName == "")
        {
            ErrorText.text = "please choose a name!";
            return;
        }
        try
        {
            var res = await PlayerServiceConnections.instance.BackendPlayerClient.SetUsername(playerName);
            Connect();

        } catch(RpcException e)
        {
            ErrorText.text = e.Message;
            return;
        }
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
        PlayerPrefs.SetString("playerName", playerName);
        PlayerPrefs.Save();
    }
    public override void OnSelect()
    {
        base.OnSelect();
        ErrorText.text = "";

        pubkey = PlayerServiceConnections.instance.GetPubkey();
    }

    public async void Connect()
    {
        try
        {
            await LndConnector.Instance.Connect();
        }
        catch(Exception e)
        {
            Debug.LogError("Error while connecting: " + e.Message);
            //TODO removeOldStuff
            ErrorText.text = e.Message;
            return;
        }
    }

 
}
