using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class AdManager : MonoBehaviour
{
    public static AdManager instance;


    public Material defaultSquareAdMaterial;
    public Material defaultPickupAdMaterial;
    public Material defaultHorizontalAdMaterial;
    public Material defaultVerticalAdMaterial;

    public Dictionary<string, Advertiser> advertisers;
    public List<AdvertiserInvestmentInfos> investments;

    [HideInInspector] public long totalSponsoredSats;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
        advertisers = new Dictionary<string, Advertiser>();
    }

    private void Start()
    {
        //ClientEvents.instance.onMapLoaded.AddListener(Initialize);
    }

    void Initialize()
    {
        SortInvestments();
        InitializeAllAds();
    }
    void InitializeAllAds()
    {
        AdBillboard[] banners = FindObjectsOfType<AdBillboard>();
        Debug.Log("Total ad billboards Count: " + banners.Length);
        Debug.Log("Total investments Count: " + investments.Count);

        banners = banners.OrderBy(x => Random.value).ToArray<AdBillboard>();
        List<AdBillboard> bannersLeft = banners.ToList();

        foreach (AdvertiserInvestmentInfos inv in investments)
        {
            Advertiser advertiser = advertisers[inv.key];
            int count = Mathf.Max(1, (int)(banners.Length * inv.sats / totalSponsoredSats));

            for (int j = 0; j < count; j++)
            {
                if (bannersLeft.Count == 0)
                {
                    Debug.LogWarning("no BannersLeft!");
                    break;
                }
                bannersLeft[0].SetAdvertiser(advertiser);
                bannersLeft.RemoveAt(0);
            }
        }

        foreach (AdBillboard ab in bannersLeft)
        {
            ab.SetAdvertiser(advertisers[investments[0].key]);
        }

    }

    void SortInvestments()
    {
        totalSponsoredSats = 0;
        foreach (AdvertiserInvestmentInfos inv in investments)
        {
            totalSponsoredSats += inv.sats;
        }

        investments = investments.OrderByDescending(o => o.sats).ToList<AdvertiserInvestmentInfos>();
    }

    public Advertiser GetRandomAdvertiser()
    {
        var winningTicket = Random.Range(0, totalSponsoredSats);
        long ticket = 0;
        foreach (var inv in investments)
        {
            ticket += inv.sats;
            if (ticket > winningTicket)
                return advertisers[inv.key];

        }
        return advertisers[investments[0].key];
    }

    public async void UpdateAdvertisers(List<AdvertiserInvestmentInfos> args)
    {
        investments = args;
        List<string> advKeys = new List<string>();

        foreach (AdvertiserInvestmentInfos aiv in args)
        {
            if (!advertisers.ContainsKey(aiv.key))
            {
                advKeys.Add(aiv.key);
            }
        }
        await AddAdvertisers(advKeys);

        Initialize();
    }

    public static async Task<List<Texture2D>> getTexturesFromURLArray(string[] urls)
    {
        List<Texture2D> textures = new List<Texture2D>();
        if (urls == null)
        {
            Debug.LogWarning("url list is null");
            return textures;
        }
        foreach (string url in urls)
        {
            Texture2D tex = await AwaitRequestTexture.SendAsyncWebRequest(instance, url);

            if (tex == null)
            {
                continue;
            }

            textures.Add(tex);

        }

        return textures;
    }

    public async Task AddAdvertisers(List<string> keys)
    {

        //Todo get AdvertiserClientInfos from backend
        Dictionary<string, AdvertiserClientInfos> advertiserDict = new Dictionary<string, AdvertiserClientInfos>();

        List<Task> tasks = new List<Task>();

        foreach (string key in keys)
        {
            if (!advertiserDict.ContainsKey(key))
            {
                Debug.LogWarning("advertiser key " + key + " does not exist!");
                continue;
            }

            AdvertiserClientInfos aci = advertiserDict[key];
            tasks.Add(loadAdvertiser(key, aci));
        }
        await Task.WhenAll(tasks);
    }

    private async Task loadAdvertiser(string key, AdvertiserClientInfos aci)
    {
        Advertiser advertiser = new Advertiser();
        advertiser.name = aci.name;
        advertiser.description = aci.description;
        advertiser.squareAdTextures = await getTexturesFromURLArray(aci.squareTextureLocations);
        advertiser.horizontalAdTextures = await getTexturesFromURLArray(aci.horizontalTextureLocations);
        advertiser.verticalAdTextures = await getTexturesFromURLArray(aci.verticalTextureLocations);
        advertiser.pickupAdTextures = await getTexturesFromURLArray(aci.pickupTextureLocations);

        advertiser.Initialize();
        advertisers[key] = advertiser;
    }

}



[System.Serializable]
public struct AdvertiserInvestmentInfos
{
    public string key;
    public long sats;
}

[System.Serializable]
public class AdvertiserClientInfos
{
    public string name;
    public string description;
    public string[] squareTextureLocations = new string[] { "", "" } ;
    public string[] pickupTextureLocations;
    public string[] horizontalTextureLocations;
    public string[] verticalTextureLocations;
}

[System.Serializable]
public class Advertiser
{
    public string key;
    public string name;
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

