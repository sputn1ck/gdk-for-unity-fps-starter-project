using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class PopUpManagerUI : MonoBehaviour
{
    public PopUpUI popUpPrefab;
    public ModularPopUpUI modularPopUpPrefab;

    public Transform container;



    public static PopUpManagerUI instance;
    private void Awake()
    {
        instance = this;
    }

    public PopUpUI OpenPopUp(PopUpArgs args)
    {
        PopUpUI popup = Instantiate(popUpPrefab, container);
        popup.Set(args.headline, args.showX, args.text, args.actions,args.verticalButtonLayout, args.closeAction);
        return popup;
    }

    public PopUpUI OpenYesNoPopUp(YesNoPopUpArgs args)
    {
        PopUpUI popup = Instantiate(popUpPrefab, container);

        List<PopUpButtonArgs> actions = new List<PopUpButtonArgs>();
        actions.Add(new PopUpButtonArgs("yes", args.yesAction));
        actions.Add(new PopUpButtonArgs("no", args.noAction));

        popup.Set(args.headline, args.showX, args.text, actions,false, args.closeAction);
        return popup;
    }

    public PopUpUI OpenImagePopUp(ImagePopUpArgs args)
    {
        PopUpUI popup = Instantiate(popUpPrefab, container);
        popup.Set(args.headline, args.showX, args.text1, args.sprite, args.text2, args.actions, args.verticalButtonLayout, args.imageSizeMultiplier,args.tintImage, args.closeAction);
        return popup;
    }

    public PopUpUI OpenInputFieldPopUp(InputFieldPopUpArgs args)
    {
        PopUpUI popup = Instantiate(popUpPrefab, container);
        popup.Set(args.headline, args.showX, args.text, args.actions, args.verticalButtonLayout, args.preInputFieldText, args.postInputFieldText, args.contentType,args.alignment,args.defaultInputFieldText,args.placeholderText, args.closeAction);
        return popup;
    }

   public ModularPopUpUI OpenModularPopUp(string headline,List<IPopupElement> elements,bool showX = true, UnityAction closeAction = null)
    {
        ModularPopUpUI popup = Instantiate(modularPopUpPrefab, container);
        popup.SetUp(headline,showX,elements,closeAction);
        return popup;
    }

    public bool test;

    private void Update()
    {
        if (!test) return;

        test = false;

        List<IPopupElement> elements = new List<IPopupElement>();
        elements.Add(new ButtonPopupElement
        {
            args = new PopUpButtonArgs("I bims 1 Button",() => Utility.Log("I-bims-Button pressed",Color.green))
        }) ;
        elements.Add(new TextPopupElement
        {
            text = "I bims ein Text"
        });
        elements.Add(new ImagePopupElement
        {
            scale = 0.2f
        }) ;

        OpenModularPopUp("PopUp yeah!", elements);
    }

}

public struct PopUpArgs
{
    public string headline;
    public string text;
    public bool verticalButtonLayout;
    public bool showX;
    public List<PopUpButtonArgs> actions;
    public UnityAction closeAction;

    /// <param name="actions">the button labels and their corresponding actions for OnClick()</param>
    /// <param name="verticalButtonlayout">should the buttons be layouted vertical instead of horizontal?</param>
    public PopUpArgs(string headline, string text, List<PopUpButtonArgs> actions, bool verticalButtonlayout, bool showX = true, UnityAction closeAction = null)
    {
        this.headline = headline;
        this.text = text;
        this.showX = showX;
        this.actions = actions;
        this.verticalButtonLayout = verticalButtonlayout;
        this.closeAction = closeAction;
    }

    public PopUpArgs(string headline, string text, bool showX = true, UnityAction closeAction = null)
    {
        this.headline = headline;
        this.text = text;
        this.showX = showX;
        this.actions = new List<PopUpButtonArgs>();
        this.verticalButtonLayout = false;
        this.closeAction = closeAction;
    }
}

