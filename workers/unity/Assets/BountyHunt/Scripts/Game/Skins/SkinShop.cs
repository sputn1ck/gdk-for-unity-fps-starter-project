using Bbhrpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SkinShop
{
    public static long PlayerBalance;
    public static SkinItem EquippedSkin;
    public static Dictionary<string,SkinItem> AvailableItems;
    public static Dictionary<SkinGroup, List<SkinItem>> ItemGroupDict;

    public static async Task Refresh()
    {
        ShopSkin[] shopSkins;
        SkinInventory inventory;
        long balance;
        try
        {
            shopSkins = await PlayerServiceConnections.instance.BackendPlayerClient.GetAllSkins();
            inventory = await PlayerServiceConnections.instance.BackendPlayerClient.GetSkinInventory();
            var b = (await PlayerServiceConnections.instance.DonnerDaemonClient.GetWalletBalance());
            balance = b.DaemonBalance;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return;
        }
        ItemGroupDict = new Dictionary<SkinGroup, List<SkinItem>>();

        PlayerBalance = balance;

        if (AvailableItems == null) AvailableItems = new Dictionary<string, SkinItem>();

        Dictionary<string, SkinItem> oldItems = new Dictionary<string, SkinItem>(AvailableItems);
        AvailableItems.Clear();
        clearGroups();

        foreach(ShopSkin shopSkin in shopSkins)
        {
            
            AddOrReuseItem(oldItems, shopSkin.Id, false, shopSkin.Price);
        }

        foreach(string skinID in inventory.OwnedSkins)
        {
            if (AvailableItems.ContainsKey(skinID))
            {
                AvailableItems[skinID].owned = true;
            }
            else
            {
                AddOrReuseItem(oldItems, skinID, true);
            }
        }
        EquippedSkin = AvailableItems[inventory.EquippedSkin];
        CleanGroups();
        
    }

    static void AddOrReuseItem(Dictionary<string, SkinItem>oldItems, string skinID, bool owned, long price = 0)
    {
        SkinItem item;
        if (oldItems.ContainsKey(skinID))
        {
            item = oldItems[skinID];
        }
        else item = new SkinItem();

        item.skin = SkinsLibrary.Instance.GetSkin(skinID);
        if (price != 0) item.price = price;
        item.owned= owned;
        AvailableItems[skinID] = item;
        oldItems.Remove(skinID);
        AddItemToGroups(item);
    }

    static void AddItemToGroups(SkinItem item)
    {
        if (!ItemGroupDict.ContainsKey(item.skin.group))
        {
            ItemGroupDict[item.skin.group] = new List<SkinItem>();
        }
        ItemGroupDict[item.skin.group].Add(item);
    }

    static void CleanGroups()
    {
        List<SkinGroup> groupsToRemove = new List<SkinGroup>();

        foreach (var g in ItemGroupDict)
        {
            if (g.Value.Count == 0)
            {
                groupsToRemove.Add(g.Key);
            }
        }

        foreach(SkinGroup g in groupsToRemove)
        {
            ItemGroupDict.Remove(g);
        }
    }
    static void clearGroups()
    {
        foreach (var g in ItemGroupDict)
        {
            g.Value.Clear();
        }
    }
}

public class SkinItem
{
    public Skin skin;
    public long price=0;
    public bool owned;
}

