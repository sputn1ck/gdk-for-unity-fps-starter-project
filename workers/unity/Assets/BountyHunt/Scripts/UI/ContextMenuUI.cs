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
    public float LookAtHideDelay = 1f;

    public static ContextMenuUI Instance;

    float hideTime;

    UnityAction OpenAction;
    UnityAction CloseAction;

    private List<ContextMenuArgs> queue = new List<ContextMenuArgs>();
    private ContextMenuArgs currentLookAtMenu;
    private ContextMenuArgs currentMenu;

    
    private void Awake()
    {
        gameObject.SetActive(false);
        Instance = this;   
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
        SetCurrent(args);

    }
    public void SetLookAtMenu(ContextMenuArgs args)
    {
        currentLookAtMenu = args;

        if(currentMenu == null || currentMenu.Type == ContextMenuType.LOOKAT)
        {
            SetCurrent(args);
        }
    }

    public void UnsetLookAtMenu()
    {
        currentLookAtMenu = null;
        hideTime = Time.time + LookAtHideDelay;
    }
    private void SetCurrent(ContextMenuArgs args)
    {
        currentMenu = args;

        hideTime = Time.time + args.lifeTime;

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

        OpenAction = args.OpenAction;
        CloseAction = args.CloseAction;

        if (OpenAction != null)
        {
            OpenAction.Invoke();
        }
    }

    public void CloseCurrent()
    {
        currentMenu = null;
        gameObject.SetActive(false);

        if (currentMenu != null && currentMenu.Type == ContextMenuType.LOOKAT) currentLookAtMenu = null;

        if (CloseAction != null)
        {
            CloseAction.Invoke();
        }

        if(queue.Count > 0)
        {
            SetCurrent(queue[0]);
            queue.RemoveAt(0);
        }

        else if (currentLookAtMenu != null)
        {
            SetCurrent(currentLookAtMenu);
        }

    }


    public void UpdateAll(ContextMenuArgs args)
    {
        if (this.reference != args.ReferenceString) return;
        Hide();
        Set(args);
    }

    private void Update()
    {
        if (Time.time >= hideTime)
        {
            CloseCurrent();
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

        if (InputKeyMapping.MappedKeyDown("ContextCloseAction_Key"))
        {
            CloseCurrent();
        }

    }

    [ContextMenuItem("Test queueElement", "TestQueue")]
    public string testString;

    public void TestQueue()
    {
        ContextMenuArgs args = new ContextMenuArgs
        {
            Headline = testString,
            Text = "random " + Random.Range(1000, 9999),
            Type = ContextMenuType.QUEUE
        };
        Instance.Set(args);
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
    public float lifeTime = 10;
    public UnityAction OpenAction = null;
    public UnityAction CloseAction = null;
    public ContextMenuType Type;
}

public enum ContextMenuType
{
    QUEUE,REPLACE,LOOKAT
}
