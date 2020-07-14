using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.Core;

public class AnimationSyncClientBehaviour : MonoBehaviour
{

    private Animator animator;
    [Require] AnimatorSyncCommandSender AnimatorSyncCommandSender;
    [Require] EntityId EntityId;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        AnimatorSyncCommandSender.SendRequestAnimatorCommand(EntityId, new Bountyhunt.Empty(), OnAnimation);
    }

    private void OnAnimation(AnimatorSync.RequestAnimator.ReceivedResponse res)
    {
        if(res.StatusCode != Improbable.Worker.CInterop.StatusCode.Success)
        {
            Debug.LogError(res);
            return;
        }
        SyncClip(res.ResponsePayload.Value.ClipName, res.ResponsePayload.Value.Time);
    }
    private void SyncClip(string animation, float time)
    {
       animator.Play(animation, 0, time);
    }
}
