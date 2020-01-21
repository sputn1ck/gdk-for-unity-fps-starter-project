using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(CanvasGroup))]
public class TabMenuWindowUI : MonoBehaviour
{

    [HideInInspector]public Toggle tab;

    public void setUpTab(Toggle tab)
    {
        this.tab = tab;
        tab.onValueChanged.AddListener(OnChangeActive);
    }

    public void OnChangeActive(bool active)
    {
        if (active)
        {
            OnActivate();
        }
        else
        {
            OnDeactivate();
        }
    }


    public virtual void OnActivate()
    {
        GetComponent<CanvasGroup>().alpha = 1;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        GetComponent<CanvasGroup>().interactable = true;
    }
    public virtual void OnDeactivate()
    {
        GetComponent<CanvasGroup>().alpha = 0;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        GetComponent<CanvasGroup>().interactable = false;
    }


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
