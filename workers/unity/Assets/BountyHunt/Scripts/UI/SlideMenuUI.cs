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
    Rect lastSliderRect;
    Rect targetSliderRect;
    SlideSubMenuUI currentSelected;

    private void Awake()
    {
        
    }
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
        StartCoroutine(InitializeSliderAndSelectFirstSubMenu());
    }

    IEnumerator InitializeSliderAndSelectFirstSubMenu()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(TabsLayoutGroup.transform as RectTransform);
        Button b = buttonSubMenuPairs[firstButtonID].button;
        (slider.transform as RectTransform).sizeDelta = Vector2.zero;
        yield return new WaitForEndOfFrame();
        (slider.transform as RectTransform).anchoredPosition = b.GetComponent<RectTransform>().anchoredPosition;
        (slider.transform as RectTransform).sizeDelta = b.GetComponent<RectTransform>().sizeDelta * Vector2.up;

        SelectSubMenu(buttonSubMenuPairs[firstButtonID].subMenu);
    }

    public void SelectSubMenu(SlideSubMenuUI subMenu)
    {
        if (currentSelected) currentSelected.Deactivate();
        currentSelected = subMenu;
        currentSelected.Activate();


        //slider animation
        Button button = buttonSubMenuPairs.Find(e => e.subMenu == subMenu).button;
        sliderAnimationEndTime = Time.time + slidingTime;
        lastSliderRect = new Rect((slider.transform as RectTransform).anchoredPosition, (slider.transform as RectTransform).sizeDelta);
        targetSliderRect = new Rect((button.transform as RectTransform).anchoredPosition, (button.transform as RectTransform).sizeDelta);
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
            Vector2 pos = Vector2.LerpUnclamped(lastSliderRect.position,targetSliderRect.position,  blend);
            Vector2 size = Vector2.LerpUnclamped(lastSliderRect.size, targetSliderRect.size,  blend);

            slider.GetComponent<RectTransform>().anchoredPosition = pos;
            slider.GetComponent<RectTransform>().sizeDelta = size;
        }
    }
}
[System.Serializable]
public class ButtonSubMenuPair
{
    public Button button;
    public SlideSubMenuUI subMenu;
}
