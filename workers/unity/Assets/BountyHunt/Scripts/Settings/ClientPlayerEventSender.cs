using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fps;
using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.Core;
using System;

public class ClientPlayerEventSender : MonoBehaviour
{
    [Require] private HealthComponentReader health;
    void Start()
    {
        health.OnHealthModifiedEvent += OnUpdateHealth;
        health.OnRespawnEvent += OnRespawn;
    }

    private void OnRespawn(Empty obj)
    {
        ClientEvents.instance.onPlayerSpawn.Invoke(gameObject);
        Debug.Log("player spawned");

    }

    private void OnUpdateHealth(HealthModifiedInfo info)
    {
        if(info.Died)
        {
            ClientEvents.instance.onPlayerDie.Invoke();
            Debug.Log("player died");

        }
    }
    
    private void OnEnable()
    {
        ClientEvents.instance.onPlayerSpawn.Invoke(gameObject);
        Debug.Log("player spawned");

    }
    
}
