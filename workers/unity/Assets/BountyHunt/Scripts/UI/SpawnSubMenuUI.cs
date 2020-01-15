using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSubMenuUI : SubMenuUI
{
    public TMPro.TextMeshProUGUI ErrorText;
    public TMPro.TMP_InputField PlayerNameInputField;
    public SubMenuUI serverMenu;

    int selectedWeaponID;
    string playerName;
    public void OnEnable()
    {
        PlayerNameInputField.onValueChanged.AddListener(SetPlayerName);
        if (PlayerPrefs.HasKey("playerName"))
            PlayerNameInputField.text = PlayerPrefs.GetString("playerName", "");
    }

    public void SelectRifle(bool doIt)
    {
        if (doIt)
        {
            selectedWeaponID = 0;
        }
    }

    public void SelectSniper(bool doIt)
    {
        if (doIt)
        {
            selectedWeaponID = 1;
        }
    }
    public void SelectShotgun(bool doIt)
    {
        if (doIt)
        {
            selectedWeaponID = 2;
        }
    }


    public void Spawn()
    {
        if(playerName == "")
        {
            ErrorText.text = "please choose a name!";
            return;
        }
        PlayerPrefs.SetString("playerName", playerName);
        PlayerPrefs.Save();
        LndConnector.Instance.SpawnPlayer(playerName,selectedWeaponID);
        menu.Dectivate();

    }

    private async void spawnAsync()
    {

    }

    

    public void Back()
    {
        serverMenu.Select();
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    public override void OnSelect()
    {
        base.OnSelect();
        playerName = PlayerPrefs.GetString("playerName", "");
        ErrorText.text = "";
    }
}
