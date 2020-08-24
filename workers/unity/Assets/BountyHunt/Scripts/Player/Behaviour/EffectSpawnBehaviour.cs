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

    [HideInInspector]public LinkedEntityComponent LinkedEntityComponent;


    private void Awake()
    {
        effectsDict = new Dictionary<string, Effect>();
        foreach(Effect e in effects)
        {
            effectsDict[e.key] = e;
        }
    }

    private void OnEnable()
    {
        effectSpawnerComponentCommandReceiver.OnSpawnEffectRequestReceived += OnSpawnEffect;
        LinkedEntityComponent = GetComponent<LinkedEntityComponent>();
    }

    void OnSpawnEffect(EffectSpawnerComponent.SpawnEffect.ReceivedRequest obj)
    {
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

        var effect = Instantiate(effectsDict[info.Key],parent);
        if (info.PositionIsLocal)
        {
            effect.transform.localPosition = info.Position.convert() + LinkedEntityComponent.Worker.Origin;
            effect.transform.localRotation = UnityEngine.Quaternion.Euler(info.RotationEuler.convert());
        }
        else
        {
            effect.transform.position = info.Position.convert() + LinkedEntityComponent.Worker.Origin;
            effect.transform.rotation = UnityEngine.Quaternion.Euler(info.RotationEuler.convert());
        }

        if(effect.soundEffect != null)
        {
            AudioManager.instance.spawnSound(effect.soundEffect, effect.transform);
        }

    }

}
