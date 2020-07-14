using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.Core;

public class AnimationSyncServerBehaviour : MonoBehaviour
{
    private Animator animator;
    [Require] AnimatorSyncCommandReceiver AnimatorSyncCommandReceiver;
    [Require] EntityId EntityId;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    private void OnEnable()
    {
        AnimatorSyncCommandReceiver.OnRequestAnimatorRequestReceived += OnRequestAnimator;
    }

    private void OnRequestAnimator(AnimatorSync.RequestAnimator.ReceivedRequest obj)
    {
        var animationName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        var time = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        AnimatorSyncCommandReceiver.SendRequestAnimatorResponse(obj.RequestId, new AnimatorData(animationName, time));
    }
}
