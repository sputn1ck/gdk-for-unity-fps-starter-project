using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    public AudioObject audioObjectPrefab;
    public AudioObject audioObject2DPrefab;
    public static AudioManager instance;
    public AudioMixer mixer;

    public AudioClip DamageTakenSound;
    public AudioClip JumpSound;

    Dictionary<VolumeType, VolumeKeys> keys;

    private void Awake()
    {
        if (!instance) instance = this;
        else Destroy(this);


        keys = new Dictionary<VolumeType, VolumeKeys>();
        keys[VolumeType.MASTER] = new VolumeKeys("Master", "Volume_Master");
        keys[VolumeType.MUSIC] = new VolumeKeys("Music", "Volume_Music");
        keys[VolumeType.SFX] = new VolumeKeys("Sfx", "Volume_Sfx");

    }

    private void Start()
    {
        LoadVolumes();
        ClientEvents.instance.onGameJoined.AddListener(OnGameJoined);
    }

    public void spawnSound(AudioClip clip, Vector3 position, float volume=1, float pitch =1)
    {
        AudioObject ao = Instantiate(audioObjectPrefab,position, Quaternion.identity);
        ao.Play(clip,volume,pitch);
    }

    public void spawnSound(AudioClip clip, Transform parent, float volume = 1, float pitch = 1)
    {
        AudioObject ao = Instantiate(audioObjectPrefab, parent);
        ao.Play(clip,volume,pitch);
    }

    public void spawnSound(AudioClip clip, Transform parent, Vector3 position, float volume = 1, float pitch = 1)
    {
        AudioObject ao = Instantiate(audioObjectPrefab, position, Quaternion.identity, parent);
        ao.Play(clip,volume,pitch);
    }

    public void spawn2DSound(AudioClip clip, float volume = 1, float pitch = 1)
    {
        AudioObject ao = Instantiate(audioObject2DPrefab);
        ao.Play(clip,volume,pitch);
    }
    /// <summary>
    /// values from 0 to  100
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    public void SetVolume(VolumeType type, float value)
    {
        PlayerPrefs.SetFloat(keys[type].playerPrefsKey, value);
        PlayerPrefs.Save();
        float volume = Mathf.Log10(Mathf.Clamp(value, 0.01f, 100)/100) * 20;
        mixer.SetFloat(keys[type].mixerKey, volume);
    }
    /// <summary>
    /// values from 0 to 100
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public float GetVolume (VolumeType type)
    {
        return PlayerPrefs.GetFloat(keys[type].playerPrefsKey, 80);
    }

    public void LoadVolumes()
    {
        SetVolume(VolumeType.MASTER, GetVolume(VolumeType.MASTER));
        SetVolume(VolumeType.MUSIC, GetVolume(VolumeType.MUSIC));
        SetVolume(VolumeType.SFX, GetVolume(VolumeType.SFX));
    }


    void OnGameJoined()
    {
        Destroy(GetComponent<AudioListener>());
    }


}

public enum VolumeType
{
    MASTER,MUSIC,SFX
}

public class VolumeKeys
{
    public string mixerKey;
    public string playerPrefsKey;

    public VolumeKeys(string mixerKey, string playerPrefsKey)
    {
        this.mixerKey = mixerKey;
        this.playerPrefsKey = playerPrefsKey;
    }
}
