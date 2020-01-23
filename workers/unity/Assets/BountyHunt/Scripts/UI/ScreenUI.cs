using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ScreenUI : MonoBehaviour
{
    public bool showing { get; private set; }

    /// <summary>
    /// for the screenManager to store a State
    /// </summary>
    [HideInInspector] public bool activated;

    CanvasGroup _group;
    CanvasGroup group
    {
        get
        {
            if (!_group) _group = GetComponent<CanvasGroup>();
            return _group;
        }
    }

    protected virtual void Awake()
    {
        activated = false;
        showing = false;
        group.alpha = 0;
        group.blocksRaycasts = false;
        group.interactable = false;
    }

    public void Show(bool show)
    {
        if (show == showing) return;
        showing = show;

        if (show)
        {
            group.alpha = 1;
            group.blocksRaycasts = true;
            group.interactable = true;
            OnShow();
        }

        else
        {
            group.alpha = 0;
            group.blocksRaycasts = false;
            group.interactable = false;
            OnHide();
        }
    }


    protected virtual void OnShow()
    {

    }

    protected virtual void OnHide()
    {

    }

}
