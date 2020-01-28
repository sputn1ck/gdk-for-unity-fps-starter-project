using Fps.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RespawnScreenUI : ScreenUI
{
    public Toggle RifleToggle;
    public Toggle SniperToggle;
    public Toggle ShotGunToggle;
    public Button respawnButton;
    private bool isOn;
    // Start is called before the first frame update
    private void OnEnable()
    {
        RifleToggle.onValueChanged.AddListener(SelectRifle);
        SniperToggle.onValueChanged.AddListener(SelectSniper);
        ShotGunToggle.onValueChanged.AddListener(SelectShotgun);
        isOn = true;
        //TODO
        //respawnButton.onClick.AddListener(DonnerPlayerAuthorative.instance.GetComponent<DonnerFpsDriver>().Respawn);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnDisable()
    {
        RifleToggle.onValueChanged.RemoveAllListeners();
        SniperToggle.onValueChanged.RemoveAllListeners();
        ShotGunToggle.onValueChanged.RemoveAllListeners();
        isOn = false;
        respawnButton.onClick.RemoveAllListeners();
    }

    public void SelectRifle(bool doIt)
    {
        if (doIt)
        {
            FpsDriver.instance.ChangeGunId(0);
        }
    }

    public void SelectSniper(bool doIt)
    {
        if (doIt)
        {
            FpsDriver.instance.ChangeGunId(1);
        }
    }
    public void SelectShotgun(bool doIt)
    {
        if (doIt)
        {
            FpsDriver.instance.ChangeGunId(2);
        }
    }
}
