using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BBH/Skins/SkinsLibrary")]
public class SkinsLibrary : ScriptableObject
{

    public List<SkinGroup> _maskSkins;
    public List<SkinGroup> _bodySkins;

    public Dictionary<SkinGroup.SkinSlot, List<SkinGroup>> groupsBySlot;

    public string defaultSkin;

    private Dictionary<string, SkinAndGroup> skinsByID;

    public void Init()
    {
        InitializeDictionaries();
    }

    
    public Skin getSkin(string skinID)
    {
        return skinsByID[skinID].skin;
    }
    public SkinGroup getGroup(string skinID)
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
        
        groupsBySlot = new Dictionary<SkinGroup.SkinSlot, List<SkinGroup>>();
        groupsBySlot[SkinGroup.SkinSlot.MASK] = _maskSkins;
        groupsBySlot[SkinGroup.SkinSlot.BODY] = _bodySkins;

        skinsByID = new Dictionary<string, SkinAndGroup>();

        foreach(var v in groupsBySlot)
        {
            foreach (SkinGroup group in v.Value)
            {
                foreach (Skin skin in group.skins)
                {
                    skinsByID[skin.ID]= new SkinAndGroup(skin, group);
                }
            }
        }

        


    }

}

[System.Serializable]
public class SkinGroup
{
    public enum SkinSlot { BODY, MASK }

    public SkinSlot slot;
    public string name;
    public List<Skin> skins;
    public Sprite sprite;
}

[System.Serializable]
public class Skin
{
    public string ID;
    public Color identificationColor;
    public long price;
    public bool owned;
}
