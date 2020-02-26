using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ServerEvents : MonoBehaviour
{

    public static ServerEvents instance;

    public InvoicePaidEvent OnInvoicePaid = new InvoicePaidEvent();
    public AuctionInvoicePaidEvent OnAuctionInvoicePaid = new AuctionInvoicePaidEvent();
    public BountyInvoicePaidEvent OnBountyInvoicePaid = new BountyInvoicePaidEvent();
    public RandomInvoicePaidEvent OnRandomInvoicePaid = new RandomInvoicePaidEvent();

    private void Awake()
    {
        instance = this;
    }
}

public class AuctionInvoicePaidEvent : UnityEvent<AuctionInvoice> { };
public class BountyInvoicePaidEvent : UnityEvent<BountyInvoice> { };

public class RandomInvoicePaidEvent : UnityEvent<string, long> { };

public class InvoicePaidEvent : UnityEvent<InvoiceSettledEventArgs> { };

