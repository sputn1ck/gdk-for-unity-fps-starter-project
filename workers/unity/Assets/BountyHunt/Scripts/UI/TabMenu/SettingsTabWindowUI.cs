using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsTabWindowUI : TabMenuWindowUI
{
    //audio
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public AudioMixer mixer;

    //Logs
    public Toggle playerChatVisibilityToggle;
    public Toggle infoLogVisibilityToggle;
    public Toggle errorLogVisibilityToggle;
    public Toggle debugLogVisibilityToggle;

    //Inputs
    public Slider mouseSpeedSlider;


    private void Start()
    {
        playerChatVisibilityToggle.isOn = PlayerPrefs.GetInt("ShowPlayerChat", 1) != 0;
        infoLogVisibilityToggle.isOn = PlayerPrefs.GetInt("ShowInfoLog", 1) != 0;
        errorLogVisibilityToggle.isOn = PlayerPrefs.GetInt("ShowErrorLog", 1) != 0;
        debugLogVisibilityToggle.isOn = PlayerPrefs.GetInt("ShowDebugLog", 0) != 0;


        playerChatVisibilityToggle.onValueChanged.AddListener(SetPlayerChatVisibility);
        infoLogVisibilityToggle.onValueChanged.AddListener(SetInfoLogVisibility);
        errorLogVisibilityToggle.onValueChanged.AddListener(SetErrorLogVisibility);
        debugLogVisibilityToggle.onValueChanged.AddListener(SetDebugLogVisibility);


        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);
        masterVolumeSlider.value = AudioManager.instance.GetVolume(VolumeType.MASTER);
        musicVolumeSlider.value = AudioManager.instance.GetVolume(VolumeType.MUSIC);
        sfxVolumeSlider.value = AudioManager.instance.GetVolume(VolumeType.SFX);


        //mouseSpeedSlider.onValueChanged.AddListener(SetSensitivity);
        mouseSpeedSlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 1);
    }

    private void SetMasterVolume(float vol)
    {
        AudioManager.instance.SetVolume(VolumeType.MASTER, vol);
    }

    private void SetMusicVolume(float vol)
    {
        AudioManager.instance.SetVolume(VolumeType.MUSIC, vol);
    }

    private void SetSfxVolume(float vol)
    {
        AudioManager.instance.SetVolume(VolumeType.SFX, vol);
    }
    /*
    public void SetSensitivity(float sens)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", sens);
        PlayerPrefs.Save();
        KeyboardControls.Instance.sensitivity = sens;
    }
    */
    private void SetPlayerChatVisibility(bool show)
    {
        PlayerPrefs.SetInt("ShowPlayerChat", show ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void SetInfoLogVisibility(bool show)
    {
        PlayerPrefs.SetInt("ShowInfoLog", show ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void SetErrorLogVisibility(bool show)
    {
        PlayerPrefs.SetInt("ShowErrorLog", show ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void SetDebugLogVisibility(bool show)
    {
        PlayerPrefs.SetInt("ShowDebugLog", show ? 1 : 0);
        PlayerPrefs.Save();
    }

}
