using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Improbable;
using System;

public class BountyTracerClientBehaviour : MonoBehaviour
{
    [Require] EntityId id;
    [Require] TracerComponentReader tracerComponentReader;
    [Require] PositionReader spatialPosition;
    LinkedEntityComponent linkedEntity;
    EntityId attachedHunter;
    private void OnEnable()
    {
        try
        {
            attachedHunter = new EntityId(tracerComponentReader.Data.AttachedHunter);
        } catch(Exception e)
        {
            Destroy(this.gameObject);
            Debug.Log("cube excepction");
        }
        
        ClientGameObjectManager.Instance.AddBountyTracerGO(attachedHunter, this.gameObject);
        linkedEntity = GetComponent<LinkedEntityComponent>();
        spatialPosition.OnUpdate += SpatialPosition_OnUpdate;
    }

    private void SpatialPosition_OnUpdate(Position.Update obj)
    {
        if (!obj.Coords.HasValue)
            return;
        var unityVec = obj.Coords.Value.ToUnityVector() + linkedEntity.Worker.Origin;
        this.transform.position = unityVec;
    }

    private void OnDisable()
    {
        ClientGameObjectManager.Instance.RemoveBountyTracerGO(attachedHunter);
    }
}
