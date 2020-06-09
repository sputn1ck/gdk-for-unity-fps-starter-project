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

    public async Task<Advertiser> GetAdvertiser(AdvertiserInvestmentInfo aii)
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

    public async Task<List<AdvertiserInvestment>> GetAdvertiserInvestments(ListAdvertiserResponse aiis)
    {
        List<AdvertiserInvestment> advis = new List<AdvertiserInvestment>();
        List<Task> tasks = new List<Task>();
        foreach (var aii in aiis.Advertisers)
        {
            tasks.Add(AddNewAdvertiserInvestmentToList(advis, aii));
        }
        await Task.WhenAll(tasks);

        return advis;
    }

    public async Task<List<AdvertiserInvestment>> GetAdvertiserInvestments(List<AdvertiserSource> aiis)
    {
        List<AdvertiserInvestment> advis = new List<AdvertiserInvestment>();
        List<Task> tasks = new List<Task>();
        foreach (var aii in aiis)
        {
            tasks.Add(AddNewAdvertiserInvestmentToList(advis, aii));
        }
        await Task.WhenAll(tasks);

        return advis;
    }

    public async Task AddNewAdvertiserInvestmentToList(List<AdvertiserInvestment> list, AdvertiserInvestmentInfo aii)
    {
        Advertiser adv = await GetAdvertiser(aii);
        list.Add(new AdvertiserInvestment(adv, aii.Investment));
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



