using Bbhrpc;
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
    public BackendKickEvent OnBackendKickEvent = new BackendKickEvent();
    public BackendChatEvent OnBackendChatEvent = new BackendChatEvent();

    private void Awake()
    {
        instance = this;
    }
}

public class AuctionInvoicePaidEvent : UnityEvent<AuctionInvoice> { };
public class BountyInvoicePaidEvent : UnityEvent<BountyInvoice> { };

public class RandomInvoicePaidEvent : UnityEvent<RandomInvoice> { };

public class InvoicePaidEvent : UnityEvent<InvoiceSettledEventArgs> { };
public class BackendKickEvent : UnityEvent<Bbhrpc.KickEvent> { };
public class BackendChatEvent : UnityEvent<ChatEvent> { };

