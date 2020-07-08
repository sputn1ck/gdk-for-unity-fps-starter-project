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
        AvailableItems = new Dictionary<string, SkinItem>();

        foreach(ShopSkin shopSkin in shopSkins)
        {
            Skin skin = SkinsLibrary.Instance.GetSkin(shopSkin.Id);
            SkinItem item = new SkinItem
            {
                skin = skin,
                price = shopSkin.Price,
                owned = false
            };
            AvailableItems[shopSkin.Id]=item;
            AddItemToGroups(item);
        }

        foreach(string skinID in inventory.OwnedSkins)
        {
            if (AvailableItems.ContainsKey(skinID))
            {
                AvailableItems[skinID].owned = true;
            }
            else
            {
                Skin skin = SkinsLibrary.Instance.GetSkin(skinID);
                SkinItem item = new SkinItem
                {
                    skin = skin,
                    price = 0,
                    owned = true
                };
                AvailableItems[skinID] = item;
                AddItemToGroups(item);
            }
        }
        EquippedSkin = AvailableItems[inventory.EquippedSkin];
    }

    static void AddItemToGroups(SkinItem item)
    {
        if (!ItemGroupDict.ContainsKey(item.skin.group))
        {
            ItemGroupDict[item.skin.group] = new List<SkinItem>();
        }
        ItemGroupDict[item.skin.group].Add(item);
    }
}

public class SkinItem
{
    public Skin skin;
    public long price;
    public bool owned;
}

