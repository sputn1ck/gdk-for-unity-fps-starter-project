using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Improbable;

public class BountyTracerClientBehaviour : MonoBehaviour
{
    [Require] EntityId id;
    [Require] TracerComponentReader tracerComponentReader;
    [Require] PositionReader spatialPosition;
    LinkedEntityComponent linkedEntity;

    private void OnEnable()
    {
        ClientGameObjectManager.Instance.AddBountyTracerGO(new EntityId(tracerComponentReader.Data.AttachedHunter), this.gameObject);
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
        ClientGameObjectManager.Instance.RemoveBountyTracerGO(new EntityId(tracerComponentReader.Data.AttachedHunter));
    }
}
