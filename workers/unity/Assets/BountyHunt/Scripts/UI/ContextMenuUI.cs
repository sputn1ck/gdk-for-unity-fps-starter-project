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

    Object referenceObject;

    public static ContextMenuUI Instance;

    private void Awake()
    {
        gameObject.SetActive(false);
        Instance = this;   
    }

    public void Set(Object reference, string headline, string text, List<(UnityAction action, string label)> actions, Sprite sprite = null, int imageSize = 200)
    {
        gameObject.SetActive(true);
        referenceObject = reference;
        this.headline.text = headline;

        if (text != "") {
            this.text.gameObject.SetActive(true);
            this.text.text = text;
        }
        else this.text.gameObject.SetActive(false);

        if (sprite != null)
        {
            this.image.gameObject.SetActive(true);
            this.image.sprite = sprite;
            this.image.GetComponent<LayoutElement>().preferredHeight = imageSize;
        }
        else this.image.gameObject.SetActive(false);


        for (int i = 0; i < actionUIs.Count; i++)
        {
            if(i < actions.Count)
            {
                actionUIs[i].gameObject.SetActive(true);
                actionUIs[i].labelText.text = actions[i].label;
                actionUIs[i].key = string.Format("ContextAction{}_Key", i + 1);
                actionUIs[i].keyText.text = InputKeyMapping.GetKeyCode(actionUIs[i].key).ToString();
                actionUIs[i].action = actions[i].action;
            }
            else
            {
                actionUIs[i].gameObject.SetActive(false);
            }
        }

    }

    public void Hide(Object reference)
    {
        if (referenceObject!= reference) return;
        gameObject.SetActive(false);
    }

    private void Update()
    {
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
