using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputKeyMapping : MonoBehaviour
{
    public SkillDictionary skillDictionary;
    static Dictionary<string, KeyCode> Keys;

    static InputKeyMapping instance;

    void Awake()
    {
        instance = this;
        Keys = new Dictionary<string, KeyCode>();
        LoadAllKeys();
    }

    static void LoadAllKeys()
    {
        LoadKey("Aim_Key", KeyCode.Mouse1);
        LoadKey("Sprint_Key", KeyCode.LeftShift);
        LoadKey("Jump_Key", KeyCode.Space);
        LoadKey("Shoot_Key", KeyCode.Mouse0);
        LoadKey("Menu_Key", KeyCode.Escape);
        LoadKey("Respawn_Key", KeyCode.Space);
        LoadKey("Chat_Key", KeyCode.T);
        LoadKey("Forward_Key", KeyCode.W);
        LoadKey("Backward_Key", KeyCode.S);
        LoadKey("Left_Key", KeyCode.A);
        LoadKey("Right_Key", KeyCode.D);
        LoadKey("ContextAction1_Key", KeyCode.Alpha1);
        LoadKey("ContextAction2_Key", KeyCode.Alpha2);
        LoadKey("ContextAction3_Key", KeyCode.Alpha3);
        LoadKey("ContextAction4_Key", KeyCode.Alpha4);
        LoadKey("ContextAction5_Key", KeyCode.Alpha5);
        LoadKey("ThirdPerson_Key", KeyCode.LeftAlt);

        foreach (PlayerSkill skill in instance.skillDictionary.skills)
        {
            LoadKey(skill.SkillName + "_Key", skill.defaultkey);
        }
    }

    public static void ResetAllKeys()
    {
        foreach(var key in Keys)
        {
            PlayerPrefs.DeleteKey(key.Key);
        }
        LoadAllKeys();
    }

    static void LoadKey(string key, KeyCode defaultCode)
    {
        Keys[key] = (KeyCode)(PlayerPrefs.GetInt(key, (int)defaultCode));
    }

    public static void SetKey(string key, KeyCode keyCode)
    {
        Keys[key] = keyCode;

        PlayerPrefs.SetInt(key, (int)keyCode);
        PlayerPrefs.Save();
    }

    public static KeyCode GetKeyCode(string key)
    {
        return Keys[key];
    }

    public static bool MappedKeyDown(string key)
    {
        return Input.GetKeyDown(Keys[key]);
    }

    public static bool MappedKeyHeld(string key)
    {
        return Input.GetKey(Keys[key]);

    }

    public static bool MappedKeyUp(string key)
    {
        return Input.GetKeyUp(Keys[key]);

    }
}
