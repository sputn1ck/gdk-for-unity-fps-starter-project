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
    private void Start()
    {
        ClientEvents.instance.onPopUp.AddListener(OpenPopUp);
        ClientEvents.instance.onYesNoPopUp.AddListener(OpenYesNoPopUp);
        ClientEvents.instance.onImagePopUp.AddListener(OpenImagePopUp);
    }

    public void OpenPopUp(PopUpEventArgs args)
    {
        PopUpUI popup = Instantiate(popUpPrefab, container);
        popup.Set(args.headline, args.showX, args.text, args.actions,args.verticalButtonLayout,args.popUpID);
    }

    public void OpenYesNoPopUp(YesNoPopUpEventArgs args)
    {
        PopUpUI popup = Instantiate(popUpPrefab, container);

        List<LabelAndAction> actions = new List<LabelAndAction>();
        actions.Add(new LabelAndAction("yes", args.yesAction));
        actions.Add(new LabelAndAction("no", args.noAction));

        popup.Set(args.headline, args.showX, args.text, actions,false,args.popUpID);
    }

    public void OpenImagePopUp(ImagePopUpEventArgs args)
    {
        PopUpUI popup = Instantiate(popUpPrefab, container);
        popup.Set(args.headline, args.showX, args.text1, args.sprite, args.text2, args.actions, args.verticalButtonLayout, args.imageSizeMultiplier,args.tintImage, args.popUpID);
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

            PopUpEventArgs args;
            switch (variant)
            {
                case 0: default: args = new PopUpEventArgs("header",text,actions,verticalButtons,showX, ID);break;
                case 1: args = new PopUpEventArgs(header,text,ID); break;
            }

            ClientEvents.instance.onPopUp.Invoke(args);

        }

        if (testYesNoPopup)
        {
            testYesNoPopup = false;
            YesNoPopUpEventArgs args = new YesNoPopUpEventArgs("header", text, testYN, showX,ID);
            ClientEvents.instance.onYesNoPopUp.Invoke(args);
        }

        if (testImagePopup)
        {
            testImagePopup = false;
            List<LabelAndAction> actions = new List<LabelAndAction>();
            if (btn1) actions.Add(new LabelAndAction("btn1", test1));
            if (btn2) actions.Add(new LabelAndAction("button2", test2));
            if (btn3) actions.Add(new LabelAndAction("Auslöser3", test3));

            testYesNoPopup = false;
            ImagePopUpEventArgs args = new ImagePopUpEventArgs("header", text, sprite, text2,actions,verticalButtons,tintImage,imageSizeMultiplier,showX,ID);
            ClientEvents.instance.onImagePopUp.Invoke(args);
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
