using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HoverDescriptionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string descriptionString;
    public TextMeshProUGUI descriptionText;


    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionText.text = descriptionString;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionText.text = "";
    }
}
