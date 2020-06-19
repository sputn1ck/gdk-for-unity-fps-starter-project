using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SponsorsMenuUI : MonoBehaviour, IRefreshableUI
{
    public Transform sponsorTilesContainer;
    public SponsorTileUI sponsorTileUIPrefab;

    public List<SponsorTileUI> tiles;

    private void Awake()
    {
        tiles = sponsorTilesContainer.GetComponentsInChildren<SponsorTileUI>().ToList();

    }
    void OnEnable()
    {
        if (PlayerServiceConnections.instance.ServicesReady)
        {
            RefreshTiles();
        }
    }

    async void RefreshTiles()
    {
        Bbhrpc.ListAdvertiserResponse res;

        try
        {
            res = await PlayerServiceConnections.instance.BackendPlayerClient.ListAdvertisers();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

        List<AdvertiserInvestment> adInvs = await PlayerServiceConnections.instance.AdvertiserStore.GetAdvertiserInvestments(res);

        int missingTilesCount = Mathf.Max(0, adInvs.Count - tiles.Count);

        for (int i = 0; i < missingTilesCount; i++)
        {
            SponsorTileUI tile = Instantiate(sponsorTileUIPrefab, sponsorTilesContainer);
            tiles.Add(tile);
        }

        for (int i = 0; i < tiles.Count; i++)
        {
            SponsorTileUI tile = tiles[i];

            if (i >= adInvs.Count)
            {
                tile.gameObject.SetActive(false);
                continue;
            }
            tile.gameObject.SetActive(true);
            tile.Set(adInvs[i]);

        }

    }

    public void Refresh()
    {
        if (PlayerServiceConnections.instance.ServicesReady)
        {
            RefreshTiles();
        }
    }
}
