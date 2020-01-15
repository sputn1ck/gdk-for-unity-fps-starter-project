using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bounty PickUp Material Settings", menuName = "Donner/BountyPickup_MaterialSettings")]
public class BountyPickUpMaterialSettings : ScriptableObject
{

    public Material defaultMat;
    public Material holoMat;
    [SerializeField] public List<satColorValue> satColors;
    private Dictionary<Color, Material> mats = new Dictionary<Color, Material>();

    private Dictionary<Color, Material> holoMats = new Dictionary<Color, Material>();

    public BountyAppearence getMaterialByValue(long sats)
    {
        satColorValue scv = new satColorValue { minSatValue = 0, color = Color.white };

        foreach (satColorValue sc in satColors)
        {
            if(sats >= sc.minSatValue)
            {
                if (sc.minSatValue > scv.minSatValue) scv = sc;
            }
        }

        return new BountyAppearence(getMaterialByColor(scv.color),scv.scale);

    }
    public Material getHoloMaterialByValue(long sats)
    {
        satColorValue scv = new satColorValue { minSatValue = 0, color = Color.white };

        return new Material(getHoloMaterialByColor(scv.color));

    }

    Material getHoloMaterialByColor(Color color)
    {
        if (!holoMats.ContainsKey(color))
        {
            Material mat = Instantiate(holoMat);
            mat.SetColor("MainColor", color);
            holoMats[color] = mat;
        }

        return holoMats[color];

    }
    Material getMaterialByColor(Color color)
    {
        if (!mats.ContainsKey(color))
        {
            Material mat = Instantiate(defaultMat);
            mat.SetColor("_EmissionColor", color);
            mats[color] = mat;
        }

        return mats[color];

    }


    public Color getColorByValue(long sats)
    {
        satColorValue scv = new satColorValue { minSatValue = 0, color = Color.white };

        foreach (satColorValue sc in satColors)
        {
            if (sats >= sc.minSatValue)
            {
                if (sc.minSatValue > scv.minSatValue) scv = sc;
            }
        }

        return scv.color;
    }

    void CreateColorstepsTexture()
    {
        Texture2D tex = new Texture2D(10,10*satColors.Count,TextureFormat.RGB24,false);
        tex.SetPixels(new Color[] { Color.black });

        for (int i = 0; i<satColors.Count;i++)
        {
            tex.SetPixels(i * 10 + 1, 0, 10, 9, new Color[] { satColors[i].color });
        }

    }

}

[System.Serializable]
public struct satColorValue
{
    public float minSatValue;
    public Color color;
    public float scale;
}

public class BountyAppearence
{
    public Material mat;
    public float scale;
    public BountyAppearence(Material m, float s)
    {
        mat = m;
        scale = s;
    }

}
