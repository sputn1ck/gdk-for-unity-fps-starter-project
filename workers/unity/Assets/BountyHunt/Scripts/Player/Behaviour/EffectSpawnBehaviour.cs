using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;

public class EffectSpawnBehaviour : MonoBehaviour
{
    [Require] EffectSpawnerComponentCommandReceiver effectSpawnerComponentCommandReceiver;

    public List<Effect> effects;
    Dictionary<string,Effect> effectsDict;

    private void Awake()
    {
        foreach(Effect e in effects)
        {
            effectsDict[e.key] = e;
        }
    }

    private void OnEnable()
    {
        effectSpawnerComponentCommandReceiver.OnSpawnEffectRequestReceived += OnSpawnEffect;
    }

    void OnSpawnEffect(EffectSpawnerComponent.SpawnEffect.ReceivedRequest obj)
    {
        Utility.Log("spawning Effect!", Color.cyan);

        EffectInfo info = obj.Payload;

        if (!effectsDict.ContainsKey(info.Key))return;

        Transform parent;

        switch (info.Parent)
        {
            case EffectParent.ENTITY:
                parent = transform;
                break;
            default:
                parent = null;
                break;
        }

        Instantiate(effectsDict[info.Key],parent,!info.PositionIsLocal);

    }

}
