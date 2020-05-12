using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightningAuction.Delivery;
using Grpc.Core;

using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using System.Threading;
using System.Threading.Tasks;
using Bountyhunt;
public class ServerPaymentManager : MonoBehaviour
{
    [Require] PaymentManagerComponentWriter PaymentManagerComponentWriter;


    private void OnEnable()
    {
        ServerEvents.instance.OnAuctionInvoicePaid.AddListener(NewBid);
    }
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(AuctionEnumerator());
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
            GetComponent<ServerGameChat>().SendAuctionStartedChatMessage("new auction started");
        }

        Debug.Log("auction started " + res.Auction.Id);
    }

    private void NewBid(AuctionInvoice invoice)
    {
        PaymentManagerComponentWriter.SendUpdate(new PaymentManagerComponent.Update { AuctionWinner = new Option<NewWinnerMessage>(new NewWinnerMessage(invoice.WinningMessage, invoice.Amount)) });
            
        
    }
    private void OnDisable()
    {

        ServerEvents.instance.OnAuctionInvoicePaid.RemoveListener(NewBid);
    }
}
