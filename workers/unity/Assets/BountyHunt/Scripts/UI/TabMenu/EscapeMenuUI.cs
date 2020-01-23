using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeMenuUI : ScreenUI
{
    [SerializeField] List<TabWindowInfo> tabWindowsInfo;
    public int firstSelectedTabID;

    protected override void Awake()
    {
        base.Awake();
        firstSelectedTabID = Mathf.Clamp(firstSelectedTabID, 0, tabWindowsInfo.Count - 1);

        for(int  i = 0; i<tabWindowsInfo.Count;i++)
        {
            TabWindowInfo win = tabWindowsInfo[i];
            win.window.gameObject.SetActive(true);
            win.window.setUpTab(win.tab);
            bool active = i == firstSelectedTabID;
            win.window.OnChangeActive(active);
            win.tab.isOn = active;
        }

    }

}

[System.Serializable]
public class TabWindowInfo
{
    public Toggle tab;
    public TabMenuWindowUI window;
}
