using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightningAuction.Delivery;
using Grpc.Core;

using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using System.Threading;
using System.Threading.Tasks;

public class AuctionBehaviour : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AuctionEnumerator());
    }

    IEnumerator AuctionEnumerator()
    {
        while (!ServerServiceConnections.ct.IsCancellationRequested)
        {
            if (FlagManager.instance.GetShouldRunAuction())
            {
                var duration = FlagManager.instance.GetAuctionDuration();
                StartAuction(duration);
                yield return new WaitForSeconds(duration + 15f);
            }
            yield return null;
        }
        yield return null;

    }

    private async void StartAuction(int duration)
    {
        var res = await ServerServiceConnections.instance.AuctionController.StartAuction(duration);
        if(res != null)
        {
            GetComponent<ServerGameChat>().SendAuctionStartedChatMessage(res.Auction.WinningEntry.Description);
        }

        Debug.Log("auction started " + res.Auction.Id);
    }

}
