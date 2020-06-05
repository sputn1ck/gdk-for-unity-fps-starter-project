using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fps.PlayerControls;

public class InputSettingsMenuUI : MonoBehaviour
{
    public Button resetButton;
    public Transform skillInputSettingsContainer;
    public KeySettingButtonUI keyButtonPrefab;
    KeySettingButtonUI[] keyButtons;
    public List<KeyCode> keyBlacklist;

    public AnimationCurve mouseSensitivitySliderCurve;
    public float maxMouseSensitivity;
    public Slider mouseSensitivitySlider;
    public TextMeshProUGUI mouseSensitivityText;

    public Toggle invertMouseXToggle;
    public Toggle invertMouseYToggle;

    // Start is called before the first frame update
    void Start()
    {
        mouseSensitivitySlider.onValueChanged.AddListener(UpdateMouseSensitivity);
        resetButton.onClick.AddListener(OnResetClick);
        invertMouseXToggle.onValueChanged.AddListener(InvertX);
        invertMouseYToggle.onValueChanged.AddListener(InvertY);
        foreach (PlayerSkill skill in SkillDictionary.Instance.skills)
        {
            var btn = Instantiate(keyButtonPrefab,skillInputSettingsContainer);
            btn.label = skill.SkillName;
            btn.key = skill.SkillName + "_Key";
            btn.RefreshTexts();
        }
        keyButtons = GetComponentsInChildren<KeySettingButtonUI>();

        foreach(var btn in keyButtons)
        {
            btn.keyBlacklist = keyBlacklist;
        }
    }


    private void OnEnable()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(skillInputSettingsContainer as RectTransform);
        SetMouseSensitivitySliderValue(PlayerPrefs.GetFloat("MouseSensitivity", 1));
        invertMouseXToggle.isOn = PlayerPrefs.GetFloat("MouseMultiplierX", 1) < 0 ? true : false;
        invertMouseYToggle.isOn = PlayerPrefs.GetFloat("MouseMultiplierY", 1) < 0 ? true : false;
    }

    void OnResetClick()
    {
        InputKeyMapping.ResetAllKeys();
        foreach(var kb in keyButtons)
        {
            kb.RefreshTexts();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMouseSensitivitySliderValue(float sens)
    {
        mouseSensitivityText.text = sens.ToString("0.000");
        float value = Mathf.Log(sens + 0.1f, 1.58805f) + 5;
        mouseSensitivitySlider.SetValueWithoutNotify(value);

    }

    public void UpdateMouseSensitivity(float sliderValue)
    {

        float sens = Mathf.Pow(1.58805f, (sliderValue - 5f)) - 0.1f;
        sens = Mathf.Max(0, sens);
        mouseSensitivityText.text = sens.ToString("0.000");

        PlayerPrefs.SetFloat("MouseSensitivity", sens);
        PlayerPrefs.Save();
        if (!KeyboardControls.instance) return;
        KeyboardControls.instance.sensitivity = sens;
    }

    void InvertX(bool invert)
    {
        float value = invert ? -1 : 1;

        PlayerPrefs.SetFloat("MouseMultiplierX", value);

        if (!KeyboardControls.instance) return;
        KeyboardControls.instance.mouseMultiplierX = value;
    }
    void InvertY(bool invert)
    {
        float value = invert ? -1 : 1;
        PlayerPrefs.SetFloat("MouseMultiplierY", value);

        if (!KeyboardControls.instance) return;
        KeyboardControls.instance.mouseMultiplierY = value;
    }

}
