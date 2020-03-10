using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct GameModePlayerSettings
{
    public bool TeleportPlayerOnStart;
    public bool ClearBountyOnEnd;
    public bool ClearStatsOnEnd;

    public double BountyTickConversion;
    public float BountyTickTime;
    public double BountyDropPercentageOnDeath;
}
