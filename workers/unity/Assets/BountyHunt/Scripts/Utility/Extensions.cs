using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{

    public static void resetLocalTransform(this Transform t)
    {
        t.localPosition = Vector3.zero;
        t.localScale = Vector3.one;
        t.localRotation = Quaternion.identity;
    }

}
