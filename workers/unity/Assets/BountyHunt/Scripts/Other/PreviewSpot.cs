using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSpot : MonoBehaviour
{
    public GameObject character;
    List<Renderer> bodyRenderers;
    public static PreviewSpot Instance;

    private void Awake()
    {
        Instance = this;
        bodyRenderers = new List<Renderer>();
        var renderers = character.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var r in renderers)
        {
            bodyRenderers.Add(r);
        }
    }

    public void TurnCharacter(float degrees)
    {
        character.transform.Rotate(new Vector3(0, degrees, 0));
    }

    public void SetSkin(Skin skin)
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
