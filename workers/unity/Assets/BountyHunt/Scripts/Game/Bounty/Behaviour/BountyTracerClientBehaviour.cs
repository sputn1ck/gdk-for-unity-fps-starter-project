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
    private Vector3 NewPos;
    private Vector3 LastPos;
    private float startLerpTime;
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
        NewPos = transform.position;
        LastPos = transform.position;
    }

    private void SpatialPosition_OnUpdate(Position.Update obj)
    {
        if (!obj.Coords.HasValue)
            return;
        var unityVec = obj.Coords.Value.ToUnityVector() + linkedEntity.Worker.Origin;
        LastPos = NewPos;
        NewPos = unityVec;
        this.transform.position = unityVec;
        startLerpTime = Time.time;

        transform.position = Vector3.Lerp(LastPos, NewPos, 0);
    }

    private void Update()
    {
        float elapsedTime = Mathf.Clamp(Time.time - startLerpTime,0,5);
        transform.position = Vector3.Lerp(LastPos, NewPos, elapsedTime / 5f);
    }

    private void OnDisable()
    {
        ClientGameObjectManager.Instance.RemoveBountyTracerGO(attachedHunter);
    }
}
