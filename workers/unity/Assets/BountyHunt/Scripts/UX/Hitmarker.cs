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
        ClientEvents.instance.onOpponentHit.AddListener(OnHit);
    }

    private void OnHit(bool headshot)
    {
        AudioManager.instance.spawn2DSound(hitmarkerSound, !headshot ? 1.0f : 1.5f);

    }
}
