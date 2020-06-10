using Bountyhunt;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public class Advertiser
{
    public string name;
    public string description;
    public string url;
    public string hash;
    public (List<Texture2D>, List<string>) squareMedia;

    Dictionary<AdMaterialType, List<MaterialInfo>> materials;

    public async Task Load(AdvertiserInvestmentInfo source)
    {
        name = source.Name;
        url = source.Url;
        hash = source.Hash;
        squareMedia = await AdUtility.getTexturesFromURLList(source.SquareImageUrls);
        Initialize();
    }


    
    public void Initialize()
    {
        materials = new Dictionary<AdMaterialType, List<MaterialInfo>>();
        InitializeMatrials(squareMedia, AdMaterialType.SQUARE, AdUtility.instance.defaultSquareBillboardMaterial, "_EmissionMap");
        //InitializeMatrials(pickupAdTextures, AdMaterialType.PICKUP, AdManager.instance.defaultPickupAdMaterial, "_MainTex");
        //InitializeMatrials(horizontalAdTextures, AdMaterialType.HORIZONTAL, AdManager.instance.defaultHorizontalAdMaterial, "_EmissionMap");
        //InitializeMatrials(verticalAdTextures, AdMaterialType.VERTICAL, AdManager.instance.defaultVerticalAdMaterial, "_EmissionMap");

    }
    public void InitializeMatrials((List<Texture2D> textures, List<string> videolinks) media, AdMaterialType type, Material defaultMaterial, string TextureToReplace)
    {
        materials[type] = new List<MaterialInfo>();
        foreach (var texture in media.textures)
        {
            var material = ClientAdManagerBehaviour.Instantiate(defaultMaterial);
            material.SetTexture(TextureToReplace, texture);
            materials[type].Add(new MaterialInfo(material, texture));
        }

        foreach (string link in media.videolinks)
        {
            var material = ClientAdManagerBehaviour.Instantiate(defaultMaterial);
            VideoPlayer vp = AdUtility.instance.GetNewVideoPlayer();
            vp.url = link;
            vp.renderMode = VideoRenderMode.RenderTexture;
            RenderTexture rt = new RenderTexture(512, 512, 0);
            vp.targetTexture = rt;
            material.SetTexture(TextureToReplace, rt);
            materials[type].Add(new MaterialInfo(material, rt));

        }
    }

    public Material GetRandomMaterial(AdMaterialType type)
    {
        return GetRandomMaterialInfo(type).Material;
    }
    public Texture GetRandomTexture(AdMaterialType type)
    {
        return GetRandomMaterialInfo(type).Texture;
    }
    public MaterialInfo GetRandomMaterialInfo(AdMaterialType type)
    {
        List<MaterialInfo> mats = GetMaterials(type);
        if (mats.Count == 0)
        {
            Debug.LogError(name + " has no Material of type " + type);
            return new MaterialInfo();
        }

        var rnd = Random.Range(0, mats.Count);
        if (rnd > mats.Count - 1)
            return mats[0];
        return mats[rnd];
    }

    public List<MaterialInfo> GetMaterials(AdMaterialType type)
    {
        return materials[type];
    }

    public enum AdMaterialType { SQUARE, HORIZONTAL, VERTICAL }
}

public struct MaterialInfo
{
    public Material Material;
    public Texture Texture;

    public MaterialInfo(Material mat, Texture tex)
    {
        Material = mat;
        Texture = tex;
    }
}

[System.Serializable]
public class AdvertiserInvestment
{
    public Advertiser advertiser;
    public long investment;

    public AdvertiserInvestment(Advertiser advertiser, long investment)
    {
        this.advertiser = advertiser;
        this.investment = investment;
    }
}
