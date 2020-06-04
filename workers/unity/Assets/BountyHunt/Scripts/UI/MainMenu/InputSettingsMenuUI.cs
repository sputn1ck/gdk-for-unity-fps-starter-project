using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InputSettingsMenuUI : MonoBehaviour
{
    public Button resetButton;
    public Transform skillInputSettingsContainer;
    public KeySettingButtonUI keyButtonPrefab;
    KeySettingButtonUI[] keyButtons;
    public List<KeyCode> keyBlacklist;

    // Start is called before the first frame update
    void Start()
    {
        resetButton.onClick.AddListener(OnResetClick);
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
}
