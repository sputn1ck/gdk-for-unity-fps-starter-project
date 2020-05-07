using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public PreviewSpot previewSpot;
    public Button playButton;
    public GameObject connectingInfoObject;
    public TextMeshProUGUI versionText; 

    private void Awake()
    {
        playButton.onClick.AddListener(OnPlayButtonPress);
    }

    private void OnEnable()
    {
        connectingInfoObject.SetActive(false);
        previewSpot.gameObject.SetActive(true);
        if (PlayerServiceConnections.instance.ServicesReady)
        {
            RefreshVersion();
        }
    }
    private void OnDisable()
    {
        if(previewSpot != null)
            previewSpot.gameObject.SetActive(false);
    }

    public async void OnPlayButtonPress()
    {
        connectingInfoObject.gameObject.SetActive(true);

        try
        {
            await LndConnector.Instance.Connect();
        }
        catch (Exception e)
        {
            PopUpArgs args = new PopUpArgs("Error", e.Message);
            PopUpManagerUI.instance.OpenPopUp(args);
            connectingInfoObject.SetActive(false);
            return;
        }
        OnConnectionSuccesss();

    }

    public void OnConnectionSuccesss()
    {
        Debug.Log("joining game");
        BBHUIManager.instance.ShowGameView();
        LndConnector.Instance.SpawnPlayer("gude", 0);
    }

    async void RefreshVersion()
    {
        string version;
        try
        {
            version = await PlayerServiceConnections.instance.BackendPlayerClient.GetGameVersion();
        }
        catch (Exception e)
        {
            PopUpArgs args = new PopUpArgs("Error", e.Message);
            PopUpManagerUI.instance.OpenPopUp(args);
            connectingInfoObject.SetActive(false);
            return;
        }

        versionText.text = version;
    }
}
