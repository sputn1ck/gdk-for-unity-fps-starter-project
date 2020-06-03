using Fps.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BBHUIManager : MonoBehaviour
{

    public ScreenManager ScreenManager;
    public StartScreenUI startScreen;
    public InGameScreenManagerUI inGame;
    public MainMenuUI mainMenu;
    public GameObject uiCam;
    public GameObject blendImage;
    public static BBHUIManager instance;


    private void Awake()
    {
        instance = this;

        ScreenManager.gameObject.SetActive(false);

    }

    private void Start()
    {
        ShowFrontEnd();
    }


    public void ShowGameView()
    {
        ScreenManager.gameObject.SetActive(false);
        inGame.gameObject.SetActive(true);
        mainMenu.gameObject.SetActive(false);
        startScreen.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        CursorUI.Instance.Hide();
    }

    public void ShowFrontEnd()
    {
        inGame.gameObject.SetActive(false);
        ScreenManager.gameObject.SetActive(true);
        mainMenu.gameObject.SetActive(false);
        startScreen.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        CursorUI.Instance.Show();
    }

    public void ShowMainMenu()
    {
        inGame.gameObject.SetActive(false);
        ScreenManager.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
        startScreen.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        CursorUI.Instance.Show();
    }

}
