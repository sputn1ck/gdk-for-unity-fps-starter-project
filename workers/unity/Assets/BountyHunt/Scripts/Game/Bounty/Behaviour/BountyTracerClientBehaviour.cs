using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
public class BountyTracerClientBehaviour : MonoBehaviour
{
    [Require] EntityId id;
    [Require] TracerComponentReader tracerComponentReader;

    private void OnEnable()
    {
        ClientGameObjectManager.Instance.AddBountyTracerGO(new EntityId(tracerComponentReader.Data.AttachedHunter), this.gameObject);
    }

    private void OnDisable()
    {
        ClientGameObjectManager.Instance.RemoveBountyTracerGO(new EntityId(tracerComponentReader.Data.AttachedHunter));
    }
}
