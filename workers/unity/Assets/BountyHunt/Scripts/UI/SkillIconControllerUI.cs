using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillIconControllerUI : MonoBehaviour
{
    public List <SkillIconUI> SkillIcons;

    private void Start()
    {
        for (int i = 0; i< SkillIcons.Count; i++)
        {
            if(SkillDictionary.Count > i)
            {
                SkillIcons[i].gameObject.SetActive(true);
                SkillIcons[i].setSkill(SkillDictionary.Get(i));
            }
            else
            {
                SkillIcons[i].gameObject.SetActive(false);
            }
        }
    }



}
