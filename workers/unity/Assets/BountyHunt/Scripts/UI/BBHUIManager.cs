using Fps.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BBHUIManager : MonoBehaviour
{

    public ScreenManager ScreenManager;
    public InGameScreenManagerUI inGame;
    public static BBHUIManager instance;

    public Color primaryUIColor= Color.white;
    public Color secondaryUIColor = Color.white;


    private void Awake()
    {
        instance = this;

        ScreenManager.gameObject.SetActive(false);

        UITinter.setColor(TintColor.Primary, primaryUIColor);
        UITinter.setColor(TintColor.Secondary, secondaryUIColor);

    }

    private void Start()
    {
        ShowFrontEnd();
    }


    public void ShowGameView()
    {
        ScreenManager.gameObject.SetActive(false);
        inGame.gameObject.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowFrontEnd()
    {
        inGame.gameObject.SetActive(false);
        ScreenManager.gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

}
