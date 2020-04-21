using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Grpc.Core;

public class ConnectSubMenuUI : SubMenuUI
{
    public SubMenuUI serverMenu;
    public SubMenuUI spawnMenu;

    public SubMenuUI nameMenu;
    public TextMeshProUGUI errorText;
    public Button ConnectButton;

    public bool setName;

    public void Awake()
    {
        ClientEvents.instance.onServicesSetup.AddListener(getNameAndPubkey);
    }
    public async void Connect()
    {

        SetButtonFalse();
        var res = await LndConnector.Instance.Connect();
        if(res != "")
        {
            //SHOW ERROR
            Debug.LogError("Error while connecting: " + res);
            Invoke("SetButtonTrue", 2f);
            return;
        }
        spawnMenu.Select();
    }

    

    public void SetButtonFalse()
    {
        ConnectButton.interactable = false;
        Invoke("SetButtonTrue", 2f);
    }

    public void SetButtonTrue()
    {

        ConnectButton.interactable = true;
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void StartLND()
    {

    }

    public override void OnSelect()
    {
        base.OnSelect();
        errorText.text = "";
        //Invoke("getNameAndPubkey",2f);
    }

    private async void getNameAndPubkey()
    {
        var getinfo = await PlayerServiceConnections.instance.lnd.GetInfo();
        var pubkey = getinfo.IdentityPubkey;
        try
        {

            var name = await PlayerServiceConnections.instance.BackendPlayerClient.GetUsername();
            if (pubkey == name)
            {
                setName = true;
            }
            else
            {
                setName = false;

                PlayerPrefs.SetString("playerName", name);
                PlayerPrefs.Save();
            }
        } catch (RpcException e)
        {
            Debug.Log(e);
        }
    }

}
