using Bbhrpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

    public class DummyAdvertiserStore : MonoBehaviour, IAdvertiserStore
    {
    public Texture2D advertiserTexture;

    public Task<Texture2D> GetAdvertiser(string advertiserId, string advertiserUrl)
    {
        return Task.FromResult(advertiserTexture);
    }


    public async Task Initialize(ListAdvertiserResponse advertisers)
    {
        await Task.Delay(UnityEngine.Random.Range(100, 1000));
    }
}

