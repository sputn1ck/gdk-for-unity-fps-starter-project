using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public SubMenuUI startSubMenu;
    SubMenuUI selectedSubMenu;

    public List<SubMenuUI> allSubMenus =  new List<SubMenuUI>();

    private void Start()
    {
        foreach (SubMenuUI smui in allSubMenus)
        {
            smui.menu = this;
            smui.gameObject.SetActive(false);
        }
        Select(startSubMenu);
    }
    public void Reset()
    {
        Select(allSubMenus[0]);
    }
    public void Select(SubMenuUI subMenu)
    {
        if (!allSubMenus.Contains(subMenu))
        {
            allSubMenus.Add(subMenu);
        }

        if (selectedSubMenu != null)
        {
            selectedSubMenu.OnDeselect();
        }

        if(subMenu != null)subMenu.OnSelect();
        selectedSubMenu = subMenu;

    }

    public void AddSubMenu(SubMenuUI subMenu)
    {
        if (allSubMenus.Contains(subMenu)) return;
        allSubMenus.Add(subMenu);
        subMenu.menu = this;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Dectivate()
    {
        gameObject.SetActive(false);
    }

}
