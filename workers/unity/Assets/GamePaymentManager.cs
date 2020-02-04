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
        lnServer.lnd.AddCallback(InvoiceSettled);
    }

    void InvoiceSettled(object sender, InvoiceSettledEventArgs e)
    {
        var auction = DonnerUtils.MemoToAuctionInvoice(e.Invoice.Memo);
        if (auction != null && auction.AuctionId != null)
        {
            ServerEvents.instance.OnAuctionInvoicePaid.Invoke(auction);
            return;
        }
        var bounty = DonnerUtils.MemoToBountyInvoice(e.Invoice.Memo);
        if (bounty != null && bounty.pubkey != "")
        {
            ServerEvents.instance.OnBountyInvoicePaid.Invoke(bounty);
            return;
        }

        ServerEvents.instance.OnRandomInvoicePaid.Invoke(e.Invoice.Memo, e.Invoice.AmtPaidSat);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
