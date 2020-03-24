using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt.Ads;
using Improbable.Gdk.Subscriptions;

public class ServerAdManagerBehaviour : MonoBehaviour
{
    [Require] AdvertisingConmponentWriter advertisingConmponentWriter;

    private void OnEnable()
    {
        UpdateAdvertisers();
    }

    public void UpdateAdvertisers()
    {

        //Todo get from backend!
        //List<AdvertiserSource> sources = new List<AdvertiserSource>();
        List<AdvertiserSource> sources = testSources;

        advertisingConmponentWriter.SendUpdate(new AdvertisingConmponent.Update()
        {
            CurrentAdvertisers = sources
        });
    }


    public bool test;
    [SerializeField] public List<AdvertiserSource> testSources;


    private void Update()
    {
        if (test)
        {
            test = false;
            UpdateAdvertisers();
        }
    }
}
