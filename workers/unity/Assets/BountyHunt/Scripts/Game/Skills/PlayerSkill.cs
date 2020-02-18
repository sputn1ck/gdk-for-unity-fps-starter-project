using Fps;
using Fps.Respawning;
using Improbable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Fps.SchemaExtensions;
using UnityEngine.Events;

public abstract class PlayerSkill : ScriptableObject
{
    public float Cooldown;
    public Sprite icon;
    public string key;

    public FloatEvent onCooldownStart = new FloatEvent();
    public UnityEvent onCastFailed = new UnityEvent();

    public void CooldownStart()
    {
        onCooldownStart.Invoke(Cooldown);
    }

    public abstract void ServerCastSkill(ServerPlayerSkillBehaviour player);
    public abstract void ClientCastSkill(ClientPlayerSkillBehaviour player);

    public abstract void NonAuthorativeCastSkill(NonAuthorativePlayerSkillBehaviour player);
}

public abstract class PayloadSkill : PlayerSkill
{
    public abstract string GetPayloadString();
    
}
public abstract class TimedSkill : PlayerSkill
{
    public float castTime;
    public override void ServerCastSkill(ServerPlayerSkillBehaviour player)
    {
        player.StartCoroutine(CastTime(player));
    }
    private IEnumerator CastTime(ServerPlayerSkillBehaviour player)
    {
        yield return new WaitForSeconds(castTime);
        RealCast(player);
    }
    public abstract void RealCast(ServerPlayerSkillBehaviour player);
}

