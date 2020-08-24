using Bbhrpc;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "BBH/Skins/SkinsLibrary")]
public class SkinsLibrary : ScriptableObject
{
    public static SkinsLibrary Instance;

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


    public void Initialize()
    {
        skinsByID = new Dictionary<string, Skin>();

        foreach (SkinGroup g in groups)
        {
            List<Skin> skins = g.GetAllSkins();
            foreach (Skin s in skins)
            {
                s.group = g;
                skinsByID[s.ID] = s;
            }
        }
    }

}
