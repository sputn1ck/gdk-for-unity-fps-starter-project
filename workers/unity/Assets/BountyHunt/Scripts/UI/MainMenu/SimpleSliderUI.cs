using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SimpleSliderUI : MonoBehaviour
{
    [SerializeField] private GameObject slider;
    [SerializeField] private float slidingTime;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private int firstButtonID = 0;
    [SerializeField] private LayoutGroup TabsLayoutGroup;
    public List<Button> buttons;
    public Dictionary<Button, SlideButtonEvents> buttonEvents = new Dictionary<Button, SlideButtonEvents>();
    

    float sliderAnimationEndTime;
    bool animateSlider;
    TransformationInfo lastSliderTransformation;
    TransformationInfo targetSliderTransformation;
    Button currentSelected;

    private void Awake()
    {

        int i = 0;
        foreach(Button b in buttons)
        {
            buttonEvents[b] = new SlideButtonEvents();
            b.onClick.AddListener(() => { SelectButton(b); }) ;
            i++;
        }

        StartCoroutine(InitializeSliderAndSelectFirstSubMenu());
    }

    IEnumerator InitializeSliderAndSelectFirstSubMenu()
    {
        if(TabsLayoutGroup) LayoutRebuilder.ForceRebuildLayoutImmediate(TabsLayoutGroup.transform as RectTransform);
        Button b = buttons[firstButtonID];
        (slider.transform as RectTransform).sizeDelta = Vector2.zero;
        yield return new WaitForEndOfFrame();
        (slider.transform as RectTransform).anchoredPosition = b.GetComponent<RectTransform>().anchoredPosition;
        (slider.transform as RectTransform).sizeDelta = b.GetComponent<RectTransform>().sizeDelta * Vector2.up;
        SelectButton(buttons[firstButtonID]);
    }

    public void SelectButton(Button btn)
    {
        buttonEvents[btn].onClick.Invoke();
        if (currentSelected == btn) return;
        if (currentSelected != null) buttonEvents[currentSelected].onDeactivate.Invoke();
        currentSelected = btn;
        buttonEvents[currentSelected].onActivate.Invoke();

        sliderAnimationEndTime = Time.time + slidingTime;
        lastSliderTransformation = slider.transform as RectTransform;
        targetSliderTransformation = btn.transform as RectTransform;
        animateSlider = true;
    }


    private void Update()
    {
        //slider animation
        if(animateSlider)
        {
            float t = 1 - (sliderAnimationEndTime - Time.time)/slidingTime;
            if (t > 1)
            {
                t = 1;
                animateSlider = false;
            }

            float blend = curve.Evaluate(t);
            TransformationInfo ti = TransformationInfo.LerpUnclamped(lastSliderTransformation, targetSliderTransformation,blend);
            (slider.transform as RectTransform).ApplyTransformation(ti);
            
        }
    }

    public SlideButtonEvents GetSlideButtonEvents(int btnID)
    {
        return buttonEvents[buttons[btnID]];
    }

}
public class TransformationInfo
{
    public Vector2 anchorMin;
    public Vector2 anchorMax;
    public Vector2 anchoredPosition;
    public Vector2 sizeDelta;
    public Vector2 pivot;

    public static implicit operator TransformationInfo(RectTransform rectTrans)
    {
        return new TransformationInfo
        {
            anchorMin = rectTrans.anchorMin,
            anchorMax = rectTrans.anchorMax,
            anchoredPosition = rectTrans.anchoredPosition,
            sizeDelta = rectTrans.sizeDelta,
            pivot = rectTrans.pivot

        };
    }

    public static TransformationInfo LerpUnclamped(TransformationInfo a, TransformationInfo b, float t)
    {
        return new TransformationInfo
        {
            anchorMin = Vector2.LerpUnclamped(a.anchorMin,b.anchorMin,t),
            anchorMax = Vector2.LerpUnclamped(a.anchorMax, b.anchorMax, t),
            anchoredPosition = Vector2.LerpUnclamped(a.anchoredPosition, b.anchoredPosition, t),
            sizeDelta = Vector2.LerpUnclamped(a.sizeDelta, b.sizeDelta, t),
            pivot = Vector2.LerpUnclamped(a.pivot, b.pivot, t),
        };
    }
}

public static class TransformationUtility
{
    public static void ApplyTransformation(this RectTransform rt, TransformationInfo ti)
    {
        rt.anchoredPosition = ti.anchoredPosition;
        rt.sizeDelta = ti.sizeDelta;
        rt.anchorMin = ti.anchorMin;
        rt.anchorMax = ti.anchorMax;
        rt.pivot = ti.pivot;
    }
}

public class SlideButtonEvents
{
    public UnityEvent onClick = new UnityEvent();
    public UnityEvent onActivate = new UnityEvent();
    public UnityEvent onDeactivate = new UnityEvent();
}
