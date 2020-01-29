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
            var res = await BackendPlayerBehaviour.instance.client.SetUsername(pubkey, playerName);
            StartCoroutine(ConnectEnumerator());

        } catch(RpcException e)
        {
            ErrorText.text = e.Message;
            return;
        }
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
    }
    public override void OnSelect()
    {
        base.OnSelect();
        ErrorText.text = "";

        pubkey = LnClient.instance.GetPubkey();
    }

    public IEnumerator ConnectEnumerator()
    {

            if (LndConnector.Instance.connectLocal)
            {
                yield return LndConnector.Instance.GetPit();
                LndConnector.Instance.deploymentId = "";
                yield return LndConnector.Instance.GetLoginToken();
                LndConnector.Instance.Connect();
                spawnMenu.Select();
            }
            else
            {
                yield return LndConnector.Instance.GetPit();
                serverMenu.Select();
            }
        }
 
}
