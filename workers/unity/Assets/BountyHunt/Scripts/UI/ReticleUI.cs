using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReticleUI : MonoBehaviour
{
    public Animator hitmarkerAnimator;
    [SerializeField] private Image reticle;
    private Vector3 baseSize;
    private TintColor baseTint;
    private UITinter uiTinter;
    private void Start()
    {
        ClientEvents.instance.onOpponentHit.AddListener(onOpponentHit);
        baseSize = reticle.transform.localScale;
        uiTinter = GetComponent<UITinter>();
        baseTint = uiTinter.tint;
    }

    public void showReticle(bool show)
    {
        reticle.gameObject.SetActive(show);
    }

    void onOpponentHit(bool headshot)
    {
        if (headshot)
        {
            uiTinter.updateColor(TintColor.Error);
            reticle.transform.localScale = baseSize * 1.5f;
        }else
        {
            uiTinter.updateColor(baseTint);
            reticle.transform.localScale = baseSize;
        }
        hitmarkerAnimator.SetTrigger("play");
    }

}
