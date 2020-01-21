using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using System.Threading;

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

        StartCoroutine(BountyTick());
    }

    private void BountyComponentCommandReceiver_OnAddBountyRequestReceived(BountyComponent.AddBounty.ReceivedRequest obj)
    {
        BountyComponentWriter.SendUpdate(new BountyComponent.Update { Bounty = BountyComponentWriter.Data.Bounty + obj.Payload.Amount });
    }
    //TODO UPDATE BOUNTYTICK WITH NEW STUFF
    IEnumerator BountyTick()
    {
        yield return new WaitForSeconds(5f);
        while (!ct.IsCancellationRequested)
        {
            var state = BountyComponentWriter.Data;
            if (state.Bounty == 0)
            {

            }
            else
            {
                var tick = calculateTick(state.Bounty, FlagManager.instance.defaultBountyPerTick);
                BountyComponentWriter.SendUpdate(new BountyComponent.Update { Bounty = state.Bounty - tick});
                HunterComponentWriter.SendUpdate(new HunterComponent.Update() { Earnings = HunterComponentWriter.Data.Earnings + tick });
                //PrometheusManager.TotalEarnings.Inc(tick);
            }
            yield return new WaitForSeconds(FlagManager.instance.defualtTimePerBountyTick);
        }
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private long calculateTick(long bounty, double percentage)
    {
        long sats = (long)System.Math.Round(bounty * percentage);
        return sats < 1 ? 1 : sats;
    }
}
