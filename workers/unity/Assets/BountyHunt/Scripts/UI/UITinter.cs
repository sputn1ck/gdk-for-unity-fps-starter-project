using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

[System.Serializable] public enum TintColor
{
    Primary,Secondary
}

[ExecuteInEditMode]
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
        updateColor();
    }
}
