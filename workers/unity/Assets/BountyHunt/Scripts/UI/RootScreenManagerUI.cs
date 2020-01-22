using Fps.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootScreenManagerUI : MonoBehaviour
{

    public ScreenManager frontEnd;
    public InGameScreenManagerUI inGame;
    public static RootScreenManagerUI instance;


    private void Awake()
    {
        instance = this;

        frontEnd.gameObject.SetActive(false);

        UIManager.onShowFrontEnd.AddListener(OnShowFrontEnd);
        UIManager.onShowGameView.AddListener(OnShowGameView);
        UIManager.onToggleEscapeMenu.AddListener(OnToggleEscapeMenu);
    }

    private void Start()
    {
        OnShowFrontEnd();
    }


    void OnShowGameView()
    {
        frontEnd.gameObject.SetActive(false);
        inGame.gameObject.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnShowFrontEnd()
    {
        inGame.gameObject.SetActive(false);
        frontEnd.gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void OnToggleEscapeMenu()
    {
        inGame.ToggleEscapeScreen();
    }
}
