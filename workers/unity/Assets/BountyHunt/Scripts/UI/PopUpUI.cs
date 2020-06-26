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
    public GameObject inputfieldContainer;
    public TextMeshProUGUI preInputText;
    public TMP_InputField inputField;
    public TextMeshProUGUI postInputText;


    [HideInInspector] public UnityAction closeAction;
    [HideInInspector] public List<Button> buttons;


    public void Set(string headline, bool showX, string text, List<PopUpButtonArgs> buttonActions, bool verticalLayoutedButtons, UnityAction closeAction = null)
    {
        this.closeAction = closeAction;
        image.gameObject.SetActive(false);
        inputfieldContainer.SetActive(false);

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

    public void Set(string headline, bool showX, string text, List<InputPopUpButtonArgs> buttonActions, bool verticalLayoutedButtons, string preInputFieldText, string postInputFieldText, TMP_InputField.ContentType contentType, TextAlignmentOptions alignment, string defaultInputText, string placeholderText , UnityAction closeAction = null)
    {
        Set(headline, showX, text, new List<PopUpButtonArgs>(), verticalLayoutedButtons, closeAction) ;

        inputfieldContainer.SetActive(true);
        inputField.contentType = contentType;
        inputField.text = defaultInputText;
        (inputField.placeholder as TextMeshProUGUI).text = placeholderText;

        inputField.textComponent.alignment = alignment;
        (inputField.placeholder as TextMeshProUGUI).alignment = alignment;

        if (preInputFieldText != "") preInputText.text = preInputFieldText;
        else preInputText.gameObject.SetActive(false);

        if (postInputFieldText != "") postInputText.text = postInputFieldText;
        else postInputText.gameObject.SetActive(false);

        Transform btnContainer;
        if (buttonActions.Count == 0)
        {
            horizontalButtonsContainer.gameObject.SetActive(false);
            verticalButtonsContainer.gameObject.SetActive(false);
            return;
        }
        else if (verticalLayoutedButtons)
        {
            verticalButtonsContainer.gameObject.SetActive(true);
            horizontalButtonsContainer.gameObject.SetActive(false);
            btnContainer = verticalButtonsContainer;
        }
        else
        {
            horizontalButtonsContainer.gameObject.SetActive(true);
            verticalButtonsContainer.gameObject.SetActive(false);
            btnContainer = horizontalButtonsContainer;
        }

        foreach (InputPopUpButtonArgs puba in buttonActions)
        {
            Button b = Instantiate(buttonPrefab, btnContainer);
            b.GetComponentInChildren<TextMeshProUGUI>().text = puba.label;
            b.onClick.AddListener(() => puba.action(inputField.text));
            if (puba.closePopupOnClick) b.onClick.AddListener(Close);
            this.buttons.Add(b);
        }
    }


    public void Close()
    {
        Close(true);
    }
    public void Close(bool runCloseAction)
    {
        if(runCloseAction)
            closeAction?.Invoke();
        Destroy(gameObject);
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

public class InputPopUpButtonArgs
{
    public string label;
    public UnityAction<string> action;
    public bool closePopupOnClick;
    public InputPopUpButtonArgs(string label, UnityAction<string> action, bool closePopupOnClick = true)
    {
        this.label = label;
        this.action = action;
        this.closePopupOnClick = closePopupOnClick;
    }
}

