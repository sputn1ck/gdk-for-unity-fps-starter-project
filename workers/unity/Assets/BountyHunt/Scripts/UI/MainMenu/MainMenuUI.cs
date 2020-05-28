using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI instance;
    public PreviewSpot previewSpot;
    public Button playButton;
    public Button quitButton;
    public GameObject connectingInfoObject;
    public TextMeshProUGUI versionText;
    public GameObject uiCam;
    public GameObject blendImage;
        

    private void Awake()
    {
        instance = this;
        playButton.onClick.AddListener(OnPlayButtonPress);
        quitButton.onClick.AddListener(OnQuitButtonPress);
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
        string playername;
        try
        {
            await LndConnector.Instance.Connect();
            playername = await PlayerServiceConnections.instance.BackendPlayerClient.GetUsername();

        }
        catch (Exception e)
        {
            PopUpArgs args = new PopUpArgs("Error", e.Message);
            PopUpManagerUI.instance.OpenPopUp(args);
            connectingInfoObject.SetActive(false);
            return;
        }
        OnConnectionSuccesss(playername);

    }

    public async void OnConnectionSuccesss(string playername)
    {
        Debug.Log("joining game");
        BBHUIManager.instance.ShowGameView();
        LndConnector.Instance.SpawnPlayer(playername, 0);
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

    public void OnQuitButtonPress()
    {
        YesNoPopUpArgs args = new YesNoPopUpArgs("Quit", GameText.QuitGamePopup, Quit);
        PopUpManagerUI.instance.OpenYesNoPopUp(args);
        return;
    }

    void Quit(bool doIt)
    {
        if (!doIt) return;
        Debug.Log("Quitting!");
        Application.Quit();
    }


    public void BlendImage(bool show)
    {
        uiCam.GetComponent<PostProcessVolume>().enabled = !show;
        blendImage.SetActive(show);
    }


}
