using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputKeyMapping : MonoBehaviour
{
    Dictionary<string, KeyCode> Keys;

    void Awake()
    {
        Keys = new Dictionary<string, KeyCode>();
        LoadAllKeys();
    }

    void LoadAllKeys()
    {
        LoadKey("Aim_Key", "mouse 1");
        LoadKey("Sprint_Key", "left shift");
        LoadKey("Jump_Key", "space");
        LoadKey("Shoot_Key", "mouse 0");
        LoadKey("Menu_Key", "escape");
        LoadKey("Respawn_Key", "space");
        LoadKey("Chat_Key", "t");
        LoadKey("Forward_Key", "w");
        LoadKey("Backward_Key", "s");
        LoadKey("Left_Key", "a");
        LoadKey("Right_Key", "d");
    }

    public void ResetAllKeys()
    {
        foreach(var key in Keys)
        {
            PlayerPrefs.DeleteKey(key.Key);
        }
        LoadAllKeys();
    }

    void LoadKey(string key, string defaultValue)
    {
        Keys[key] = (KeyCode)System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString(key, defaultValue));

    }

    public void SetKey(string key, KeyCode keyCode)
    {
        Keys[key] = keyCode;

        PlayerPrefs.SetString(key, keyCode.ToString());
        PlayerPrefs.Save();
    }

    public KeyCode getKey(string key)
    {
        return Keys[key];
    }
}
