using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
public class TransformSyncBehaviour : MonoBehaviour
{
    [Require] TransformSyncReader TransformSyncReader;

    private void OnEnable()
    {
        SetTransform();
    }

    public void SetTransform()
    {
        var t = TransformSyncReader.Data.Transform;

        transform.localScale = Utility.Vector3FloatToVector3(t.Scale);
        transform.rotation = Utility.BhQuatToQuat(t.Rotation);
    }
}
