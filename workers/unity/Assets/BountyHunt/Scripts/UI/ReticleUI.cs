using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleUI : MonoBehaviour
{
    public Animator animator;

    private void Start()
    {
        ClientEvents.instance.onOpponentHit.AddListener(onOpponentHit);
    }

    void onOpponentHit()
    {
        animator.SetTrigger("play");
    }

}
