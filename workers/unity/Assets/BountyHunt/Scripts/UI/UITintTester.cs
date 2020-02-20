using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BBH/UI/TintTEster")]
public class UITintTester : ScriptableObject
{
    [SerializeField] public List<TintingPair> UITints;

    private void OnValidate()
    {

        foreach (TintingPair tp in UITints)
        {
            UITinter.setColor(tp.tint, tp.color);
        }
    }


}
