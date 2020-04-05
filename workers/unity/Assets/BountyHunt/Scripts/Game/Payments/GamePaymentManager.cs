using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePaymentManager : MonoBehaviour
{
    private ServerServiceConnections lnServer;

    private bool useLnd;

    private void Awake()
    {
        lnServer = GetComponent<ServerServiceConnections>();
        if (lnServer == null)
        {
            useLnd = false;
            return;
        }

        useLnd = true;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        //lnServer.lnd.AddCallback(InvoiceSettled);
        ServerEvents.instance.OnInvoicePaid.AddListener(InvoiceSettled);
    }

    void InvoiceSettled(InvoiceSettledEventArgs e)
    {
        PrometheusManager.TotalSatsAdded.Inc(e.Invoice.AmtPaidSat);
        if (e.Invoice.IsKeysend)
        {
            foreach (var htlc in e.Invoice.Htlcs)
            {
                if (htlc.CustomRecords.ContainsKey(Utility.BountyInt))
                {
                    Google.Protobuf.ByteString memoByte;
                    var memoString = "";
                    if (htlc.CustomRecords.TryGetValue(Utility.MemoInt, out memoByte))
                    {
                        memoString = memoByte.ToStringUtf8();
                    }

                    var playerpub = Utility.bytesToString(htlc.CustomRecords[Utility.BountyInt].ToByteArray());
                    ServerEvents.instance.OnBountyInvoicePaid.Invoke(new BountyInvoice
                    {
                        amount = e.Invoice.AmtPaidSat,
                        pubkey = playerpub,
                        message = memoString
                    });
                    PrometheusManager.TotalBountyPaidAmount.Inc();
                    PrometheusManager.TotalBountyPaidSats.Inc(e.Invoice.AmtPaidSat);
                }
            }

            return;
        }

        var auction = DonnerUtils.MemoToAuctionInvoice(e.Invoice.Memo);
        if (auction != null && auction.AuctionId != null)
        {
            ServerEvents.instance.OnAuctionInvoicePaid.Invoke(auction);

            PrometheusManager.TotalAuctionsPaidAmount.Inc();
            PrometheusManager.TotalAuctionsPaidSats.Inc(e.Invoice.AmtPaidSat);
            return;
        }

        var bounty = DonnerUtils.MemoToBountyInvoice(e.Invoice.Memo);
        if (bounty != null && bounty.pubkey != "")
        {
            ServerEvents.instance.OnBountyInvoicePaid.Invoke(bounty);

            PrometheusManager.TotalBountyPaidAmount.Inc();
            PrometheusManager.TotalBountyPaidSats.Inc(e.Invoice.AmtPaidSat);
            return;
        }
    }



}
