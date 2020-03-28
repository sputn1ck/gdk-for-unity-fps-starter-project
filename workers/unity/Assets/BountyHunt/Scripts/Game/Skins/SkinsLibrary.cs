using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BBH/Skins/SkinsLibrary")]
public class SkinsLibrary : ScriptableObject
{

    public List<SkinGroup> _maskSkins;
    public List<SkinGroup> _bodySkins;

    private Dictionary<string, SkinAndGroup> skinsByID;

    private void Awake()
    {
        InitializeDictionary();
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

    void InitializeDictionary()
    {
        skinsByID = new Dictionary<string, SkinAndGroup>();
        foreach(SkinGroup group in _maskSkins)
        {
            foreach(Skin skin in group.skins)
            {
                skinsByID.Add(skin.ID, new SkinAndGroup(skin, group));
            }
        }
    }

}

[System.Serializable]
public class SkinGroup
{
    public enum skinSlot { BODY, MASK }

    public skinSlot slot;
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
