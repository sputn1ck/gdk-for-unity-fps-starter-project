using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITintManager : MonoBehaviour
{

    [SerializeField] public List<TintingPair> UITints;


    private void Awake()
    {

        foreach (TintingPair tp in UITints)
        {
            UITinter.setColor(tp.tint, tp.color);
        }

    }

}
