using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSpot : MonoBehaviour
{
    public GameObject character;
    public Renderer maskRenderer;
    List<Renderer> bodyRenderers;
    public static PreviewSpot Instance;

    private void Awake()
    {
        Instance = this;
        bodyRenderers = new List<Renderer>();
        var renderers = character.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var r in renderers)
        {
            if (r != maskRenderer)
            {
                bodyRenderers.Add(r);
            }
        }
    }

    public void TurnCharacter(float degrees)
    {
        character.transform.Rotate(new Vector3(0, degrees, 0));
    }

    public void SetSkin(Skin skin)
    {
        if(skin.group.slot == SkinSlot.BODY)
        {
            if(skin.material != null)
            {
                foreach(var renderer in bodyRenderers)
                {
                    renderer.material = skin.material;
                }
            }
        }
    }
}
