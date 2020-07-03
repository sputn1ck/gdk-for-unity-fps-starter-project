using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AdBillboard : MonoBehaviour, ILookAtHandler
{
    public MeshRenderer AdRenderer;
    public Advertiser.AdMaterialType AdType;
    private AdvertiserInvestment advertiserInvestment;
    string billboardID;

    private void Awake()
    {
        var rnd = new System.Random();
        billboardID = Utility.GetUniqueString();
    }

    public void SetAdvertiser(AdvertiserInvestment adInv)
    {
        this.advertiserInvestment = adInv;
        Material mat = this.advertiserInvestment.advertiser.GetRandomMaterial(AdType);
        if (mat == null) return;
        
        AdRenderer.material = mat;
    }

    public void OnLookAtEnter()
    {
        List<(UnityAction, string)> actions = new List<(UnityAction, string)>();
        (UnityAction, string) bookmarkAction = (BookmarkUrl, GameText.AdContextMenuBookmarkActionLabel);
        actions.Add(bookmarkAction);
        string text = Utility.SatsToShortString(advertiserInvestment.investment, true, UITinter.tintDict[TintColor.Sats]);
        ContextMenuArgs args = new ContextMenuArgs
        {
            ReferenceString = billboardID,
            Headline = advertiserInvestment.advertiser.name,
            Text = text,
            Actions = actions
        };
        ContextMenuUI.Instance.Set(args);
    }

    public void OnLookAtExit()
    {
        ContextMenuUI.Instance.Hide(billboardID);
    }

    void BookmarkUrl()
    {
        UrlMemory.AddUrl(advertiserInvestment.advertiser.url);
        ClientEvents.instance.onAnnouncement.Invoke(GameText.linkBookmarkedInfo, Color.white);
    }
}
