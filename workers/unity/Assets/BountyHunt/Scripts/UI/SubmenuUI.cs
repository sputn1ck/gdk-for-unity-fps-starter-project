using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMenuUI : MonoBehaviour
{
    [HideInInspector]
    public MenuUI menu;

    public void Select()
    {
        menu.Select(this);
    }

    public virtual void OnSelect()
    {
        gameObject.SetActive(true);
    }

    public virtual void OnDeselect()
    {
        gameObject.SetActive(false);
    }

}
