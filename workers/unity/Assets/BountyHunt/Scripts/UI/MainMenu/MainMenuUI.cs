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
        previewSpot.gameObject.SetActive(false);
    }

    public async void OnPlayButtonPress()
    {
        connectingInfoObject.gameObject.SetActive(true);
        string answer = await LndConnector.Instance.Connect();
        //string answer = "Todo: server Connection";

        if (answer == "ok")
        {
            OnConnectionSuccesss();
        }
        else
        {
            PopUpEventArgs args = new PopUpEventArgs("Error", answer);
            ClientEvents.instance.onPopUp.Invoke(args);
            connectingInfoObject.SetActive(false);
        }

    }

    public void OnConnectionSuccesss()
    {
        LndConnector.Instance.SpawnPlayer("gude", 0);
    }
}
