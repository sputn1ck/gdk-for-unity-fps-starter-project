using Bbhrpc;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "BBH/Skins/SkinsLibrary")]
public class SkinsLibrary : ScriptableObject
{
    public static SkinsLibrary MasterInstance;

    private Dictionary<string, Skin> skinsByID;


    public string defaultSkinID;
    public List<SkinGroup> groups;

    public Skin GetSkin(string skinID)
    {
        if (!skinsByID.ContainsKey(skinID))
        {
            return skinsByID[defaultSkinID];
        }
        return skinsByID[skinID];
    }

    public SkinGroup GetGroup(string skinID)
    {
        if (!skinsByID.ContainsKey(skinID)) return null;
        return skinsByID[skinID].group;
    }


    public void Initialize()
    {
        skinsByID = new Dictionary<string, Skin>();

        foreach (SkinGroup g in groups)
        {
            foreach (Skin s in g.skins)
            {
                skinsByID[s.ID] = s;
            }
        }
    }

    private void InitializeWithShopSkins(ShopSkin[] shopSkins)
    {

        skinsByID = new Dictionary<string, Skin>();


        List<SkinGroup> groups = new List<SkinGroup>(this.groups);
        this.groups.Clear();
        foreach (SkinGroup g in groups)
        {
            SkinGroup group = Instantiate(g);

            List<Skin> skins = new List<Skin>(group.skins);
            group.skins.Clear();
            foreach (Skin s in skins)
            {
                var shopSkin = shopSkins.FirstOrDefault(ss => ss.Id == s.ID);
                if (shopSkins.Length>0 && shopSkin == null) {
                    continue;
                }
                Skin skin = Instantiate(s);
                skin.group = group;
                skin.price = shopSkin.Price;
                group.skins.Add(skin);
                skinsByID[skin.ID] = skin;
            }
            if (group.skins.Count > 0)
            {
                this.groups.Add(group);
            }
        }
    }


    public void InitializeForCharacterMenu(ShopSkin[] shopSkins, string[] OwnedIDs)
    {
        InitializeWithShopSkins(shopSkins);
        SetOwnedStates(OwnedIDs);
    }
    public void SetOwnedStates(string[] IDs)
    {
        foreach(string id in IDs)
        {
            if(skinsByID.ContainsKey(id))
                skinsByID[id].owned = true;
        }
    }


}
