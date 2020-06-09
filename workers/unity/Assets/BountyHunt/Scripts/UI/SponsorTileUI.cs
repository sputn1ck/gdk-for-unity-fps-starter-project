using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SponsorTileUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI satsText;
    public Button openLinkButton;
    public Button openLaterButton;
    public TextMeshProUGUI savedForLaterText;

    Animator animator;
    Advertiser advertiser;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Set(AdvertiserInvestment advertiser)
    {
        advertiser = advertiser;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetBool("showInfo", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetBool("showInfo", false);

    }
}
