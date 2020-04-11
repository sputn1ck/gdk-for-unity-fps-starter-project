using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class GraphicSettingsMenuUI : MonoBehaviour
{
    public Button resoloutionButton;
    public List<Vector2Int> resoloutions;

    private void Start()
    {
        resoloutionButton.onClick.AddListener(OnResoloutionButtonClick);
    }

    void OnResoloutionButtonClick()
    {
        List<LabelAndAction> resos = new List<LabelAndAction>();
        foreach(Vector2Int res in resoloutions)
        {
            resos.Add(resoloutionLabelAndAction(res));
        }
        PopUpEventArgs args = new PopUpEventArgs("Resoloution", "", resos, true);
        ClientEvents.instance.onPopUp.Invoke(args);
    }

    LabelAndAction resoloutionLabelAndAction(Vector2Int res)
    {
        string label = res.x + " x " + res.y;
        UnityAction action = delegate { setResoloution(res); };
        return new LabelAndAction(label, action);
    }

    void setResoloution(Vector2Int res)
    {
        resoloutionButton.GetComponentInChildren<TextMeshProUGUI>().text = res.x + " x " + res.y;
    }

}
