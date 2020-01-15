using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ConnectSubMenuUI : SubMenuUI
{
    public SubMenuUI serverMenu;
    public SubMenuUI spawnMenu;
    public TextMeshProUGUI errorText;
    public Button ConnectButton;

    public void Connect()
    {

        SetButtonFalse();
        StartCoroutine(ConnectEnumerator());
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

    public void SetButtonFalse()
    {
        ConnectButton.interactable = false;
        Invoke("SetButtonTrue", 5f);
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
    }

}
