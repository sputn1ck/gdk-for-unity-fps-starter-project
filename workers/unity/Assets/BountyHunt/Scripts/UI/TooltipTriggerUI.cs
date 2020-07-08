using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class TooltipTriggerUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string tooltipText;
    [Tooltip ("overrides text if field exists")]
    public string gameTextStringName;
    private bool isShowing;

    private void Awake()
    {
        
        if (gameTextStringName == "") return;
        try
        {
            var field = typeof(GameText).GetField(gameTextStringName);
            tooltipText = (string)field.GetValue(null);
        }
        catch
        {
            Debug.LogError("couldnt find field " + gameTextStringName);
        }
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isShowing = true;
        CursorUI.Instance.SetToolTipText(tooltipText);
    }
    public void OnPointerExit(PointerEventData eventData)
    {

        isShowing = false;
        CursorUI.Instance.RemoveToolTipText();
    }

    public void OnDestroy()
    {
        if (isShowing)
        {
            CursorUI.Instance.RemoveToolTipText();
        }

    }
}
