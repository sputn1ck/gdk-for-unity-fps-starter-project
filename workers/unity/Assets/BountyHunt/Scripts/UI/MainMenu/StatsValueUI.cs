using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class StatsValueUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI valueText;

    public void Set(string name, string value)
    {
        nameText.text = name;
        valueText.text = value;
    }

    public void UpdateValue(string value)
    {
        valueText.text = value;
    }
}
