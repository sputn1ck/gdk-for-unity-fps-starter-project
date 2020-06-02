using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeySettingButtonUI : MonoBehaviour
{
    public string label;
    public string key;
    public Button button;
    public TextMeshProUGUI labelText;
    public TextMeshProUGUI keyText;

    float listeningCounter = 0f;
    const float detectKeyCountDown = 5f;
    PopUpUI popUp;

    private void Awake()
    {
        labelText.text = label;
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnEnable()
    {
        keyText.text = InputKeyMapping.GetKeyCode(key).ToString();
    }

    void OnButtonClick()
    {
        listeningCounter = detectKeyCountDown;
        string header = string.Format(GameText.AssignButtonPopupHeader, label);
        PopUpArgs popUpArgs = new PopUpArgs(header, GameText.AssignButtonPopup,StopListening);

    }

    public void StopListening()
    {
        listeningCounter = 0;
    }

    private void Update()
    {
        if (listeningCounter==0) return;
        listeningCounter -= Mathf.Max(listeningCounter - Time.deltaTime);

        foreach (KeyCode keycode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keycode))
            {
                InputKeyMapping.SetKey(key, keycode);
                keyText.text = keycode.ToString();
                StopListening();
                popUp.Close();
                return;
            }
        }

        if (listeningCounter == 0)
        {
            StopListening();
            popUp.Close();
        }
    }

}
