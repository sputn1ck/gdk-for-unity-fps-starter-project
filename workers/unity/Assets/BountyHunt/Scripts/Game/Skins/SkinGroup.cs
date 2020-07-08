using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BBH/Skins/SkinGroup")]
public class SkinGroup : ScriptableObject
{
    public string groupName;
    public Sprite sprite;
    [SerializeField] private List<Skin> skins;

    public List<Skin> GetAllSkins()
    {
        return skins;
    } 
}
