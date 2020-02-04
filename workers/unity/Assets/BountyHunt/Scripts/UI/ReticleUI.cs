using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReticleUI : MonoBehaviour
{
    public Animator hitmarkerAnimator;
    [SerializeField] private Image reticle;

    private void Start()
    {
        ClientEvents.instance.onOpponentHit.AddListener(onOpponentHit);
    }

    public void showReticle(bool show)
    {
        reticle.gameObject.SetActive(show);
    }

    void onOpponentHit()
    {
        hitmarkerAnimator.SetTrigger("play");
    }

}
