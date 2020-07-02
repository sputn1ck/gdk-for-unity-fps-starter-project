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

    string reference;

    public static ContextMenuUI Instance;

    float hideTime;

    UnityAction OpenAction;
    UnityAction CloseAction;

    private void Awake()
    {
        gameObject.SetActive(false);
        Instance = this;   
    }

    /// <summary>
    /// sets up the context menu, aborts if there is a contextmenu already
    /// </summary>
    /// <param name="args"></param>
    /// <returns>true if not aborted</returns>
    public bool Set(ContextMenuArgs args)
    {
        if (gameObject.activeSelf) return false;

        hideTime = Time.time + args.lifeTime;

        gameObject.SetActive(true);
        reference = args.ReferenceString;
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
        return true;
    }

    public void Hide(string reference)
    {
        if (this.reference!= reference) return;
        Hide();

    }

    private void Hide()
    {
        gameObject.SetActive(false);
        if (CloseAction != null)
        {
            CloseAction.Invoke();
        }
    }

    public void UpdateText(string text, string reference)
    {
        if (this.reference != reference) return;
        this.text.text = text;
    }


    private void Update()
    {
        if (Time.time >= hideTime)
        {
            Hide();
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

    }

}

public class ContextMenuArgs
{
    public string ReferenceString;
    public string Headline = "";
    public string Text =  "";
    public Sprite ImageSprite = null;
    public List<(UnityAction action,string label)> Actions  = new List<(UnityAction action, string label)>();
    public Color ImageColor = Color.white;
    public float ImageSize = 200;
    public float lifeTime = 10;
    public UnityAction OpenAction = null;
    public UnityAction CloseAction = null;
}
