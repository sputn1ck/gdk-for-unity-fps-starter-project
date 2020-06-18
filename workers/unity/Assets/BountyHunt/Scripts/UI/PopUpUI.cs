using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopUpUI : MonoBehaviour
{
    public TextMeshProUGUI headline;
    public TextMeshProUGUI text;
    public Image image;
    public Transform horizontalButtonsContainer;
    public Transform verticalButtonsContainer;
    public Button buttonPrefab;
    public Button xButton;

    public UnityAction closeAction;
    [HideInInspector] public List<Button> buttons;

    static List<PopUpUI> allPopUps = new List<PopUpUI>();

    public void Set(string headline, bool showX, string text, List<PopUpButtonArgs> buttonActions, bool verticalLayoutedButtons, UnityAction closeAction = null)
    {
        this.closeAction = closeAction;
        allPopUps.Add(this);
        image.gameObject.SetActive(false);

        this.headline.text = headline;

        if (text == "") this.text.gameObject.SetActive(false);
        this.text.text = text;

        if (showX) xButton.onClick.AddListener(Close);
        else xButton.gameObject.SetActive(false);


        Transform btnContainer;
        if (buttonActions.Count == 0)
        {
            horizontalButtonsContainer.gameObject.SetActive(false);
            verticalButtonsContainer.gameObject.SetActive(false);
            return;
        }
        else if (verticalLayoutedButtons)
        {
            horizontalButtonsContainer.gameObject.SetActive(false);
            btnContainer = verticalButtonsContainer;
        }
        else
        {
            verticalButtonsContainer.gameObject.SetActive(false);
            btnContainer = horizontalButtonsContainer;
        }

        foreach (PopUpButtonArgs puba in buttonActions)
        {
            Button b = Instantiate(buttonPrefab, btnContainer);
            b.GetComponentInChildren<TextMeshProUGUI>().text = puba.label;
            b.onClick.AddListener(puba.action);
            if (puba.closePopupOnClick) b.onClick.AddListener(Close);
            this.buttons.Add(b);
        }
        
    }

    public void Set(string headline, bool showX, string text1,Sprite sprite, string text2, List<PopUpButtonArgs> buttonActions, bool verticalLayoutedButtons, float imageSizeMultiplier, bool tintImage, UnityAction closeAction = null)
    {
        Set(headline, showX, text1, buttonActions, verticalLayoutedButtons, closeAction);

        image.gameObject.SetActive(true);
        image.sprite = sprite;
        image.GetComponent<LayoutElement>().preferredHeight *= imageSizeMultiplier;

        if (!tintImage)
        {
            image.GetComponent<UITinter>().enabled = false;
            image.color = Color.white;
        }


        if (string.IsNullOrEmpty(text2));
        {
            TextMeshProUGUI textMesh2 = Instantiate(this.text,this.text.transform.parent);
            textMesh2.transform.SetSiblingIndex(2);
            textMesh2.gameObject.SetActive(true);
            textMesh2.text = text2;
        }
    }

    public void Close()
    {
        
        closeAction?.Invoke();
        allPopUps.Remove(this);
        Destroy(gameObject);
    }
    public void Close(bool runCloseAction)
    {
        if(runCloseAction)
            closeAction?.Invoke();
        allPopUps.Remove(this);
        Destroy(gameObject);
    }
    public static void CloseAll()
    {
        for (int i = allPopUps.Count - 1; i >= 0; i--)
        {
            allPopUps[i].Close();
        }
    }

}

public class PopUpButtonArgs
{
    public string label;
    public UnityAction action;
    public bool closePopupOnClick;
    public PopUpButtonArgs(string label, UnityAction action, bool closePopupOnClick = true)
    {
        this.label = label;
        this.action = action;
        this.closePopupOnClick = closePopupOnClick;
    }
}

