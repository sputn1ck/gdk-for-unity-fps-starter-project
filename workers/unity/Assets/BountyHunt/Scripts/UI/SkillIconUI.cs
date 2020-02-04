using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillIconUI : MonoBehaviour
{
    public Image Icon;
    public Image CoverImage;
    public TextMeshProUGUI KeyText;

    public bool clockwise = true;
    public bool reverse = false;
    public bool hideWhenInactive = false;

    float totalTime;
    float endTime;
    bool active = true;

    private PlayerSkill skill;

    private void Start()
    {
        Deactivate();
    }

    public void StartCooldown(float time)
    {
        totalTime = time;
        endTime = Time.time + time;
        Activate();
    }

    private void Update()
    {
        if (!active) return;

        float remaining = Mathf.Clamp((endTime - Time.time) / totalTime, 0, 1);

        if (reverse) remaining = 1 - remaining;

        CoverImage.fillClockwise = clockwise;

        CoverImage.fillAmount = remaining;

        if (remaining == 0)
        {
            Deactivate();
        } 

    }

    void Activate()
    {
        active = true;
        Icon.gameObject.SetActive(true);
        KeyText.gameObject.SetActive(false);
    }

    void Deactivate()
    {
        active = false;
        if(hideWhenInactive) Icon.gameObject.SetActive(true);
        KeyText.gameObject.SetActive(true);
    }

    public void setSkill(PlayerSkill skill)
    {
        this.skill = skill;
        KeyText.text = skill.key.ToUpper();
        Icon.sprite = skill.icon;

        skill.onCooldownStart.AddListener(StartCooldown);
        
    }

}
