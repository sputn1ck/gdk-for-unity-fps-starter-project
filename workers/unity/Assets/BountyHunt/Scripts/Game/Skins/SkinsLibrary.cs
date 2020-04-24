using Bbhrpc;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "BBH/Skins/SkinsLibrary")]
public class SkinsLibrary : ScriptableObject
{
    public static SkinsLibrary MasterInstance;

    [SerializeField] private List<SkinSlotSettings> slotSettings;

    public Dictionary<SkinSlot, SkinSlotSettings> skinSlotSettings;

    private Dictionary<string, Skin> skinsByID;

    public Skin GetSkin(string skinID, SkinSlot slot)
    {
        //Debug.Log("slot: " + slot);
        //Debug.Log("skin: " + skinID);
        //Debug.Log("default: " + settings[slot].defaultSkinID);

        if (!skinsByID.ContainsKey(skinID))
        {
            return skinsByID[skinSlotSettings[slot].defaultSkinID];
        }
        return skinsByID[skinID];
    }

    public SkinGroup GetGroup(string skinID,SkinSlot slot)
    {
        if (!skinsByID.ContainsKey(skinID)) return null;
        return skinsByID[skinID].group;
    }


    public void Initialize()
    {
        skinSlotSettings = new Dictionary<SkinSlot, SkinSlotSettings>();
        foreach (SkinSlotSettings sss in slotSettings)
        {
            skinSlotSettings[sss.slot] = sss;
        }

        skinsByID = new Dictionary<string, Skin>();


        foreach (var setting in skinSlotSettings)
        {
            foreach (SkinGroup g in setting.Value.groups)
            {
                foreach (Skin s in g.skins)
                {
                    skinsByID[s.ID] = s;
                }
            }
        }
    }

    private void InitializeWithShopSkins(ShopSkin[] shopSkins)
    {

        skinSlotSettings = new Dictionary<SkinSlot, SkinSlotSettings>();
        foreach (SkinSlotSettings sss in slotSettings)
        {
            skinSlotSettings[sss.slot] = sss;
        }

        skinsByID = new Dictionary<string, Skin>();


        foreach (var v in skinSlotSettings)
        {
            List<SkinGroup> groups = new List<SkinGroup>(v.Value.groups);
            v.Value.groups.Clear();
            foreach (SkinGroup g in groups)
            {
                SkinGroup group = Instantiate(g);
                group.slot = v.Value.slot;

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
                    v.Value.groups.Add(group);
                }
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
        foreach(var sk in skinsByID)
        {
            if (IDs.Contains(sk.Key))
            {
                sk.Value.owned = true;
            }
            else
            {
                sk.Value.owned = false;
            }
        }
    }


}
[System.Serializable]
public struct SkinSlotSettings
{
    public SkinSlot slot;
    public string defaultSkinID;
    public List<SkinGroup> groups;

    public static implicit operator List<SkinGroup>(SkinSlotSettings s)
    {
        return s.groups;
    }
}



public enum SkinSlot { BODY, MASK }
