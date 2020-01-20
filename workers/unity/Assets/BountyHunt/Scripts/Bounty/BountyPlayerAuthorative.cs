using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using System.Threading;

public class BountyPlayerAuthorative : MonoBehaviour
{

    [Require] BountyComponentReader BountyComponentReader;

    private long lastBounty = 0;
    // Start is called before the first frame update
    void OnEnable()
    {
        BountyComponentReader.OnUpdate += BountyComponentReader_OnUpdate;
        lastBounty = 0;
    }

    private void BountyComponentReader_OnUpdate(BountyComponent.Update obj)
    {
        if (obj.Bounty.HasValue)
        {
            ClientEvents.instance.onBountyUpdate.Invoke(obj.Bounty.Value,lastBounty,BountyReason.PICKUP);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
