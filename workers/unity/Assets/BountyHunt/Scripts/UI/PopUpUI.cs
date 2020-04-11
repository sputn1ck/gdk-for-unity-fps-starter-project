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
    public Transform horizontalButtonsContainer;
    public Transform verticalButtonsContainer;
    public Button buttonPrefab;
    public Button xButton;

    public void Set(string headline, bool showX, string text, List<LabelAndAction> buttonActions, bool verticalLayoutedButtons)
    {
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
        }
        
    }

    public void Close()
    {
        Destroy(gameObject);
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
