using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ContextMenuUI : MonoBehaviour
{
    public TextMeshProUGUI headline;
    public TextMeshProUGUI text;
    public Image image;
    public List<ContextMenuActionUI> actionUIs;
    public ContextMenuActionUI closeActionUI;

    public static ContextMenuUI Instance;

    float hideTime;

    private List<ContextMenuArgs> queue = new List<ContextMenuArgs>();
    private ContextMenuArgs currentLookAtMenu;
    private ContextMenuArgs currentMenu;

    const float defaultLifeTime = 10;
    const float defaultLookAtHideDelay = 0;


    private void Awake()
    {
        gameObject.SetActive(false);
        Instance = this;   
    }

    private void OnEnable()
    {
        closeActionUI.keyText.text = GameText.GetKeyName(InputKeyMapping.GetKeyCode("ContextCloseAction_Key"));
        closeActionUI.labelText.text = GameText.ContextMenuCloseActionLabel;
    }

    public void Set(ContextMenuArgs args)
    {
        switch (args.Type)
        {
            case ContextMenuType.QUEUE:
            default:
                AddToQueue(args);
                break;
            case ContextMenuType.REPLACE:
                ReplaceCurrent(args);
                break;
            case ContextMenuType.LOOKAT:
                SetLookAtMenu(args);
                break;
        }

    }

    void AddToQueue(ContextMenuArgs args)
    {
        if(currentMenu!= null && currentMenu.Type != ContextMenuType.LOOKAT)
        {
            queue.Add(args);
        }
        else
        {
            SetCurrent(args);
        }
    }
    void ReplaceCurrent(ContextMenuArgs args)
    {
        
        if (currentMenu != null && currentMenu.CloseAction != null)
        {
            currentMenu.CloseAction.Invoke();
        }

        SetCurrent(args);
    }
    public void SetLookAtMenu(ContextMenuArgs args)
    {
        currentLookAtMenu = args;

        if(currentMenu == null || currentMenu.Type == ContextMenuType.LOOKAT)
        {
            SetCurrent(args);
            hideTime = float.PositiveInfinity;
        }

    }

    public void UnsetLookAtMenu()
    {
        if(currentMenu != null && currentMenu.Type == ContextMenuType.LOOKAT) hideTime = Time.time + currentMenu.LifeTime;
        currentLookAtMenu = null;
    }
    private void SetCurrent(ContextMenuArgs args)
    {
        if(args.LifeTime == -1)
        {
            args.LifeTime = args.Type == ContextMenuType.LOOKAT ? defaultLookAtHideDelay : defaultLifeTime;
        }

        closeActionUI.gameObject.SetActive(args.Type != ContextMenuType.LOOKAT);

        currentMenu = args;

        hideTime = Time.time + args.LifeTime;

        gameObject.SetActive(true);
        this.headline.text = args.Headline;

        if (args.Text != "") {
            this.text.gameObject.SetActive(true);
            this.text.text = args.Text;
        }
        else this.text.gameObject.SetActive(false);

        if (args.ImageSprite != null)
        {
            this.image.gameObject.SetActive(true);
            this.image.sprite = args.ImageSprite;
            this.image.GetComponent<LayoutElement>().preferredHeight = args.ImageSize;
            this.image.color = args.ImageColor;
        }
        else this.image.gameObject.SetActive(false);


        for (int i = 0; i < actionUIs.Count; i++)
        {
            if(i < args.Actions.Count)
            {
                actionUIs[i].gameObject.SetActive(true);
                actionUIs[i].labelText.text = args.Actions[i].label;
                actionUIs[i].key = string.Format("ContextAction{0}_Key", i + 1);
                actionUIs[i].keyText.text = GameText.GetKeyName(InputKeyMapping.GetKeyCode(actionUIs[i].key));
                actionUIs[i].action = args.Actions[i].action;
            }
            else
            {
                actionUIs[i].gameObject.SetActive(false);
            }
        }


        if (currentMenu.OpenAction != null)
        {
            currentMenu.OpenAction.Invoke();
        }
    }

    public void CloseCurrentAndShowNext()
    {
        gameObject.SetActive(false);

        if (currentMenu != null && currentMenu.Type == ContextMenuType.LOOKAT) currentLookAtMenu = null;

        if (currentMenu.CloseAction != null)
        {
            currentMenu.CloseAction.Invoke();
        }
        currentMenu = null;

        if(queue.Count > 0)
        {
            SetCurrent(queue[0]);
            queue.RemoveAt(0);
        }

        else if (currentLookAtMenu != null)
        {
            SetCurrent(currentLookAtMenu);
            hideTime = float.PositiveInfinity;
        }

    }


    private void Update()
    {
        if (Time.time >= hideTime)
        {
            CloseCurrentAndShowNext();
            return;
        }

        if (UnityEngine.EventSystems.EventSystem.current.IsEditingInpputfield()|| ChatPanelUI.instance.chatting) return;

        foreach (ContextMenuActionUI ui in actionUIs)
        {
            if (ui.gameObject.activeSelf)
            {
                if (InputKeyMapping.MappedKeyDown(ui.key))
                {

                    ui.action.Invoke();
                }
            }
        }

        if (InputKeyMapping.MappedKeyDown("ContextCloseAction_Key")&&currentMenu.Type != ContextMenuType.LOOKAT)
        {
            CloseCurrentAndShowNext();
        }

    }

}

public class ContextMenuArgs
{
    public string Headline = "";
    public string Text =  "";
    public Sprite ImageSprite = null;
    public List<(UnityAction action,string label)> Actions  = new List<(UnityAction action, string label)>();
    public Color ImageColor = Color.white;
    public float ImageSize = 200;
    /// <summary>
    /// if type is LOOKAT, this is the delay, it needs to hide after looking away(-1 means default value)
    /// </summary>
    public float LifeTime = -1;
    public UnityAction OpenAction = null;
    public UnityAction CloseAction = null;
    public ContextMenuType Type;
}

public enum ContextMenuType
{
    QUEUE,REPLACE,LOOKAT
}
