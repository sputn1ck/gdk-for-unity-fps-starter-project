using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsMenuUI : MonoBehaviour
{
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public SlideSubMenuUI subMenu;
    void Start()
    {
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);
        Refresh();
        subMenu.onActivate.AddListener(Refresh);
    }

    void Refresh()
    {
        masterVolumeSlider.value = AudioManager.instance.GetVolume(VolumeType.MASTER);
        musicVolumeSlider.value = AudioManager.instance.GetVolume(VolumeType.MUSIC);
        sfxVolumeSlider.value = AudioManager.instance.GetVolume(VolumeType.SFX);
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
}
