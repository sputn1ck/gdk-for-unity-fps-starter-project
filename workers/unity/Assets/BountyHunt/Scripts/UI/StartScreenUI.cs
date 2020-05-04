using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenUI : MonoBehaviour
{
    public GameObject initializePanel;
    public TextMeshProUGUI initializeText;

    public GameObject retryPanel;
    public Button retryButton;

    public GameObject setNamePanel;
    public TMP_InputField nameInput;
    public Button submitNameButton;

    private void Awake()
    {
        retryButton.onClick.AddListener(Initialize);
        submitNameButton.onClick.AddListener(SubmitName);
    }

    private void Start()
    {
        Initialize();
    }

    public async void Initialize()
    {

        initializePanel.SetActive(true);
        setNamePanel.SetActive(false);
        retryPanel.SetActive(false);

        SetInitializeString("Initialize");

        try
        {
            await PlayerServiceConnections.instance.SetupServices(SetInitializeString);
            if (await PlayerServiceConnections.instance.BackendPlayerClient.NeedsUsernameChange())
            {
                ShowSetNamePanel();
                return;
            }
        }
        catch (Exception e)
        {
            PopUpArgs args = new PopUpArgs("Error", e.Message);
            PopUpManagerUI.instance.OpenPopUp(args);
            ShowRetryPanel();
            return;
        }

        OpenMainMenu();
    }

    public void ShowSetNamePanel()
    {
        initializePanel.SetActive(false);
        setNamePanel.SetActive(true);
        retryPanel.SetActive(false);
    }

    public void ShowRetryPanel()
    {
        initializePanel.SetActive(false);
        setNamePanel.SetActive(false);
        retryPanel.SetActive(true);
    }


    public void SetInitializeString(string text)
    {
        initializeText.text = text;
    }

    public async void SubmitName()
    {
        string n = nameInput.text;
        try
        {
            await PlayerServiceConnections.instance.BackendPlayerClient.SetUsername(n);
        }
        catch(Exception e)
        {
            PopUpArgs args = new PopUpArgs("Error", e.Message);
            PopUpManagerUI.instance.OpenPopUp(args);
            return;
        }
        OpenMainMenu();
    }

    void OpenMainMenu()
    {
        if (BBHUIManager.instance != null)
        {
            BBHUIManager.instance.ShowMainMenu();
        }
        else
        {
            Debug.Log("there is no main menu!");
        }
    }

}
