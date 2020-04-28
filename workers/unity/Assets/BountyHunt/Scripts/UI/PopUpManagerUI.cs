using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class PopUpManagerUI : MonoBehaviour
{
    public PopUpUI popUpPrefab;
    public Transform container;

    public static PopUpManagerUI instance;

    private void Awake()
    {
        instance = this;
    }

    public PopUpUI OpenPopUp(PopUpArgs args)
    {
        PopUpUI popup = Instantiate(popUpPrefab, container);
        popup.Set(args.headline, args.showX, args.text, args.actions,args.verticalButtonLayout,args.popUpID);
        return popup;
    }

    public PopUpUI OpenYesNoPopUp(YesNoPopUpArgs args)
    {
        PopUpUI popup = Instantiate(popUpPrefab, container);

        List<LabelAndAction> actions = new List<LabelAndAction>();
        actions.Add(new LabelAndAction("yes", args.yesAction));
        actions.Add(new LabelAndAction("no", args.noAction));

        popup.Set(args.headline, args.showX, args.text, actions,false,args.popUpID);
        return popup;
    }

    public PopUpUI OpenImagePopUp(ImagePopUpArgs args)
    {
        PopUpUI popup = Instantiate(popUpPrefab, container);
        popup.Set(args.headline, args.showX, args.text1, args.sprite, args.text2, args.actions, args.verticalButtonLayout, args.imageSizeMultiplier,args.tintImage, args.popUpID);
        return popup;
    }

    //TEST

    [Space(10)]
    public string header;
    public string text;
    public string text2;
    public Sprite sprite;
    public bool showX;
    public bool verticalButtons;
    public bool btn1;
    public bool btn2;
    public bool btn3;
    public string ID;
    public bool tintImage;
    public float imageSizeMultiplier;

    [Space(10)] [Range(0, 1)]
    public int variant;
    public bool testPopUp;
    public bool testYesNoPopup;
    public bool testImagePopup;
    public bool closeWithID;
    public bool closeAllWithID;
    public bool closeAll;

    private void Update()
    {
        if (testPopUp)
        {
            testPopUp = false;
            List<LabelAndAction> actions = new List<LabelAndAction>();
            if (btn1) actions.Add(new LabelAndAction("btn1",test1));
            if (btn2) actions.Add(new LabelAndAction("button2",test2));
            if (btn3) actions.Add(new LabelAndAction("Auslöser3",test3));

            PopUpArgs args;
            switch (variant)
            {
                case 0: default: args = new PopUpArgs("header",text,actions,verticalButtons,showX, ID);break;
                case 1: args = new PopUpArgs(header,text,ID); break;
            }

            OpenPopUp(args);

        }

        if (testYesNoPopup)
        {
            testYesNoPopup = false;
            YesNoPopUpArgs args = new YesNoPopUpArgs("header", text, testYN, showX,ID);
            OpenYesNoPopUp(args);
        }

        if (testImagePopup)
        {
            testImagePopup = false;
            List<LabelAndAction> actions = new List<LabelAndAction>();
            if (btn1) actions.Add(new LabelAndAction("btn1", test1));
            if (btn2) actions.Add(new LabelAndAction("button2", test2));
            if (btn3) actions.Add(new LabelAndAction("Auslöser3", test3));

            testYesNoPopup = false;
            ImagePopUpArgs args = new ImagePopUpArgs("header", text, sprite, text2,actions,verticalButtons,tintImage,imageSizeMultiplier,showX,ID);
            OpenImagePopUp(args);
        }

        if (closeWithID)
        {
            closeWithID = false;
            PopUpUI.CloseAllWithID(ID);

        }

        if (closeAllWithID)
        {
            closeAllWithID = false;
            PopUpUI.CloseAllWithID(ID);
        }

        if (closeAll)
        {
            closeAll = false;
            PopUpUI.CloseAll();
        }

    }

    void test1()
    {
        Utility.Log("testing answer 1", Color.yellow);
    }
    void test2()
    {
        Utility.Log("testing answer 2", Color.cyan);
    }
    void test3()
    {
        Utility.Log("testing answer 3", Color.blue);
    }

    void testYN(bool yes)
    {
        if(yes)
        Utility.Log("YES!", Color.green);
        else
        Utility.Log("NO!", Color.red);

    }


}

