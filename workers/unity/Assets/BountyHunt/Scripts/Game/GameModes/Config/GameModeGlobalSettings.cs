using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct GameModeGlobalSettings
{
    public long SecondDuration;
    public bool ClearPickups;
    public long NanoSeconds
    {
        get
        {
            return SecondDuration * 10000000;
        }
    }

}
