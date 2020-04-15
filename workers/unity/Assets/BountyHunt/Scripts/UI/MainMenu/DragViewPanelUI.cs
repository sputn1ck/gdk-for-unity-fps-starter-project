using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragViewPanelUI : MonoBehaviour, IDragHandler
{

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("dragging");
        PreviewSpot.Instance.TurnCharacter(-eventData.delta.x / Screen.width * 360);

    }

}
