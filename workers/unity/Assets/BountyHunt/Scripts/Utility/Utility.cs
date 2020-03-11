using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Utility
{
    public const ulong BountyInt = 989810498;
    public const ulong MemoInt = 9898104109;


    //static Colors for variation
    public static Color32 failureColor = new Color32(255, 120, 105, 255);
    public const string failureColorHex = "FF7869FF";

    public static Color32 successColor = new Color32(147, 245, 153, 255);
    public const string successColorHex = "93F59AFF";


    public static void resetLocalTransform(this Transform t)
    {
        t.localPosition = Vector3.zero;
        t.localScale = Vector3.one;
        t.localRotation = Quaternion.identity;
    }

    public static long Abs(long input)
    {
        if (input >= 0) return input;
        else return -input;
    }  

    public static void Log(string message, Color color)
    {
        Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte) (color.r * 255f),
            (byte) (color.g * 255f), (byte) (color.b * 255f), message));
    }

    public static bool IsEditingInpputfield(this EventSystem sys)
    {
        GameObject current = sys.currentSelectedGameObject;
        if (!current) return false;
        TMP_InputField field = current.gameObject.GetComponent<TMP_InputField>();
        if (!field || !field.isFocused) return false;
        return true;
    }

    public static string ColorToHex(Color32 color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2") + color.a.ToString("X2");
        return hex;
    }

    public static Color HexToColor (string hex)
    {
        
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        byte a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, a);
    }

    public static string bytesToString(byte[] ba)
    {
        StringBuilder hex = new StringBuilder(ba.Length * 2);
        foreach (byte b in ba)
            hex.AppendFormat("{0:x2}", b);
        return hex.ToString();
    }

    /// <summary>
    /// returns the shortened string without curccency symbol
    /// </summary>
    public static string SatsToShortString(long sats)
    {
        return SatsToShortString(sats,false, Color.clear);
    }

    /// <summary>
    /// returns the shortened string with tinted symbol (if any)
    /// </summary>
    public static string SatsToShortString(long sats,bool includeSymbol)
    {
        return SatsToShortString(sats, includeSymbol, Color.clear);
    }

    /// <summary>
    /// returns the shortened string with a colored symbol
    /// </summary>
    public static string SatsToShortString(long sats, Color symbolColor)
    {
        return SatsToShortString(sats, true, symbolColor);
    }

    /// <param name="sats">satoshi amount</param>
    /// <param name="includeSymbol">should sats/btc sprite be included? (using TMP sprite Asset) </param>
    /// <param name="symbolColor">Color of the Symbol (if included) Color.Clear => tint</param>
    public static string SatsToShortString (long sats, bool includeSymbol, Color symbolColor)
    {
        string colorString;
        if (symbolColor == Color.clear) colorString = " tint=1";
        else colorString = " color=#" + ColorToHex(symbolColor);

        string valueString;
        string symbolCode = "sats";

        long nabs = Abs(sats);

        if (nabs >= 10000000000 && !includeSymbol)
        {
            float n = sats / 1000000000f;
            valueString = String.Format("{0:0G}", n);
        }

        if (nabs >= 1000000000)
        {
            if (includeSymbol)
            {
                float n = sats / 100000000f;
                valueString = String.Format("{0:0}", n);
            }
            else
            {
                float n = sats / 1000000000f;
                valueString = String.Format("{0:0.0G}", n);
            }
            
            symbolCode = "btc";
        }
        else if (nabs >= 100000000 && includeSymbol)
        {
            float n = sats / 100000000f;
            valueString = String.Format("{0:0.0}", n);
            symbolCode = "btc";
        }
        else if (nabs >= 10000000)
        {
            float n = sats / 1000000f;
            valueString = String.Format("{0:0M}", n);
        }
        else if (nabs >= 1000000)
        {
            float n = sats / 1000000f;
            valueString = String.Format("{0:0.0M}", n);
        }
        else if (nabs >= 10000)
        {
            float n = sats / 1000f;
            valueString = String.Format("{0:0K}", n);
        }
        else if (nabs >= 1000)
        {
            float n = sats / 1000f;
            valueString = String.Format("{0:0.0K}", n);
        }
        else valueString = sats.ToString();

        if(includeSymbol)valueString += "<sprite name=\""+symbolCode+"\"" + colorString + ">";

        return valueString;
    }

    public static string tintedSatsSymbol = "<sprite name=\"sats\" tint=1>";
    //*
    public static string steppedNumberString(long number)
    {
        string newString = number.ToString();
        int length = newString.Length;

        for(int i = 3; i< length; i += 3)
        {
            newString  = newString.Insert(length - i, " ");
        }
        return newString;
    }
    //*/
}
