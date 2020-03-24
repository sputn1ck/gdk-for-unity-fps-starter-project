using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Bountyhunt.Ads;

public class ClientAdManagerBehaviour : MonoBehaviour
{
    [Require] AdvertisingConmponentReader advertisingConmponentReader;

    public static ClientAdManagerBehaviour instance;

    public Material defaultSquareAdMaterial;

    private List<Advertiser> advertisers;

    [HideInInspector] public long totalSponsoredSats;


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
        advertisers = new List <Advertiser>();
    }

    private void OnEnable()
    {
        advertisingConmponentReader.OnCurrentAdvertisersUpdate += UpdateAdvertisers;
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
        //Todo maybe don't search all ads every time?
        AdBillboard[] banners = FindObjectsOfType<AdBillboard>();
        Debug.Log("Total ad billboards Count: " + banners.Length);
        Debug.Log("Total advertisers Count: " + advertisers.Count);

        banners = banners.OrderBy(x => Random.value).ToArray<AdBillboard>();
        List<AdBillboard> bannersLeft = banners.ToList();

        foreach (Advertiser adv in advertisers)
        {
            int count = Mathf.Max(1, (int)(banners.Length * adv.investment / totalSponsoredSats));

            for (int j = 0; j < count; j++)
            {
                if (bannersLeft.Count == 0)
                {
                    Debug.LogWarning("no BannersLeft!");
                    break;
                }
                bannersLeft[0].SetAdvertiser(adv);
                bannersLeft.RemoveAt(0);
            }
        }

        foreach (AdBillboard ab in bannersLeft)
        {
            ab.SetAdvertiser(GetRandomAdvertiser());
        }

    }

    void SortInvestments()
    {
        totalSponsoredSats = 0;
        foreach (Advertiser adv in advertisers)
        {
            totalSponsoredSats += adv.investment;
        }

        advertisers = advertisers.OrderByDescending(o => o.investment).ToList<Advertiser>();
    }

    public Advertiser GetRandomAdvertiser()
    {
        var winningTicket = Random.Range(0, totalSponsoredSats);
        long ticket = 0;
        foreach (var adv in advertisers)
        {
            ticket += adv.investment;
            if (ticket > winningTicket)
                return adv;

        }
        return advertisers[0];
    }

    public async void UpdateAdvertisers(List<AdvertiserSource> advertiserSources)
    {
        
        List<Task> tasks = new List<Task>();

        advertisers.Clear();
        foreach (AdvertiserSource source in advertiserSources)
        {
            tasks.Add(loadAdvertiser(source));
        }
        await Task.WhenAll(tasks);


        Initialize();
    }

    private static async Task<List<Texture2D>> getTexturesFromURLList(List<string> urls)
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

    private async Task loadAdvertiser(AdvertiserSource source)
    {
        Advertiser advertiser = new Advertiser();
        advertiser.name = source.Name;
        advertiser.investment = source.Investment;
        advertiser.squareAdTextures = await getTexturesFromURLList(source.SquareTextureLinks);
        
        advertiser.Initialize();
        advertisers.Add(advertiser);
    }

}


[System.Serializable]
public class Advertiser
{
    public long investment;
    public string name;
    public string description;
    public List<Texture2D> squareAdTextures;

    Dictionary<AdMaterialType, List<Material>> materials;

    public void Initialize()
    {
        materials = new Dictionary<AdMaterialType, List<Material>>();
        InitializeMatrials(squareAdTextures, AdMaterialType.SQUARE, ClientAdManagerBehaviour.instance.defaultSquareAdMaterial, "_EmissionMap");
        //InitializeMatrials(pickupAdTextures, AdMaterialType.PICKUP, AdManager.instance.defaultPickupAdMaterial, "_MainTex");
        //InitializeMatrials(horizontalAdTextures, AdMaterialType.HORIZONTAL, AdManager.instance.defaultHorizontalAdMaterial, "_EmissionMap");
        //InitializeMatrials(verticalAdTextures, AdMaterialType.VERTICAL, AdManager.instance.defaultVerticalAdMaterial, "_EmissionMap");

    }
    public void InitializeMatrials(List<Texture2D> textures, AdMaterialType type, Material defaultMaterial, string TextureToReplace)
    {
        materials[type] = new List<Material>();
        foreach (var texture in textures)
        {
            var material = ClientAdManagerBehaviour.Instantiate(defaultMaterial);
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
        
    public enum AdMaterialType { SQUARE, HORIZONTAL, VERTICAL}
}

