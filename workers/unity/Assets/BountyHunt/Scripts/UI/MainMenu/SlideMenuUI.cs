using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideMenuUI : MonoBehaviour
{
    public GameObject slider;
    public float slidingTime;
    public AnimationCurve curve;
    public int firstButtonID = 0;
    public LayoutGroup TabsLayoutGroup;
    public List<ButtonSubMenuPair> buttonSubMenuPairs;


    float sliderAnimationEndTime;
    bool animateSlider;
    TransformationInfo lastSliderTransformation;
    TransformationInfo targetSliderTransformation;
    SlideSubMenuUI currentSelected;

    private void Start()
    {
        foreach (ButtonSubMenuPair bsmp in buttonSubMenuPairs)
        {
            bsmp.subMenu.gameObject.SetActive(true);
            CanvasGroup gc = bsmp.subMenu.canvasGroup;
            gc.alpha = 0;
            gc.interactable = false;
            gc.blocksRaycasts = false;
            bsmp.subMenu.menu = this;
            bsmp.button.onClick.AddListener(bsmp.subMenu.Select);
        }

        slider.transform.parent = buttonSubMenuPairs[firstButtonID].button.transform;
        (slider.transform as RectTransform).ApplyTransformation(TransformationInfo.FitParent);
        SelectSubMenu(buttonSubMenuPairs[firstButtonID].subMenu);
        LayoutRebuilder.ForceRebuildLayoutImmediate(TabsLayoutGroup.transform as RectTransform);
    }


    public void SelectSubMenu(SlideSubMenuUI subMenu)
    {
        if (currentSelected) currentSelected.Deactivate();
        currentSelected = subMenu;
        currentSelected.Activate();


        //slider animation
        Button button = buttonSubMenuPairs.Find(e => e.subMenu == subMenu).button;

        slider.transform.parent = button.transform;
        sliderAnimationEndTime = Time.time + slidingTime;
        lastSliderTransformation = slider.transform as RectTransform;
        targetSliderTransformation = TransformationInfo.FitParent;
        animateSlider = true;
    }

    private void Update()
    {
        //slider animation
        if (animateSlider)
        {
            float t = 1 - (sliderAnimationEndTime - Time.time) / slidingTime;
            if (t > 1)
            {
                t = 1;
                animateSlider = false;
            }

            float blend = curve.Evaluate(t);
            TransformationInfo ti = TransformationInfo.LerpUnclamped(lastSliderTransformation, targetSliderTransformation, blend);
            (slider.transform as RectTransform).ApplyTransformation(ti);
        }
    }
}
[System.Serializable]
public class ButtonSubMenuPair
{
    public Button button;
    public SlideSubMenuUI subMenu;
}
