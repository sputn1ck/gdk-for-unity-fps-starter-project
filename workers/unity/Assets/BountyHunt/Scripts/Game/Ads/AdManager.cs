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

    public async void UpdateAdvertisers(List<AdvertiseInvestmentInfos> args)
    {
        
        advertisers.Clear();
        foreach(AdvertiseInvestmentInfos aav in args)
        {
            Advertiser addy = await RequestAdvertiser(aav.key);
            addy.satsDonation = aav.sats;
            advertisers.Add(addy);
        }

        Initialize();
    }

    public static async Task<List<Texture2D>> getTexturesFromURLArray(string[] urls)
    {
        List<Texture2D> textures  = new List<Texture2D>();
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

    public static async Task<Advertiser> RequestAdvertiser(string key)
    {
        Advertiser advertiser = new Advertiser();


        //Todo get AdvertiserClientInfos from backend
        AdvertiserClientInfos aci = new AdvertiserClientInfos(); 

        advertiser.name = aci.name;
        advertiser.description = aci.description;
        advertiser.squareAdTextures = await getTexturesFromURLArray(aci.squareTextureLocations);
        advertiser.horizontalAdTextures = await getTexturesFromURLArray(aci.horizontalTextureLocations);
        advertiser.verticalAdTextures= await getTexturesFromURLArray(aci.verticalTextureLocations);
        advertiser.pickupAdTextures= await getTexturesFromURLArray(aci.pickupTextureLocations);

        return advertiser;
    }

}



[System.Serializable]
public struct AdvertiseInvestmentInfos
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

