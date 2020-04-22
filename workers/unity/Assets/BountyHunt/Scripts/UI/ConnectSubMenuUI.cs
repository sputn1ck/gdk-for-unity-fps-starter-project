using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Grpc.Core;
using System;

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

        SetButtonFalse();
        ClientEvents.instance.onServicesSetup.AddListener(getNameAndPubkey);
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

            //TODO remove old stuff
            Invoke("SetButtonTrue", 2f);
            return;
        }

        //TODO remove old stuff
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
        SetButtonTrue();
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
