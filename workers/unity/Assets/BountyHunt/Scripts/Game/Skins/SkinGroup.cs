using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BBH/Skins/SkinGroup")]
public class SkinGroup : ScriptableObject
{
    public string groupName;
    public Sprite sprite;
    public List<Skin> skins;
    [HideInInspector] public SkinSlot slot;
}
