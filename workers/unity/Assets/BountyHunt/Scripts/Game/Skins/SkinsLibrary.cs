using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BBH/Skins/SkinsLibrary")]
public class SkinsLibrary : ScriptableObject
{

    [SerializeField] private List<SkinSlotSettings> slotSettings;

    public Dictionary<SkinSlot, SkinSlotSettings> settings;

    private Dictionary<string, Skin> skinsByID;

    public Skin GetSkin(string skinID, SkinSlot slot)
    {
        Debug.Log("slot: " + slot);
        Debug.Log("skin: " + skinID);
        Debug.Log("default: " + settings[slot].defaultSkinID);

        if (!skinsByID.ContainsKey(skinID)) return skinsByID[settings[slot].defaultSkinID];
        return skinsByID[skinID];
    }

    public SkinGroup GetGroup(string skinID,SkinSlot slot)
    {
        if (!skinsByID.ContainsKey(skinID)) return null;
        return skinsByID[skinID].group;
    }


    public void Initialize()
    {
        Initialize(new List<string>());
    }

    public void Initialize(List<string> IDs)
    {

        settings = new Dictionary<SkinSlot, SkinSlotSettings>();
        foreach (SkinSlotSettings sss in slotSettings)
        {
            settings[sss.slot] = sss;
        }

        skinsByID = new Dictionary<string, Skin>();


        foreach (var v in settings)
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
                    if (IDs.Count>0 && !IDs.Contains(s.ID)) {
                        continue;
                    }
                    Skin skin = Instantiate(s);
                    skin.group = group;
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

    public void Initialize(List<string> AllIDs, List<string> OwnedIDs)
    {
        Initialize(AllIDs);
        SetOwnedStates(OwnedIDs);
    }

    public void SetOwnedStates(List<string> IDs)
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
