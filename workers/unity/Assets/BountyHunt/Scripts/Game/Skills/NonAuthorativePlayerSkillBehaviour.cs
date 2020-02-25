using Fps.Config;
using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;

[WorkerType(WorkerUtils.UnityClient)]
public class NonAuthorativePlayerSkillBehaviour : MonoBehaviour
{
    [Require] PlayerSkillComponentReader PlayerSkillComponentReader;

    private void OnEnable()
    {
        PlayerSkillComponentReader.OnActivatedSkillEventEvent += OnActivatedSkill;
    }

    private void OnActivatedSkill(ActivatedSkillEvent obj)
    {
        var skill = SkillDictionary.Get(obj.Id);
        if (skill != null)
        {
            skill.NonAuthorativeCastSkill(this);
        }
    }

}
