using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Grpc.Core;

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
            var res = await PlayerServiceConnections.instance.BackendPlayerClient.SetUsername(pubkey, playerName);
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
        var res = await LndConnector.Instance.Connect();
        if (res != "")
        {
            //SHOW ERROR
            Debug.LogError("Error while connecting: " + res);

            ErrorText.text = res;
            return;
        }
    }

 
}
