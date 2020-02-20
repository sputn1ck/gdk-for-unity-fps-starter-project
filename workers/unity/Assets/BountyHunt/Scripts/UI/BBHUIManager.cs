using Fps.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BBHUIManager : MonoBehaviour
{

    public ScreenManager ScreenManager;
    public InGameScreenManagerUI inGame;
    public static BBHUIManager instance;

    [SerializeField] public List<TintingPair> UITints;


    private void Awake()
    {
        instance = this;

        ScreenManager.gameObject.SetActive(false);

        foreach(TintingPair tp in UITints)
        {
            UITinter.setColor(tp.tint, tp.color);
        }

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
