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
    private UITinter[] uiTinters;
    private void Awake()
    {
        uiTinters = GetComponentsInChildren<UITinter>();
    }
    private void Start()
    {
        ClientEvents.instance.onTargetHit.AddListener(onTargetHit);
        baseTint = uiTinters[0].tint;
    }

    public void showReticle(bool show)
    {
        if(Fps.Movement.FpsDriver.instance != null)
        {
            if (Fps.Movement.FpsDriver.instance.thirdPersonViewActivated) show = false;
            if (Fps.Movement.FpsDriver.instance.isAiming) show = false;
        }
        
        reticle.gameObject.SetActive(show);
    }

    void onTargetHit(bool headshot)
    {
        string animationTrigger = headshot? "headshot":"show";
        hitmarkerAnimator.SetTrigger(animationTrigger);
    }

    public void DefaultTint()
    {
        Tint(baseTint);
    }
    public void HeadshotTint()
    {
        Tint(TintColor.Error);
    }
    void Tint(TintColor tint)
    {
        foreach(UITinter tinter in uiTinters)
        {
            tinter.updateColor(tint);
        }
    }


}
