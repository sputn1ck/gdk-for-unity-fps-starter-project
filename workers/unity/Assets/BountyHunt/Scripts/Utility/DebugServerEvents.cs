using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugServerEvents : MonoBehaviour
{

    public AuctionInvoice auctionInvoice;
    public bool sendAuction;
    public RandomInvoice randomInvoice;
    public bool sendRandom;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (sendRandom)
        {
            sendRandom = false;
            ServerEvents.instance.OnRandomInvoicePaid.Invoke(randomInvoice);
        }
        if (sendAuction)
        {
            sendAuction = false;
            ServerEvents.instance.OnAuctionInvoicePaid.Invoke(auctionInvoice);
        }
    }
}
