using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public static class DonnerUtils 
{
    public static AuctionInvoice MemoToAuctionInvoice(string memo)
    {
        try
        {

            var res = JsonConvert.DeserializeObject<AuctionInvoice>(memo);
            return res;
        }
        catch(Exception e)
        {
            return null;
        }
    }
    public static TeleportInvoice MemoToTeleportInvoice(string memo)
    {
        try
        {

            var res = JsonConvert.DeserializeObject<TeleportInvoice>(memo);
            return res;
        }
        catch (Exception e)
        {
            return null;
        }
    }
    public static BountyInvoice MemoToBountyInvoice(string memo)
    {
        try
        {

            var res = JsonConvert.DeserializeObject<BountyInvoice>(memo);
            return res;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public static string SatToString(long sats)
    {
        var res = "";
        if (sats > 1000000)
            res = Math.Floor(sats / 1000000d).ToString() + "M";
        else if (sats > 1000)
            res = Math.Floor(sats / 1000d).ToString() + "K";
        else
            res = sats.ToString();
        return res;
    }
    public static string NumberToString(long number)
    {
        if (number < 10)
        {
            return string.Format("{0:0.##}", number);
        }
        if (number < 100)
        {
            return string.Format("{0:0.#}", number);
        }
        if (number < 1000)
        {
            return string.Format("{0:0}", number);
        }
        if (number < 10000)
        {
            return string.Format("{0:0.#}", (number / 1000d)) + " K";
        };
        if (number < 1000000)
        {
            return (number / 1000d) + " K";
        };

        return string.Format("{0:0.#}", (number / 1000000d)) + " M";

    }
    public static long CalculateTeleportCost(Vector3 posA, Vector3 posB)
    {
        var distance = Vector3.Distance(posA, posB);
        if (distance < 1)
            distance = 1;
        return (long)distance;
    }
    public static int DateTimeToUnix(DateTime dateTime)
    {
        return (Int32)(dateTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    }

    public static DateTime UnixTimeToDateTime(Int32 unixTime)
    {
        return new DateTime(1970, 1, 1).AddSeconds(unixTime);
    }
    public static byte[] StringToByteArrayFastest(string hex)
    {
        if (hex.Length % 2 == 1)
            throw new Exception("The binary key cannot have an odd number of digits");

        byte[] arr = new byte[hex.Length >> 1];

        for (int i = 0; i < hex.Length >> 1; ++i)
        {
            arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
        }

        return arr;
    }

    private static int GetHexVal(char hex)
    {
        int val = (int)hex;
        //For uppercase A-F letters:
        return val - (val < 58 ? 48 : 55);
        //For lowercase a-f letters:
        //return val - (val < 58 ? 48 : 87);
        //Or the two combined, but a bit slower:
        //return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
    }
}

[Serializable]
public class AuctionInvoice
{
    public string AuctionId;
    public long Amount;
    public string WinningMessage;
    public string AuctionEntryId;
}
[Serializable]
public class TeleportInvoice
{
    public long entityId;
    public RoughPosition position;
    public long cost;
    public long expiry;
    public string description;
}
//TODO: REMOVE
public struct RoughPosition
{

}
[Serializable]
public class BountyInvoice
{
    public string pubkey;
    public string message;
    public long amount;
}

[Serializable]
public class RandomInvoice
{
    public string message;
    public long amount;
}
