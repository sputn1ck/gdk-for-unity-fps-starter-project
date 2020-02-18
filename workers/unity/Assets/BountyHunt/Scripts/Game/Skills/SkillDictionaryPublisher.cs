using Fps.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDictionaryPublisher : MonoBehaviour, ISettingsPublisher
{

    [SerializeField] private SkillDictionary skillDictionary;


    public void Publish()
    {
        SkillDictionary.Instance = skillDictionary;
    }
}
