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
    public bool hideWhenReady = false;

    float totalDuration;
    float endTime;
    bool activeCooldown = true;
    float readyOpacity = 1f;
    float cooldownOpacity = 0.25f;

    private PlayerSkill skill;

    private void Start()
    {
        DeactivateCooldown();
    }

    public void StartCooldown(float time)
    {
        totalDuration = time;
        endTime = Time.time + time;
        ActivateCooldown();
    }

    private void Update()
    {
        if (!activeCooldown) return;

        float remaining = Mathf.Clamp((endTime - Time.time) / totalDuration, 0, 1);

        if (reverse) CoverImage.fillAmount  = 1 - remaining;
        else CoverImage.fillAmount = remaining;

        if (remaining == 0)
        {
            DeactivateCooldown();
        } 

    }

    void ActivateCooldown()
    {
        activeCooldown = true;
        Icon.gameObject.SetActive(true);
        Icon.GetComponent<CanvasGroup>().alpha = cooldownOpacity;
        //KeyText.gameObject.SetActive(false);
    }

    void DeactivateCooldown()
    {
        activeCooldown = false;
        if(hideWhenReady) Icon.gameObject.SetActive(true);
        Icon.GetComponent<CanvasGroup>().alpha = readyOpacity;

        //KeyText.gameObject.SetActive(true);
    }

    public void setSkill(PlayerSkill skill)
    {
        this.skill = skill;
        KeyText.text = skill.key.ToString().ToUpper();
        Icon.sprite = skill.icon;

        skill.onCooldownStart.AddListener(StartCooldown);
        CoverImage.fillClockwise = clockwise;
        CoverImage.fillAmount = 1;
    }

    private void OnEnable()
    {
        setSkill(skill);
    }

}
