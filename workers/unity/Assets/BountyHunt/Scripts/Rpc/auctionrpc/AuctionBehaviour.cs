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

    CancellationTokenSource cancellationToken;
    private void Awake()
    {
        cancellationToken = new CancellationTokenSource();
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AuctionEnumerator());
    }

    IEnumerator AuctionEnumerator()
    {
        while (!cancellationToken.IsCancellationRequested)
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
        Debug.Log("auction started " + res.Auction.Id);
    }

    private void OnApplicationQuit()
    {
        cancellationToken.Cancel();
    }
}
