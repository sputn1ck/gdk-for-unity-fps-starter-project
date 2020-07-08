using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinChanger : MonoBehaviour
{
    [SerializeField] private Transform RendererParent;
    private Renderer[] renderers;

    public void Awake()
    {
        renderers = RendererParent.GetComponentsInChildren<SkinnedMeshRenderer>();
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
