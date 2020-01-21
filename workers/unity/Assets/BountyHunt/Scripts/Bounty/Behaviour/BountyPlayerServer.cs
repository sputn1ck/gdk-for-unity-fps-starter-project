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

    [Require] public HunterComponentWriter HunterComponentWriter;
    [Require] public HunterComponentCommandReceiver HunterComponentCommandReceiver;


    private LinkedEntityComponent LinkedEntityComponent;

    private CancellationTokenSource ct;
    // Start is called before the first frame update
    void OnEnable()
    {
        LinkedEntityComponent = GetComponent<LinkedEntityComponent>();
        HunterComponentCommandReceiver.OnAddBountyRequestReceived += BountyComponentCommandReceiver_OnAddBountyRequestReceived;
        ct = new CancellationTokenSource();

        //StartCoroutine(BountyTick());
    }

    private void BountyComponentCommandReceiver_OnAddBountyRequestReceived(HunterComponent.AddBounty.ReceivedRequest obj)
    {
        if (obj.CallerAttributeSet[0] != WorkerUtils.UnityGameLogic)
            return;
        HunterComponentWriter.SendUpdate(new HunterComponent.Update { Bounty = HunterComponentWriter.Data.Bounty + obj.Payload.Amount });
    }

}
