using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitTabWindowUI : TabMenuWindowUI
{
    public Button payOutAllEarningsButton;
    public Button quitGameButton;
    public Button MainMenuButton;

    void Start()
    {
        payOutAllEarningsButton.onClick.AddListener(payOutAllEarnings);
        quitGameButton.onClick.AddListener(QuitGame);
        MainMenuButton.onClick.AddListener(MainMenu);
    }

    public void QuitGame()
    {
        Application.Quit();
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
