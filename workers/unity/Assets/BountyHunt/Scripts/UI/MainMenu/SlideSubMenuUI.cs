using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class SlideSubMenuUI : MonoBehaviour
{
    [HideInInspector]
    public SlideMenuUI menu;

    public UnityEvent onActivate;
    public UnityEvent onDeactivate;

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
    }

    public void Deactivate()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        onDeactivate.Invoke();
    }

}
