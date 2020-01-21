using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using System.Threading;
using Fps.Config;

[WorkerType(WorkerUtils.UnityGameLogic)]
public class BountyPlayerServer : MonoBehaviour
{

    [Require] public BountyComponentWriter BountyComponentWriter;
    [Require] public BountyComponentCommandReceiver BountyComponentCommandReceiver;
    [Require] public HunterComponentWriter HunterComponentWriter;


    private LinkedEntityComponent LinkedEntityComponent;

    private CancellationTokenSource ct;
    // Start is called before the first frame update
    void OnEnable()
    {
        LinkedEntityComponent = GetComponent<LinkedEntityComponent>();
        BountyComponentCommandReceiver.OnAddBountyRequestReceived += BountyComponentCommandReceiver_OnAddBountyRequestReceived;
        ct = new CancellationTokenSource();

        //StartCoroutine(BountyTick());
    }

    private void BountyComponentCommandReceiver_OnAddBountyRequestReceived(BountyComponent.AddBounty.ReceivedRequest obj)
    {
        if (obj.CallerAttributeSet[0] != WorkerUtils.UnityGameLogic)
            return;
        BountyComponentWriter.SendUpdate(new BountyComponent.Update { Bounty = BountyComponentWriter.Data.Bounty + obj.Payload.Amount });
    }

}
