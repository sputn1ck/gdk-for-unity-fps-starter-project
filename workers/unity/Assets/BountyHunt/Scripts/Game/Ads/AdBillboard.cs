using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AdBillboard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public MeshRenderer AdRenderer;
    public Advertiser.AdMaterialType AdType;
    private AdvertiserInvestment advertiserInvestment;


    public void SetAdvertiser(AdvertiserInvestment adInv)
    {
        this.advertiserInvestment = adInv;
        Material mat = this.advertiserInvestment.advertiser.GetRandomMaterial(AdType);
        if (mat == null) return;
        
        AdRenderer.material = mat;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        List<(UnityAction, string)> actions = new List<(UnityAction, string)>();
        string text = Utility.SatsToShortString(advertiserInvestment.investment, true, UITinter.tintDict[TintColor.Sats]);
        ContextMenuUI.Instance.Set(this, advertiserInvestment.advertiser.name, text, actions);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ContextMenuUI.Instance.Hide(this);
    }
}
