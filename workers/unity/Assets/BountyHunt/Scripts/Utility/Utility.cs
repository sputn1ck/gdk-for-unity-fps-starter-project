using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Utility
{

    public static void resetLocalTransform(this Transform t)
    {
        t.localPosition = Vector3.zero;
        t.localScale = Vector3.one;
        t.localRotation = Quaternion.identity;
    }

    public static void Log(string message, Color color)
    {
        Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));
    }

    public static bool IsEditingInpputfield(this EventSystem sys)
    {
        GameObject current = sys.currentSelectedGameObject;
        if (!current) return false;
        TMP_InputField field = current.gameObject.GetComponent<TMP_InputField>();
        if (!field || !field.isFocused) return false;
        return true;
    }

}