public struct YesNoPopUpArgs
{
    public string headline;
    public string text;
    public bool showX;
    public UnityAction yesAction;
    public UnityAction noAction;
    public UnityAction closeAction;
    /// <param name="action">the action for both buttons, "YES" sends true, "NO" sends false</param>
    public YesNoPopUpArgs(string headline, string text, UnityAction<bool> action, bool showX = true, UnityAction closeAction = null)
    {
        this.headline = headline;
        this.text = text;
        this.showX = showX;
        this.yesAction = delegate { action.Invoke(true); };
        this.noAction = delegate { action.Invoke(false); };
        this.closeAction = closeAction;
    }

    /// <param name="yesAction">the action, for the "YES" Button</param>
    /// <param name="noAction">the action, for the "NO" Button</param>
    /// <param name="popUpID">this is used, if you want to need acces to the created popup (such as closing it from another script) </param>
    public YesNoPopUpArgs(string headline, string text, UnityAction yesAction, UnityAction noAction, bool showX = true, UnityAction closeAction = null)
    {
        this.headline = headline;
        this.text = text;
        this.showX = showX;
        this.yesAction = yesAction;
        this.noAction = noAction;
        this.closeAction = closeAction;
    }
}

public struct ImagePopUpArgs
{
    public string headline;
    public string text1;
    public string text2;
    public bool verticalButtonLayout;
    public bool showX;
    public List<PopUpButtonArgs> actions;
    public Sprite sprite;
    public bool tintImage;
    public float imageSizeMultiplier;
    public UnityAction closeAction;
    /// <param name="text1">text above the image</param>
    /// <param name="text2">text below the image</param>
    /// <param name="actions">the button labels and their corresponding actions for OnClick()</param>
    /// <param name="verticalButtonlayout">should the buttons be layouted vertical instead of horizontal?</param>
    /// <param name="tintImage">should the image be tinted with the primary color? if not, the image color is white</param>
    /// <param name="imageSizeMultiplier">relative size of the image. Size 1 is a square fitted to the panel. Sprite aspect is always preserved.</param>
    public ImagePopUpArgs(string headline, string text1, Sprite sprite, string text2, List<PopUpButtonArgs> actions, bool verticalButtonlayout, bool tintImage, float imageSizeMultiplier = 1, bool showX = true, UnityAction closeAction = null)
    {
        this.headline = headline;
        this.text1 = text1;
        this.text2 = text2;
        this.showX = showX;
        this.actions = actions;
        this.verticalButtonLayout = verticalButtonlayout;
        this.sprite = sprite;
        this.tintImage = tintImage;
        this.imageSizeMultiplier = imageSizeMultiplier;
        this.closeAction = closeAction;
    }

}

public struct InputFieldPopUpArgs
{
    public string headline;
    public string text;
    public bool verticalButtonLayout;
    public bool showX;
    public List<InputPopUpButtonArgs> actions;
    public UnityAction closeAction;
    public string preInputFieldText;
    public string postInputFieldText;
    public string defaultInputFieldText;
    public string placeholderText;
    public TMP_InputField.ContentType contentType;
    public TextAlignmentOptions alignment;

    /// <param name="actions">the button labels and their corresponding actions with a string as argument (from inputfield)</param>
    /// <param name="verticalButtonlayout">should the buttons be layouted vertical instead of horizontal?</param>
    /// <param name="preInputFieldText">text before the input field</param>
    /// <param name="postInputFieldText">text after the input field</param>
    public InputFieldPopUpArgs(string headline, string text, List<InputPopUpButtonArgs> actions, bool verticalButtonlayout, string preInputFieldText, string postInputFieldText, string defaultInputFieldText, TextAlignmentOptions alignment, TMP_InputField.ContentType contentType = TMP_InputField.ContentType.Alphanumeric, bool showX = true, UnityAction closeAction = null, string placeholderText = "")
    {
        this.headline = headline;
        this.text = text;
        this.showX = showX;
        this.actions = actions;
        this.verticalButtonLayout = verticalButtonlayout;
        this.closeAction = closeAction;
        this.preInputFieldText = preInputFieldText;
        this.postInputFieldText = postInputFieldText;
        this.defaultInputFieldText = defaultInputFieldText;
        this.placeholderText = placeholderText;
        this.alignment = alignment;
        this.contentType = contentType;
    }
}

