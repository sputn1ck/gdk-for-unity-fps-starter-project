using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Bbhrpc;
using Bountyhunt;

public class AdvertiserStore
{
    Dictionary<string,Advertiser> advertisers = new Dictionary<string,Advertiser>();

    public async Task Initialize(ListAdvertiserResponse advertisers)
    {
        foreach(var adv in advertisers.Advertisers)
        {
            await GetAdvertiser(adv);
        }
    }
    async Task<Advertiser> GetAdvertiser(Bbhrpc.Advertiser advertiser)
    {
        if (this.advertisers.ContainsKey(advertiser.Phash))
        {
            return advertisers[advertiser.Phash];
        }

        Advertiser adv = new Advertiser();
        await adv.Load(advertiser);

        advertisers[advertiser.Phash] = adv;
        return adv;
    }

    async Task<Advertiser> GetAdvertiser(AdvertiserInvestmentInfo aii)
    {
        if (this.advertisers.ContainsKey(aii.Hash))
        {
            return advertisers[aii.Hash];
        }

        Advertiser adv = new Advertiser();
        await adv.Load(aii);

        advertisers[aii.Hash] = adv;
        return adv;
    }
}

public class AdvertiserInvestmentInfo
{
    public string Name;
    public string Hash;
    public string Url;
    public List<string> SquareImageUrls;
    public long Investment;

    public static implicit operator AdvertiserInvestmentInfo (Bbhrpc.Advertiser bbhAdv)
    {
        AdvertiserInvestmentInfo advInfo = new AdvertiserInvestmentInfo
        {
            Name = bbhAdv.Name,
            Hash = bbhAdv.Phash,
            Url = bbhAdv.Url,
            Investment = bbhAdv.Balance,
            SquareImageUrls = new List<string>()
        };

        foreach (string url in bbhAdv.PicUrls)
        {
            advInfo.SquareImageUrls.Add(url);
        }
        return advInfo;
    }

    public static implicit operator AdvertiserInvestmentInfo(AdvertiserSource source)
    {
        AdvertiserInvestmentInfo advInfo = new AdvertiserInvestmentInfo
        {
            Name = source.Name,
            Hash = source.Hash,
            Url = source.Url,
            Investment = source.Investment,
            SquareImageUrls = source.SquareTextureLinks
        };

        return advInfo;
    }

}



