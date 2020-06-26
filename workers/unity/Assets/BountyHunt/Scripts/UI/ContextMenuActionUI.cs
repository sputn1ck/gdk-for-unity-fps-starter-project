using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class ContextMenuActionUI : MonoBehaviour
{
    public TextMeshProUGUI labelText;
    public TextMeshProUGUI keyText;
    [HideInInspector] public string key;
    public UnityAction action;
}
