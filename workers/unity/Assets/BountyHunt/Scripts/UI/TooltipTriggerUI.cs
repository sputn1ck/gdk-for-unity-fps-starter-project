using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class TooltipTriggerUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    public string tooltipText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorUI.Instance.SetToolTipText(tooltipText);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        CursorUI.Instance.RemoveToolTipText();
    }
}
