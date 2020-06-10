using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdBillboard : MonoBehaviour
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



}
