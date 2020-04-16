using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BBH/Skins/Skin")]
public class Skin : ScriptableObject
{
    public string ID;
    public Color identificationColor = Color.white;
    public long price;
    public bool owned;
    public Material material;
    [HideInInspector] public SkinGroup group;
}
