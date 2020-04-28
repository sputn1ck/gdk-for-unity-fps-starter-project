using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public PreviewSpot previewSpot;
    public Button playButton;
    public GameObject connectingInfoObject;

    private void Awake()
    {
        playButton.onClick.AddListener(OnPlayButtonPress);
    }

    private void OnEnable()
    {
        connectingInfoObject.SetActive(false);
        previewSpot.gameObject.SetActive(true);
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
            PopUpEventArgs args = new PopUpEventArgs("Error", e.Message);
            ClientEvents.instance.onPopUp.Invoke(args);
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
}
