using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BountyPlayerNonAuthorative : MonoBehaviour
{
    [Require] EntityId entityId;
    private void OnEnable()
    {
        ClientGameObjectManager.Instance.AddPlayerGO(entityId, this.gameObject);
    }

    private void OnDisable()
    {
        ClientGameObjectManager.Instance.RemovePlayerGO(entityId);
    }
}
