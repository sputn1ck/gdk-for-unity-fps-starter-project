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

    public void Connect()
    {

        SetButtonFalse();
        StartCoroutine(ConnectEnumerator());
    }

    public IEnumerator ConnectEnumerator()
    {
        if (setName)
        {
            nameMenu.Select();
        }else
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
        getNameAndPubkey();
    }

    private async void getNameAndPubkey()
    {
        var getinfo = await LnClient.instance.lnd.GetInfo();
        var pubkey = getinfo.IdentityPubkey;
        try
        {

            var name = await BackendPlayerBehaviour.instance.client.GetUsername(pubkey);
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
