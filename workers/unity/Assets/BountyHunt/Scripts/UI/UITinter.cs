using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

[System.Serializable] public enum TintColor
{
    Primary,Panel,MenuPanel,Secondary,Input,HighlightedPanel,Error,Sats
}

//[ExecuteInEditMode]
public class UITinter : MonoBehaviour
{
    public static UnityEvent tintEvent =  new UnityEvent();
    public static Dictionary<TintColor, Color> tintDict = new Dictionary<TintColor, Color>();

    public TintColor tint;

    void updateColor()
    {
        if (!tintDict.ContainsKey(tint)) return;

        Color color = tintDict[tint];

        Image image = GetComponent<Image>();
        if (image)
        {
            image.color = color;
            return;
        }

        TextMeshProUGUI text = GetComponent <TextMeshProUGUI>();
        if (text)
        {
            text.color = color;
        }
    }

    private void OnEnable()
    {
        tintEvent.AddListener(updateColor);
        updateColor();
    }

    private void OnDisable()
    {
        tintEvent.RemoveListener(updateColor);
    }

    private void OnValidate()
    {
        //updateColor();
    }

    public static void setColor(TintColor tint, Color color)
    {
        tintDict[tint] = color;
        string code = "UICOLOR_" + (int)tint;
        PlayerPrefs.SetString(code, Utility.ColorToHex(color));
        PlayerPrefs.Save();
        tintEvent.Invoke();
    }

    public void InitializeAll()
    {
        foreach(var t in tintDict)
        {
            string code = "UICOLOR_" + (int)tint;
            Color c = Utility.HexToColor(PlayerPrefs.GetString(code, "ffffffff"));
            tintDict[t.Key] = c;
        }
        tintEvent.Invoke();
    }

}
[System.Serializable]
public struct TintingPair
{
    public TintColor tint;
    public Color color;
}
