using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelColourSwitcher : MonoBehaviour
{
    public bool switchColor;
    public Color newBaseColor;
    public Color newMainColor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (switchColor)
        {
            switchColor = false;
            SwitchColor(newBaseColor, newMainColor);
        }
    }

    public void SwitchColor(Color newBaseColor, Color newMainColor)
    {
        var renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach(var renderer in renderers)
        {
            var newMat = Instantiate(renderer.material);
            newMat.SetColor("_Base_Colour", newBaseColor);
            newMat.SetColor("_Main_Colour", newMainColor);
            renderer.material = newMat;
        }
    }

    private void switchRendererColor(SkinnedMeshRenderer renderer, Color baseColor, Color MainColor )
    {
        var newMat = Instantiate(renderer.material);
        newMat.SetColor("Base_Colour", baseColor);
        newMat.SetColor("Main_Colour", baseColor);
        renderer.material = newMat;
    }
}
