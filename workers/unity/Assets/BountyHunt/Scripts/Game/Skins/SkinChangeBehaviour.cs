using Bountyhunt;
using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinChangeBehaviour : MonoBehaviour
{
    [Require] private HunterComponentReader hunterComponentReader;
    [SerializeField] private Transform RendererParent;
    private Renderer[] renderers;

    public void Awake()
    {
        renderers = RendererParent.GetComponentsInChildren<SkinnedMeshRenderer>();
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

    private void UpdateSkin(string skinId)
    {
        var skin = SkinsLibrary.MasterInstance.GetSkin(skinId, SkinSlot.BODY);
        if (skin.material != null)
        {
            foreach (var renderer in renderers)
            {
                renderer.material = skin.material;
            }
        }
    }
}
