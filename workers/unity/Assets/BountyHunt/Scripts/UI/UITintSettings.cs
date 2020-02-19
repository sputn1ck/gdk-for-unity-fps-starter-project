using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[CreateAssetMenu(menuName = "BBH/UI/TintSettings")]
public class UITintSettings : ScriptableObject
{

    public Color primaryColor;
    public Color secondaryColor;

    private void OnValidate()
    {
        UITinter.tintDict[TintColor.Primary] = primaryColor;
        UITinter.tintDict[TintColor.Secondary] = secondaryColor;
        UITinter.tintEvent.Invoke();
    }


}
