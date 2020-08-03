using QRCoder;
using QRCoder.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Bountyhunt;

public static class Utility
{
    public const ulong BountyInt = 989810498;
    public const ulong MemoInt = 9898104109;

    public const string AuthMessage = "BBH_DONT_SIGN";


    //static Colors for variation
    public static Color32 failureColor = new Color32(255, 120, 105, 255);
    public const string failureColorHex = "FF7869FF";

    public static Color32 successColor = new Color32(147, 245, 153, 255);
    public const string successColorHex = "93F59AFF";


    public static void resetLocalTransform(this Transform t)
    {
        t.localPosition = Vector3.zero;
        t.localScale = Vector3.one;
        t.localRotation = UnityEngine.Quaternion.identity;
    }

    public static Vector3 convert(this Bountyhunt.Vector3Float v3)
    {
        return new Vector3(v3.X, v3.Y, v3.Z);
    }

    public static Vector3 convert(this Bountyhunt.Vector3Int v3)
    {
        return new Vector3(v3.X, v3.Y, v3.Z);
    }

    public static Bountyhunt.Vector3Float ToBbhVector(this Vector3 v3)
    {
        return new Bountyhunt.Vector3Float {X = v3.x, Y = v3.y, Z = v3.z};
    }

    public static Bountyhunt.Vector3Int ToBbhVector(this UnityEngine.Vector3Int v3)
    {
        return new Bountyhunt.Vector3Int { X = v3.x, Y = v3.y, Z = v3.z };
    }

    public static long SecondsToNano(long seconds)
    {
        return seconds * 10000000;
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
    public static void CopyToClipboard(string textToCopy)
    {
        TextEditor editor = new TextEditor
        {
            text = textToCopy
        };
        editor.SelectAll();
        editor.Copy();
    }

    public static Sprite GetInvertedQRCode(string text)
    {
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.M);
        UnityQRCode qrCode = new UnityQRCode(qrCodeData);
        Texture2D tex = qrCode.GetGraphic(8);
        /*
        Color[] pixels = tex.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].r > 0.5f)
            {
                pixels[i] = Color.clear;
            }
            else
            {
                pixels[i] = Color.white;
            }
        }
        tex.SetPixels(pixels);
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        */
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100);
        return sprite;
    }

    static System.Random rnd = new System.Random();

    public static string GetUniqueString()
    {
        return rnd.Next().ToString();
    }

    public static Vector3 Vector3FloatToVector3(Vector3Float vector)
    {
        return new Vector3(vector.X, vector.Y, vector.Z);
    }
    public static Vector3Float Vector3ToVector3Float(Vector3 vector)
    {
        return new Vector3Float(vector.x, vector.y, vector.z);
    }

    public static Bountyhunt.Quaternion QuatToBhQuat(UnityEngine.Quaternion q)
    {
        return new Bountyhunt.Quaternion(q.w, q.x, q.y, q.z);
    }

    public static UnityEngine.Quaternion BhQuatToQuat(Bountyhunt.Quaternion q)
    {
        return new UnityEngine.Quaternion(q.X, q.Y, q.Z, q.W);
    }

    public static float MapValue(float x, float in_min, float in_max, float out_min, float out_max)
    {
        
        return(x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }
}
