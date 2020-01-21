using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof (TextMeshProUGUI))]
public class TextSetUI : MonoBehaviour
{
    public string type;
    TextMeshProUGUI label;

    private void Awake()
    {
        label = GetComponent<TextMeshProUGUI>();
    }

    public void SetText(string text)
    {
        label.text = text;
    }

    public void SetTextInt(float value)
    {
        if (type == "float")
        {
            label.text = value.ToString("F2");
        }
        else
        {
            label.text = ((int)value).ToString();
        }

    }
}
