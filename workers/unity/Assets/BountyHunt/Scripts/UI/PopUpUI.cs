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

    [HideInInspector] public List<Button> buttons;

    static List<PopUpUI> allPopUps = new List<PopUpUI>();

    public void Set(string headline, bool showX, string text, List<LabelAndAction> buttonActions, bool verticalLayoutedButtons)
    {
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

        foreach (LabelAndAction la in buttonActions)
        {
            Button b = Instantiate(buttonPrefab, btnContainer);
            b.GetComponentInChildren<TextMeshProUGUI>().text = la.label;
            b.onClick.AddListener(la.action);
            b.onClick.AddListener(Close);
            this.buttons.Add(b);
        }
        
    }

    public void Set(string headline, bool showX, string text1,Sprite sprite, string text2, List<LabelAndAction> buttonActions, bool verticalLayoutedButtons, float imageSizeMultiplier, bool tintImage)
    {
        Set(headline, showX, text1, buttonActions, verticalLayoutedButtons);

        image.gameObject.SetActive(true);
        image.sprite = sprite;
        image.GetComponent<LayoutElement>().preferredHeight *= imageSizeMultiplier;

        if (!tintImage)
        {
            image.GetComponent<UITinter>().enabled = false;
            image.color = Color.white;
        }
        

        if (text2 != null)
        {
            TextMeshProUGUI textMesh2 = Instantiate(this.text,this.text.transform.parent);
            textMesh2.transform.SetSiblingIndex(2);
            textMesh2.gameObject.SetActive(true);
            textMesh2.text = text2;
        }
    }


    public void Close()
    {
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

public class LabelAndAction
{
    public string label;
    public UnityAction action;

    public LabelAndAction(string label, UnityAction action)
    {
        this.label = label;
        this.action = action;
    }
}
