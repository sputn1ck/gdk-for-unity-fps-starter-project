using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Bountyhunt;
using UnityEngine.Video;

public class ClientAdManagerBehaviour : MonoBehaviour
{
    [Require] AdvertisingComponentReader advertisingConmponentReader;

    public static ClientAdManagerBehaviour instance;

    public Material defaultSquareAdMaterial;

    private List<Advertiser> advertisers;

    [HideInInspector] public long totalSponsoredSats;

    List<VideoPlayer> usedVideoPlayers = new List<VideoPlayer>();
    List<VideoPlayer> freeVideoPlayers = new List<VideoPlayer>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);

        advertisers = new List <Advertiser>();
    }

    private void OnEnable()
    {
        advertisingConmponentReader.OnCurrentAdvertisersUpdate += UpdateAdvertisers;
        UpdateAdvertisers(advertisingConmponentReader.Data.CurrentAdvertisers);
    }

    private void Start()
    {
        //ClientEvents.instance.onMapLoaded.AddListener(Initialize);
    }

    

    void Initialize()
    {
        SortInvestments();
        InitializeAllAds();
        ClientEvents.instance.onUpdateAdvertisers.Invoke(advertisers);
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
        if(advertisers.Count >= 1)
        {

            foreach (AdBillboard ab in bannersLeft)
            {
                ab.SetAdvertiser(GetRandomAdvertiser());
            }
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

    public Advertiser[] GetRandomAdvertisers(int count)
    {
        Advertiser[] answer = new Advertiser[Mathf.Min(count, this.advertisers.Count)];

        long totSats = totalSponsoredSats;
        List<Advertiser> tempAdvertisers = new List<Advertiser>(this.advertisers);

        for(int i = 0; i<answer.Length; i++)
        {
            var winningTicket = Random.Range(0, totSats);
            long ticket = 0;
            foreach (var adv in tempAdvertisers)
            {
                ticket += adv.investment;
                if (ticket > winningTicket)
                {
                    answer[i] = adv;
                    tempAdvertisers.Remove(adv);
                    totSats -= adv.investment;
                    break;
                }

            }
        }
        return answer;

    }

    public async void UpdateAdvertisers(List<AdvertiserSource> advertiserSources)
    {
        
        List<Task> tasks = new List<Task>();

        advertisers.Clear();
        foreach (VideoPlayer player in usedVideoPlayers)
        {
            freeVideoPlayers.Add(player);
            player.enabled = false;
        }
        usedVideoPlayers.Clear();
        foreach (AdvertiserSource source in advertiserSources)
        {
            tasks.Add(loadAdvertiser(source));
        }
        await Task.WhenAll(tasks);


        Initialize();
    }

    private static async Task<(List<Texture2D>,List<string>)> getTexturesFromURLList(List<string> urls)
    {

        List<Texture2D> textures = new List<Texture2D>();
        List<string> videoUrls= new List<string>();
        if (urls == null)
        {
            Debug.LogWarning("url list is null");
            return (textures,videoUrls);
        }
        foreach (string url in urls)
        {
            string extension = Path.GetExtension(url);
            switch (extension)
            {
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".bmp":
                case ".exr":
                case ".hdr":
                case ".iff":
                case ".pict":
                case ".psd":
                case ".tga":
                case ".tiff":
                case ".gif":
                

                    Texture2D tex = await AwaitRequestTexture.SendAsyncWebRequest(instance, url);

                    if (tex == null)
                    {
                        continue;
                    }

                    Color backgroundColor = Color.white; 
                    var pixels = tex.GetPixels();

                    for (int i = 0; i < pixels.Length; i++)
                    {
                        float a = pixels[i].a;
                        pixels[i] = Color.Lerp(backgroundColor,pixels[i],a);
                        pixels[i].a = a;
                    }
                    tex.SetPixels(pixels);
                    tex.Apply();
                    textures.Add(tex);

                    break;

                case ".dv":
                case ".m4v":
                case ".mov":
                case ".mp4":
                case ".mpg":
                case ".mpeg":
                case ".ogv":
                case ".vp8":
                case ".webm":

                    videoUrls.Add(url);

                    break;

                default:
                    Debug.LogError("File Type not " + extension + "supportet!");
                    break;
            }

            
        }

        return (textures,videoUrls);
    }

    private async Task loadAdvertiser(AdvertiserSource source)
    {
        Advertiser advertiser = new Advertiser();
        advertiser.name = source.Name;
        advertiser.investment = source.Investment;
        advertiser.url = source.Url;
        advertiser.squareMedia = await getTexturesFromURLList(source.SquareTextureLinks);
        advertiser.Initialize();
        advertisers.Add(advertiser);
    }

    public VideoPlayer GetNewVideoPlayer()
    {
        if (freeVideoPlayers.Count>0)
        {
            VideoPlayer p = freeVideoPlayers[0];

            usedVideoPlayers.Add(p);
            freeVideoPlayers.Remove(p);
            p.enabled = true;
            return p;
        }
        VideoPlayer vp = gameObject.AddComponent<VideoPlayer>();
        vp.source = VideoSource.Url;
        vp.isLooping = true;
        vp.renderMode = VideoRenderMode.RenderTexture;
        vp.audioOutputMode = VideoAudioOutputMode.None;
        return vp;
    }

}


[System.Serializable]
public class Advertiser
{
    public long investment;
    public string name;
    public string description;
    public string url;
    public (List<Texture2D>, List<string>) squareMedia;

    Dictionary<AdMaterialType, List<MaterialInfo>> materials;

    public void Initialize()
    {
        materials = new Dictionary<AdMaterialType, List<MaterialInfo>>();
        InitializeMatrials(squareMedia, AdMaterialType.SQUARE, ClientAdManagerBehaviour.instance.defaultSquareAdMaterial, "_EmissionMap");
        //InitializeMatrials(pickupAdTextures, AdMaterialType.PICKUP, AdManager.instance.defaultPickupAdMaterial, "_MainTex");
        //InitializeMatrials(horizontalAdTextures, AdMaterialType.HORIZONTAL, AdManager.instance.defaultHorizontalAdMaterial, "_EmissionMap");
        //InitializeMatrials(verticalAdTextures, AdMaterialType.VERTICAL, AdManager.instance.defaultVerticalAdMaterial, "_EmissionMap");

    }
    public void InitializeMatrials((List<Texture2D>textures,List<string>videolinks) media, AdMaterialType type, Material defaultMaterial, string TextureToReplace)
    {
        materials[type] = new List<MaterialInfo>();
        foreach (var texture in media.textures)
        {
            var material = ClientAdManagerBehaviour.Instantiate(defaultMaterial);
            material.SetTexture(TextureToReplace, texture);
            materials[type].Add(new MaterialInfo(material,texture));
        }

        foreach(string link in media.videolinks)
        {
            var material = ClientAdManagerBehaviour.Instantiate(defaultMaterial);
            VideoPlayer vp = ClientAdManagerBehaviour.instance.GetNewVideoPlayer();
            vp.url = link;
            vp.renderMode = VideoRenderMode.RenderTexture;
            RenderTexture rt = new RenderTexture(512, 512, 0);
            vp.targetTexture = rt;
            material.SetTexture(TextureToReplace, rt);
            materials[type].Add(new MaterialInfo(material,rt));

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

    public List<MaterialInfo> GetMaterials (AdMaterialType type)
    {
        return materials[type];
    }

    public enum AdMaterialType { SQUARE, HORIZONTAL, VERTICAL}
}

public struct MaterialInfo
{
    public Material Material;
    public Texture Texture;

    public MaterialInfo(Material mat,Texture tex)
    {
        Material = mat;
        Texture = tex;
    }
}

