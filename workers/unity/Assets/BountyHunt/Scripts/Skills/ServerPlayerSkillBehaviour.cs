using Bountyhunt;
using Fps;
using Fps.Config;
using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[WorkerType(WorkerUtils.UnityGameLogic)]
public class ServerPlayerSkillBehaviour : MonoBehaviour
{

    [Require] public PositionWriter spatialPosition;
    [Require] public ServerMovementWriter ServerMovementWriter;

    [Require] public PlayerSkillComponentCommandReceiver PlayerSkillComponentCommandReceiver;

    public LinkedEntityComponent LinkedEntityComponent;
    private Dictionary<int, bool> SkillCooldowns;


    private void OnEnable()
    {
        PlayerSkillComponentCommandReceiver.OnActivateSkillRequestReceived += OnActivateSkill;
        LinkedEntityComponent = GetComponent<LinkedEntityComponent>();
        SkillCooldowns = new Dictionary<int, bool>();
    }

    private void OnActivateSkill(PlayerSkillComponent.ActivateSkill.ReceivedRequest obj)
    {
        var skill = SkillDictionary.Get(obj.Payload.Id);
        if (skill == null)
        {
            SendError(obj, "skill not found");
            return;
        }
        if (SkillCooldowns.ContainsKey(obj.Payload.Id) && !SkillCooldowns[obj.Payload.Id])
        {
            SendError(obj, "skill on cooldown");
            return;
        }
        skill.CastSkill(this);
        StartCoroutine(HandleCooldown(obj.Payload.Id, skill.Cooldown));
        PlayerSkillComponentCommandReceiver.SendActivateSkillResponse(new PlayerSkillComponent.ActivateSkill.Response(obj.RequestId, new Bountyhunt.Empty()));
    }

    private void SendError(PlayerSkillComponent.ActivateSkill.ReceivedRequest obj, string reason)
    {
        PlayerSkillComponentCommandReceiver.SendActivateSkillFailure(obj.RequestId, reason);
    }

    private IEnumerator HandleCooldown(int SkillId, float time)
    {
        if (SkillCooldowns.ContainsKey(SkillId))
        {
            SkillCooldowns[SkillId] = false;

        } else
        {
            SkillCooldowns.Add(SkillId, false);
        }
        yield return new WaitForSeconds(time);
        SkillCooldowns[SkillId] = true;
    }
}


