using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReticleUI : MonoBehaviour
{
    public Animator hitmarkerAnimator;
    [SerializeField] private Image reticle;
    private Vector3 baseSize;
    private void Start()
    {
        ClientEvents.instance.onOpponentHit.AddListener(onOpponentHit);
        baseSize = reticle.transform.localScale;
    }

    public void showReticle(bool show)
    {
        reticle.gameObject.SetActive(show);
    }

    void onOpponentHit(bool headshot)
    {
        if (headshot)
        {
            reticle.color = Color.red;
            reticle.transform.localScale = baseSize * 1.5f;
        }else
        {
            reticle.color = Color.white;
            reticle.transform.localScale = baseSize;
        }
        hitmarkerAnimator.SetTrigger("play");
    }

}
