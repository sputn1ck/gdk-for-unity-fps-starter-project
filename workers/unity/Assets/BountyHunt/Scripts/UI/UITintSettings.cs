using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[CreateAssetMenu(menuName = "BBH/UI/TintSettings")]
public class UITintSettings : ScriptableObject
{
    public BBHUIManager OnScreenUIPrefab;

    public Color primaryColor;
    public Color secondaryColor;

    private void OnValidate()
    {
        
        UITinter.setColor(TintColor.Primary, primaryColor);
        UITinter.setColor(TintColor.Secondary, secondaryColor);

        OnScreenUIPrefab.primaryUIColor = primaryColor;
        OnScreenUIPrefab.secondaryUIColor = secondaryColor;
    }


}
