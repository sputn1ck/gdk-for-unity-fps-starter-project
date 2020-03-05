using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DebugUiPanel : MonoBehaviour
{
    public TextMeshProUGUI HeaderText;
    public TextMeshProUGUI ValueText;


    public void UpdateText(string header, string vaule)
    {
        HeaderText.text = header;
        ValueText.text = vaule;
    }
}
