using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using YamlDotNet;

public class AdManager : MonoBehaviour
{
    public static AdManager instance;


    public Material defaultSquareAdMaterial;
    public Material defaultPickupAdMaterial;
    public Material defaultHorizontalAdMaterial;
    public Material defaultVerticalAdMaterial;

    [SerializeField] public List<Advertiser> advertisers;

    [HideInInspector] public long totalSponsoredSats;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        ClientEvents.instance.onMapLoaded.AddListener(Initialize);
    }

    void Initialize()
    {
        InitializeAdvertizers();
        InitializeAllAds();
    }
    void InitializeAllAds()
    {
        AdBillboard[] banners = FindObjectsOfType<AdBillboard>();
        Debug.Log("Total ad billboards Count: " + banners.Length);
        Debug.Log("Total advertisers Count: " + advertisers.Count);

        banners = banners.OrderBy(x => Random.value).ToArray<AdBillboard>();
        List<AdBillboard> bannersLeft = banners.ToList();

        for (int  i = advertisers.Count-1; i > 0; i--)
        {
            Advertiser advertiser = advertisers[i];
            int count = Mathf.Max(1, (int)(banners.Length * advertiser.satsDonation / totalSponsoredSats));

            for(int j = 0; j< count; j++)
            {
                if(bannersLeft.Count == 0)
                {
                    Debug.LogWarning("no BannersLeft!");
                    break;
                }
                bannersLeft[0].SetAdvertiser(advertiser);
                bannersLeft.RemoveAt(0);
            }
        }
        
        foreach(AdBillboard ab in bannersLeft)
        {
            ab.SetAdvertiser(advertisers[0]);
        }
        
    }

    void InitializeAdvertizers()
    {
        totalSponsoredSats = 0;
        foreach (Advertiser advertiser in advertisers)
        {
            totalSponsoredSats += advertiser.satsDonation;
            advertiser.Initialize();
        }

        advertisers = advertisers.OrderByDescending(o => o.satsDonation).ToList<Advertiser>();
    }

    public Advertiser GetRandomAdvertiser()
    {
        var winningTicket = Random.Range(0, totalSponsoredSats);
        long ticket = 0;
        foreach (var advertiser in advertisers)
        {
            ticket += advertiser.satsDonation;
            if (ticket > winningTicket)
                return advertiser;

        }
        return advertisers[0];
    }

    public async void UpdateAdvertisers(List<AdvertiserAdressAndValue> args)
    {
        
        advertisers.Clear();
        foreach(AdvertiserAdressAndValue aav in args)
        {
            Advertiser addy = new Advertiser();
            
            string yamlString = await AwaitRequestText.SendAsyncWebRequest(this, aav.url);

            if (string.IsNullOrEmpty(yamlString))
            {
                continue;
            }

            YamlDotNet.Serialization.Deserializer deserializer = new YamlDotNet.Serialization.Deserializer();
            AdvertiserDownloadHelper adh = deserializer.Deserialize<AdvertiserDownloadHelper>(yamlString);
            addy.name = adh.name;
            addy.description = adh.dectription;

            addy.squareAdTextures = await getTexturesFromURLArray(aav.url,adh.squareTextureLocations);
            addy.horizontalAdTextures = await getTexturesFromURLArray(aav.url,adh.horizontalTextureLocations);
            addy.verticalAdTextures= await getTexturesFromURLArray(aav.url,adh.verticalTextureLocations);
            addy.pickupAdTextures = await getTexturesFromURLArray(aav.url,adh.pickupTextureLocations);

            advertisers.Add(addy);
        }

        Initialize();
    }

    public static async Task<List<Texture2D>> getTexturesFromURLArray(string urlRoot, string[] urls)
    {
        List<Texture2D> textures  = new List<Texture2D>();
        if (urls == null)
        {
            Debug.LogWarning("url list is null");
            return textures;
        }
        foreach (string url in urls)
        {
            Texture2D tex = await AwaitRequestTexture.SendAsyncWebRequest(instance, Path.Combine(urlRoot,url));

            if (tex == null)
            {
                continue;
            }

            textures.Add(tex);

        }

        return textures;
    }

}

[System.Serializable]
public struct AdvertiserAdressAndValue
{
    public string url;
    public long sats;
}

public class AdvertiserDownloadHelper
{
    public string name;
    public string dectription;
    public string[] squareTextureLocations = new string[] { "", "" } ;
    public string[] pickupTextureLocations;
    public string[] horizontalTextureLocations;
    public string[] verticalTextureLocations;
}

[System.Serializable]
public class Advertiser
{
    public string name;
    public long satsDonation;
    public string description;
    public List<Texture2D> squareAdTextures;
    public List<Texture2D> pickupAdTextures;
    public List<Texture2D> horizontalAdTextures;
    public List<Texture2D> verticalAdTextures;
    //public GameObject AdPrefab;

    Dictionary<AdMaterialType, List<Material>> materials;

    public void Initialize()
    {
        materials = new Dictionary<AdMaterialType, List<Material>>();
        InitializeMatrials(squareAdTextures, AdMaterialType.SQUARE, AdManager.instance.defaultSquareAdMaterial, "_EmissionMap");
        InitializeMatrials(pickupAdTextures, AdMaterialType.PICKUP, AdManager.instance.defaultPickupAdMaterial, "_MainTex");
        InitializeMatrials(horizontalAdTextures, AdMaterialType.HORIZONTAL, AdManager.instance.defaultHorizontalAdMaterial, "_EmissionMap");
        InitializeMatrials(verticalAdTextures, AdMaterialType.VERTICAL, AdManager.instance.defaultVerticalAdMaterial, "_EmissionMap");

    }
    public void InitializeMatrials(List<Texture2D> textures, AdMaterialType type, Material defaultMaterial, string TextureToReplace)
    {
        materials[type] = new List<Material>();
        foreach (var texture in textures)
        {
            var material = AdManager.Instantiate(defaultMaterial);
            material.SetTexture(TextureToReplace, texture);
            materials[type].Add(material);
        }
    }
    public Material GetRandomMaterial(AdMaterialType type)
    {
        List<Material> mats = GetMaterials(type);
        if (mats.Count == 0)
        {
            Debug.LogError(name + " has no Material of type " + type);
            return null;
        }

        var rnd = Random.Range(0, mats.Count);
        if (rnd > mats.Count - 1)
            return mats[0];
        return mats[rnd];
    }

    public List<Material> GetMaterials (AdMaterialType type)
    {
        return materials[type];
    }
        
    public enum AdMaterialType { PICKUP, SQUARE, HORIZONTAL, VERTICAL}
}

