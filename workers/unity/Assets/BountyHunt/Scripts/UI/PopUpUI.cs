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
    public Transform buttonsContainer;
    public Button buttonPrefab;
    public Button xButton;

    public void Set(string headline, bool showX, string text, List<LabelAndAction> buttonActions)
    {
        this.headline.text = headline;
        this.text.text = text;

        if (showX)
        {
            xButton.onClick.AddListener(Close);
        }
        else
        {
            xButton.gameObject.SetActive(false);
        }


        if (buttonActions.Count == 0)
        {
            buttonsContainer.gameObject.SetActive(false);
        }
        foreach (LabelAndAction la in buttonActions)
        {
            Button b = Instantiate(buttonPrefab, buttonsContainer);
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
