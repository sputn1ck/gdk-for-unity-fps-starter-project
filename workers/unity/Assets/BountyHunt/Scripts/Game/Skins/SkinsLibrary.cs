using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BBH/Skins/SkinsLibrary")]
public class SkinsLibrary : ScriptableObject
{

    [SerializeField] private List<SkinSlotSettings> slotSettings;

    public Dictionary<SkinSlot, SkinSlotSettings> settings;

    private Dictionary<string, Skin> skinsByID;

    public void Init()
    {
        InitializeDictionaries();
    }

    
    public Skin GetSkin(string skinID, SkinSlot slot)
    {
        if (!skinsByID.ContainsKey(skinID)) return skinsByID[settings[slot].defaultSkinID];
        return skinsByID[skinID];
    }

    public SkinGroup GetGroup(string skinID,SkinSlot slot)
    {
        if (!skinsByID.ContainsKey(skinID)) return null;
        return skinsByID[skinID].group;
    }


    void InitializeDictionaries()
    {
        
        settings = new Dictionary<SkinSlot, SkinSlotSettings>();
        foreach(SkinSlotSettings sss in slotSettings)
        {
            settings[sss.slot] = sss;
        }

        skinsByID = new Dictionary<string, Skin>();

        foreach(var v in settings)
        {
            foreach (SkinGroup group in v.Value.groups)
            {

                group.slot = v.Value.slot;
                foreach (Skin skin in group.skins)
                {
                    skin.group = group;
                    skinsByID[skin.ID]= skin;
                }
            }
        }

    }

}
[System.Serializable]
public class SkinSlotSettings
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
