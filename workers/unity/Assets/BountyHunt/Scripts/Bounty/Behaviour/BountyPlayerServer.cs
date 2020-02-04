using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using System.Threading;
using Fps.Config;
using Improbable.Gdk.Core;
[WorkerType(WorkerUtils.UnityGameLogic)]
public class BountyPlayerServer : MonoBehaviour
{

    [Require] public HunterComponentWriter HunterComponentWriter;
    [Require] public HunterComponentCommandReceiver BountyComponentCommandReceiver;
    [Require] public GameStatsCommandSender GameStatsCommandSender;


    private LinkedEntityComponent LinkedEntityComponent;

    private CancellationTokenSource ct;
    // Start is called before the first frame update
    void OnEnable()
    {
        LinkedEntityComponent = GetComponent<LinkedEntityComponent>();
        BountyComponentCommandReceiver.OnAddBountyRequestReceived += BountyComponentCommandReceiver_OnAddBountyRequestReceived;
        ct = new CancellationTokenSource();

        Invoke("SetName", 1f);
        //StartCoroutine(BountyTick());
    }

    //TODO hacky...
    async void SetName()
    {
        var bbhbackend = FlagManager.instance.GetComponent<BackendPlayerBehaviour>();
        var name = await bbhbackend.client.GetUsername(HunterComponentWriter.Data.Pubkey);
        BackendGameServerBehaviour.instance.AddPlayerHeartbeat(HunterComponentWriter.Data.Pubkey, Bbh.PlayerInfoEvent.Types.EventType.Heartbeat);
        HunterComponentWriter.SendUpdate(new HunterComponent.Update { Name = name });
        GameStatsCommandSender.SendSetNameCommand(new EntityId(2), new SetNameRequest(name, LinkedEntityComponent.EntityId, HunterComponentWriter.Data.Pubkey));
    }
    private void BountyComponentCommandReceiver_OnAddBountyRequestReceived(HunterComponent.AddBounty.ReceivedRequest obj)
    {
        if (obj.CallerAttributeSet[0] != WorkerUtils.UnityGameLogic)
            return;
        HunterComponentWriter.SendUpdate(new HunterComponent.Update { Bounty = HunterComponentWriter.Data.Bounty + obj.Payload.Amount });
    }

}
