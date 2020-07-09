using Bountyhunt;
using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinChangeBehaviour : MonoBehaviour
{
    [Require] private HunterComponentReader hunterComponentReader;
    [SerializeField] private List<Transform> RendererParents;
    private Renderer[] renderers;

    public void Awake()
    {
        List<Renderer> r = new List<Renderer>();
        foreach(Transform p in RendererParents)
        {
            r.AddRange(p.GetComponentsInChildren<SkinnedMeshRenderer>());
        }
        renderers = r.ToArray();
    }
    public void OnEnable()
    {
        hunterComponentReader.OnEquippedSkinUpdate += OnEquippedSkinUpdate;
        UpdateSkin(hunterComponentReader.Data.EquippedSkin);
    }

    private void OnEquippedSkinUpdate(string skinId)
    {
        UpdateSkin(skinId);
    }

    public void UpdateSkin(string skinId)
    {
        var skin = SkinsLibrary.Instance.GetSkin(skinId);
        if (skin.material != null)
        {
            foreach (var renderer in renderers)
            {
                renderer.material = skin.material;
            }
        }
    }
}
