using Fps.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using Bbhrpc;

public class RespawnScreenUI : ScreenUI
{
    public Toggle RifleToggle;
    public Toggle SniperToggle;
    public Toggle ShotGunToggle;
    public Button respawnButton;
    public TextMeshProUGUI respawnTimerText;
    public int respawnCooldown = 5;
    public int forceRespawnCooldownAddition = 10;
    public Image killerBadge;
    public TextMeshProUGUI killerNameText;
    public List<RespawnScreenAdUI> adObjects;
    public int adsCount;


    public bool alreadyRespawned;

    private bool isOn;

    
    bool waiting;

    private void Awake()
    {
        base.Awake();

        ClientEvents.instance.onPlayerDie.AddListener(StartCooldown);
        ClientEvents.instance.onPlayerDie.AddListener(RefreshAds);
        ClientEvents.instance.onPlayerKilled.AddListener(SetKiller);
        respawnButton.onClick.AddListener(Respawn);
        
    }

    protected override void OnShow()
    {
        int gunID = FpsDriver.instance.GetGunId();
        switch (gunID)
        {
            case 0:
            default:
                RifleToggle.isOn = false;
                RifleToggle.isOn = true;
                break;
            case 1:
                SniperToggle.isOn = false;
                SniperToggle.isOn = true;

                break;
            case 2:
                ShotGunToggle.isOn = false;
                ShotGunToggle.isOn = true;
                break;
        }

        RifleToggle.onValueChanged.AddListener(SelectRifle);
        SniperToggle.onValueChanged.AddListener(SelectSniper);
        ShotGunToggle.onValueChanged.AddListener(SelectShotgun);

        isOn = true;

        //TODO
        //respawnButton.onClick.AddListener(DonnerPlayerAuthorative.instance.GetComponent<DonnerFpsDriver>().Respawn);
    }


    protected override void OnHide()
    {
        RifleToggle.onValueChanged.RemoveAllListeners();
        SniperToggle.onValueChanged.RemoveAllListeners();
        ShotGunToggle.onValueChanged.RemoveAllListeners();
        isOn = false;
    }


    async public void StartCooldown()
    {
        waiting = true;
        respawnButton.gameObject.SetActive(false);
        respawnTimerText.gameObject.SetActive(true);

        for (int i = respawnCooldown; i > 0; i--)
        {
            respawnTimerText.text = i + " sec";
            await Task.Delay(1000);
        }

        alreadyRespawned = false;
        respawnTimerText.gameObject.SetActive(false);
        waiting = false;
        respawnButton.gameObject.SetActive(true);

        await Task.Delay(forceRespawnCooldownAddition *1000);

        if (!alreadyRespawned) Respawn();
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

    public void Respawn()
    {
        if (!FpsDriver.instance || waiting) return;
        waiting = true;
        FpsDriver.instance.Respawn();
        alreadyRespawned = true;
    }

    public void RefreshAds()
    {
        Advertiser[] advs = ClientAdManagerBehaviour.instance.GetRandomAdvertisers(adsCount);
        int i = 0;
        foreach(RespawnScreenAdUI Ad in adObjects)
        {
            if (i < advs.Length)
            {
                Ad.Set(advs[i]);
            }
            else {
                Ad.Set(null);
            }
            i++;
        }
    }

    void SetKiller(PlayerKilledArgs args)
    {
        Badge badge = BadgeManager.GetBadge(args.killerRanking.GlobalRanking.Badge);
        killerBadge.sprite = badge.sprite;
        killerBadge.color = badge.color;
        killerNameText.text = args.killerRanking.Name;
    }
}
