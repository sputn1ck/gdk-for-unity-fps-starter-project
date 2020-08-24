using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingTarget : MapClientOnlyBehaviour, IShootable
{
    public bool criticalHit;

    public OnHitResponse OnHit()
    {
        return new OnHitResponse()
        {
            headshot = criticalHit
        };
    }
}

public interface IShootable
{
    OnHitResponse OnHit();
}

public struct OnHitResponse
{
    public bool headshot;
}
