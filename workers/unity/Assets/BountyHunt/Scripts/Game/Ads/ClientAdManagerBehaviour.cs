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

    //private List<Advertiser> advertisers;
    private List<AdvertiserInvestment> advertiserInvestments;
    [HideInInspector] public long totalSponsoredSats;

    List<VideoPlayer> usedVideoPlayers = new List<VideoPlayer>();
    List<VideoPlayer> freeVideoPlayers = new List<VideoPlayer>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);

        advertiserInvestments = new List<AdvertiserInvestment>();
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
        InitializeAllBillboards();
        ClientEvents.instance.onUpdateBillboardAdvertisers.Invoke(advertiserInvestments);
    }
    void InitializeAllBillboards()
    {
        //Todo maybe don't search all ads every time?
        AdBillboard[] banners = FindObjectsOfType<AdBillboard>();
        Debug.Log("Total ad billboards Count: " + banners.Length);
        Debug.Log("Total advertisers Count: " + advertiserInvestments.Count);

        banners = banners.OrderBy(x => Random.value).ToArray<AdBillboard>();
        List<AdBillboard> bannersLeft = banners.ToList();

        foreach (AdvertiserInvestment adv in advertiserInvestments)
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
        if(advertiserInvestments.Count >= 1)
        {

            foreach (AdBillboard ab in bannersLeft)
            {
                ab.SetAdvertiser(GetRandomAdvertiserInvestment());
            }
        }

    }

    void SortInvestments()
    {
        totalSponsoredSats = 0;
        foreach (var adv in advertiserInvestments)
        {
            totalSponsoredSats += adv.investment;
        }
        
        advertiserInvestments= advertiserInvestments.OrderByDescending(o => o.investment).ToList();
    }

    public AdvertiserInvestment GetRandomAdvertiserInvestment()
    {
        var winningTicket = Random.Range(0, totalSponsoredSats);
        long ticket = 0;
        foreach (var adv in advertiserInvestments)
        {
            ticket += adv.investment;
            if (ticket > winningTicket)
                return adv;

        }
        return advertiserInvestments.First();
    }

    public AdvertiserInvestment[] GetRandomAdvertisers(int count)
    {
        AdvertiserInvestment[] answer = new AdvertiserInvestment[Mathf.Min(count, this.advertiserInvestments.Count)];

        long totSats = totalSponsoredSats;
        List<AdvertiserInvestment> tempAdvertisers = new List<AdvertiserInvestment>(this.advertiserInvestments);

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

        advertiserInvestments.Clear();
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

    

    private async Task loadAdvertiser(AdvertiserSource source)
    {
        
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

