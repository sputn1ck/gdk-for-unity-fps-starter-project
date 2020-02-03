using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fps.Guns;
using System;

public class Hitmarker : MonoBehaviour
{
    public AudioClip hitmarkerSound;
    void Start()
    {
        ClientShooting.instance.OnPlayerHit.AddListener(ClientEvents.instance.onOpponentHit.Invoke);
        ClientEvents.instance.onOpponentHit.AddListener(OnHit);
    }

    private void OnHit()
    {
        AudioManager.instance.spawn2DSound(hitmarkerSound);

    }
}
