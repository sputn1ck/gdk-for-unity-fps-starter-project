using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BBH/Skins/SkinsLibrary")]
public class SkinsLibrary : ScriptableObject
{

    [SerializeField] private List<SkinSlotSettings> slotSettings;

    public Dictionary<SkinSlot, SkinSlotSettings> settings;

    private Dictionary<string, SkinAndGroup> skinsByID;

    public void Init()
    {
        InitializeDictionaries();
    }

    
    public Skin GetSkin(string skinID)
    {
        return skinsByID[skinID].skin;
    }
    public SkinGroup GetGroup(string skinID)
    {
        return skinsByID[skinID].group;
    }


    public struct SkinAndGroup
    {
        public Skin skin;
        public SkinGroup group;

        public SkinAndGroup(Skin skin, SkinGroup group)
        {
            this.skin = skin;
            this.group = group;
        }
    }

    void InitializeDictionaries()
    {
        
        settings = new Dictionary<SkinSlot, SkinSlotSettings>();
        foreach(SkinSlotSettings sss in slotSettings)
        {
            settings[sss.slot] = sss;
        }

        skinsByID = new Dictionary<string, SkinAndGroup>();

        foreach(var v in settings)
        {
            foreach (SkinGroup group in v.Value.groups)
            {
                group.slot = v.Value.slot;
                foreach (Skin skin in group.skins)
                {
                    skinsByID[skin.ID]= new SkinAndGroup(skin, group);
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

[System.Serializable]
public class SkinGroup
{
    public string name;
    public Sprite sprite;
    public List<Skin> skins;
    [HideInInspector] public SkinSlot slot;
}

[System.Serializable]
public class Skin
{
    public string ID;
    public Color identificationColor = Color.white;
    public long price;
    public bool owned;
}

public enum SkinSlot { BODY, MASK }
