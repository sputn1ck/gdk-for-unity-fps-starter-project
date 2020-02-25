using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitTabWindowUI : TabMenuWindowUI
{
    public Button quitGameButton;
    public Button MainMenuButton;

    void Start()
    {
        quitGameButton.onClick.AddListener(QuitGame);
        MainMenuButton.onClick.AddListener(MainMenu);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void MainMenu()
    {
        LndConnector.Instance.Disconnect();
    }

    public void payOutAllEarnings()
    {
        //TODO
        //PlayerNode.instance.TryPayoutAll();
    }
}
