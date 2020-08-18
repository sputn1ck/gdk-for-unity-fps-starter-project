using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using System.Linq;
using Improbable.Gdk.Subscriptions;

public class ClientRoomAdManagerBehaviour : MonoBehaviour
{

    [Require] private RoomAdvertingManagerReader RoomAdvertingManagerReader;

    private RoomManagerClientBehaviour RoomManagerClientBehaviour;

    private List<AdvertiserInvestment> advertiserInvestments;
    [HideInInspector] public long totalSponsoredSats;
    private bool mapLoaded = false;
    private void Awake()
    {
        RoomManagerClientBehaviour = GetComponent<RoomManagerClientBehaviour>();
        RoomManagerClientBehaviour.OnMapLoaded += OnMapLoaded;
    }
    private void OnEnable()
    {
        RoomAdvertingManagerReader.OnCurrentAdvertisersUpdate += RoomAdvertingManagerReader_OnCurrentAdvertisersUpdate;
    }
    private void OnMapLoaded()
    {
        mapLoaded = true;
        UpdateAdvertisers(RoomAdvertingManagerReader.Data.CurrentAdvertisers);
    }
    private void RoomAdvertingManagerReader_OnCurrentAdvertisersUpdate(List<AdvertiserSource> obj)
    {

        UpdateAdvertisers(obj);
    }

    async void UpdateAdvertisers(List<AdvertiserSource> advertiserSources)
    {
        if (!mapLoaded)
        {
            
            return;
        }
        var map = RoomManagerClientBehaviour.map;
        var banners = map.GetBillboards();
        if(banners == null || banners.Length < 1)
        {
            return;
        }
        var bannersLeft = banners.ToList();
        advertiserInvestments = await PlayerServiceConnections.instance.AdvertiserStore.GetAdvertiserInvestments(advertiserSources);
        totalSponsoredSats = 0;
        foreach (var adv in advertiserInvestments)
        {
            totalSponsoredSats += adv.investment;
        }

        advertiserInvestments = advertiserInvestments.OrderByDescending(o => o.investment).ToList();
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
        if (advertiserInvestments.Count >= 1)
        {

            foreach (AdBillboard ab in bannersLeft)
            {
                ab.SetAdvertiser(GetRandomAdvertiserInvestment(totalSponsoredSats, advertiserInvestments));
            }
        }

        ClientEvents.instance.onUpdateBillboardAdvertisers.Invoke(advertiserInvestments);

    }
    public AdvertiserInvestment GetRandomAdvertiserInvestment(long totalSponsoredSats, List<AdvertiserInvestment> advertiserInvestments)
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

        for (int i = 0; i < answer.Length; i++)
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

}
