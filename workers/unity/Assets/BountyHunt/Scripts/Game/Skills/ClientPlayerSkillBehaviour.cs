using Fps.Config;
using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;

[WorkerType(WorkerUtils.UnityClient)]
public class ClientPlayerSkillBehaviour : MonoBehaviour
{
    [Require] private PlayerSkillComponentCommandSender PlayerSkillComponentCommandSender;
    private LinkedEntityComponent LinkedEntityComponent;
    public bool castTeleport;
    private void OnEnable()
    {
        LinkedEntityComponent = GetComponent<LinkedEntityComponent>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    //TODO add input for teleporting
    public void CastSkill(int id)
    {
        PlayerSkillComponentCommandSender.SendActivateSkillCommand(LinkedEntityComponent.EntityId, new ActivateSkillRequest(id), ActivateSkillCallback);
    }


    void ActivateSkillCallback(PlayerSkillComponent.ActivateSkill.ReceivedResponse response)
    {
        // TODO show cooldown in UI;
        if(response.StatusCode == Improbable.Worker.CInterop.StatusCode.Success)
        {
            var skill = SkillDictionary.Get(response.ResponsePayload.Value.Id);
            if (skill != null)
            {
                skill.ClientCastSkill(this);
                skill.CooldownStart();

            }
        }
        else
        {
            // TODO blink ui button in red or something
            if(response.Message == "skill on cooldown")
            {
                var skill = SkillDictionary.Get(response.RequestPayload.Id);
                if(skill != null)
                {
                    skill.onCastFailed.Invoke();
                }
            }
        }
    }
}
