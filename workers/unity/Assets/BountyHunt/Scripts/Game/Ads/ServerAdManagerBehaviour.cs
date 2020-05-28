using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Improbable.Gdk.Subscriptions;

public class ServerAdManagerBehaviour : MonoBehaviour
{
    [Require] AdvertisingComponentWriter advertisingConmponentWriter;

    private void OnEnable()
    {
        //UpdateAdvertisers();
    }

    /*
    public void UpdateAdvertisers()
    {

        //Todo get from backend!
        //List<AdvertiserSource> sources = new List<AdvertiserSource>();
        List<AdvertiserSource> sources = testSources;

        advertisingConmponentWriter.SendUpdate(new AdvertisingComponent.Update()
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
    */
}
