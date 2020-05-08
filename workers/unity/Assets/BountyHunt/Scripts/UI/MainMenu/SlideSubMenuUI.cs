using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class SlideSubMenuUI : MonoBehaviour
{
    [HideInInspector]
    public SlideMenuUI menu;

    public UnityEvent onActivate = new UnityEvent();
    public UnityEvent onDeactivate = new UnityEvent();

    [HideInInspector]
    public CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Select()
    {
        menu.SelectSubMenu(this);
    }

    public void Activate()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable =  true;
        canvasGroup.blocksRaycasts = true;
        onActivate.Invoke();
        var refreshables = GetComponentsInChildren<IRefreshableUI>();
        foreach (var refreshable in refreshables)
            refreshable.Refresh();
    }

    public void Deactivate()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        onDeactivate.Invoke();
    }

}