public struct PopUpArgs
{
    public string headline;
    public string text;
    public bool verticalButtonLayout;
    public bool showX;
    public List<LabelAndAction> actions;
    public string popUpID;

    /// <param name="actions">the button labels and their corresponding actions for OnClick()</param>
    /// <param name="verticalButtonlayout">should the buttons be layouted vertical instead of horizontal?</param>
    /// <param name="popUpID">this is used, if you want to need acces to the created popup (such as closing it from another script) </param>
    public PopUpArgs(string headline, string text, List<LabelAndAction> actions, bool verticalButtonlayout, bool showX = true, string popUpID = "")
    {
        this.headline = headline;
        this.text = text;
        this.showX = showX;
        this.actions = actions;
        this.verticalButtonLayout = verticalButtonlayout;
        this.popUpID = popUpID;
    }

    /// <param name="popUpID">this is used, if you want to need acces to the created popup (such as closing it from another script) </param>
    public PopUpArgs(string headline, string text, string popUpID = "")
    {
        this.headline = headline;
        this.text = text;
        this.showX = true;
        this.actions = new List<LabelAndAction>();
        this.verticalButtonLayout = false;
        this.popUpID = popUpID;
    }
}

public struct YesNoPopUpArgs
{
    public string headline;
    public string text;
    public bool showX;
    public UnityAction yesAction;
    public UnityAction noAction;
    public string popUpID;

    /// <param name="action">the action for both buttons, "YES" sends true, "NO" sends false</param>
    /// <param name="popUpID">this is used, if you want to need acces to the created popup (such as closing it from another script) </param>
    public YesNoPopUpArgs(string headline, string text, UnityAction<bool> action, bool showX = true, string popUpID = "")
    {
        this.headline = headline;
        this.text = text;
        this.showX = showX;
        this.yesAction = delegate { action.Invoke(true); };
        this.noAction = delegate { action.Invoke(false); };
        this.popUpID = popUpID;
    }

    /// <param name="yesAction">the action, for the "YES" Button</param>
    /// <param name="noAction">the action, for the "NO" Button</param>
    /// <param name="popUpID">this is used, if you want to need acces to the created popup (such as closing it from another script) </param>
    public YesNoPopUpArgs(string headline, string text, UnityAction yesAction, UnityAction noAction, bool showX = true, string popUpID = "")
    {
        this.headline = headline;
        this.text = text;
        this.showX = showX;
        this.yesAction = yesAction;
        this.noAction = noAction;
        this.popUpID = popUpID;
    }
}

public struct ImagePopUpArgs
{
    public string headline;
    public string text1;
    public string text2;
    public bool verticalButtonLayout;
    public bool showX;
    public List<LabelAndAction> actions;
    public Sprite sprite;
    public bool tintImage;
    public float imageSizeMultiplier;
    public string popUpID;

    /// <param name="text1">text above the image</param>
    /// <param name="text2">text below the image</param>
    /// <param name="actions">the button labels and their corresponding actions for OnClick()</param>
    /// <param name="verticalButtonlayout">should the buttons be layouted vertical instead of horizontal?</param>
    /// <param name="tintImage">should the image be tinted with the primary color? if not, the image color is white</param>
    /// <param name="imageSizeMultiplier">relative size of the image. Size 1 is a square fitted to the panel. Sprite aspect is always preserved.</param>
    /// <param name="popUpID">this is used, if you want to need acces to the created popup (such as closing it from another script) </param>
    public ImagePopUpArgs(string headline, string text1, Sprite sprite, string text2, List<LabelAndAction> actions, bool verticalButtonlayout, bool tintImage, float imageSizeMultiplier = 1, bool showX = true, string popUpID = "")
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
        this.popUpID = popUpID;
    }
}
