using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdBillboard : MonoBehaviour
{
    public MeshRenderer AdRenderer;
    public Advertiser.AdMaterialType AdType;
    private Advertiser advertiser;


    public void SetAdvertiser(Advertiser advertiser)
    {
        this.advertiser = advertiser;
        Material mat = this.advertiser.GetRandomMaterial(AdType);
        if (mat == null) return;

        AdRenderer.material = mat;
    }



}
