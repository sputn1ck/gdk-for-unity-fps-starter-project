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

    // Update is called once per frame
    void Update()
    {
        if (castTeleport)
        {
            castTeleport = false;
            CastTeleport();
        }
    }
    //TODO add input for teleporting
    void CastTeleport()
    {
        PlayerSkillComponentCommandSender.SendActivateSkillCommand(LinkedEntityComponent.EntityId, new ActivateSkillRequest(0), ActivateSkillCallback);
    }

    void ActivateSkillCallback(PlayerSkillComponent.ActivateSkill.ReceivedResponse response)
    {
        // TODO show cooldown in UI;
        if(response.StatusCode == Improbable.Worker.CInterop.StatusCode.Success)
        {

        } else
        {
            // TODO blink ui button in red or something
            if(response.Message == "skill on cooldown")
            {

            }
        }
    }
}
