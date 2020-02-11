using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClientEvents : MonoBehaviour
{
    public static ClientEvents instance;

    [HideInInspector] public UnityEvent onMapLoaded = new UnityEvent();
    [HideInInspector] public UnityEvent onGameJoined = new UnityEvent();
    [HideInInspector] public LongEvent onUpdateCanRecieveBalance = new LongEvent();
    [HideInInspector] public LongEvent onUpdateCurrentBalance = new LongEvent();
    [HideInInspector] public StringEvent onUpdateSyncedState = new StringEvent();
    [HideInInspector] public StringEvent onUpdateHasChannelState = new StringEvent();
    [HideInInspector] public UnityEvent onNewAuctionStarted = new UnityEvent();
    [HideInInspector] public GameObjectEvent onPlayerSpawn = new GameObjectEvent();
    [HideInInspector] public UnityEvent onPlayerDie = new UnityEvent();

    [HideInInspector] public UnityEvent onServicesSetup = new UnityEvent();

    [HideInInspector] public BountyUpdateEvent onBountyUpdate = new BountyUpdateEvent();
    [HideInInspector] public SessionEarningsUpdateEvent onSessionEarningsUpdate = new SessionEarningsUpdateEvent();
    [HideInInspector] public BalanceUpdateEvent onBalanceUpdate = new BalanceUpdateEvent();
    [HideInInspector] public ScoreboardUIItemListEvent onScoreboardUpdate = new ScoreboardUIItemListEvent();

    [HideInInspector] public ChatMessageEvent onChatMessageRecieve = new ChatMessageEvent();
    [HideInInspector] public StringColorEvent onAnnouncement = new StringColorEvent();
    [HideInInspector] public StringLongEvent onDonationMessageUpdate = new StringLongEvent();
    [HideInInspector] public UnityEvent onOpponentHit = new UnityEvent();

    [HideInInspector] public PaymentSuccessEvent onPaymentSucces = new PaymentSuccessEvent();
    [HideInInspector] public PaymentFailureEvent onPaymentFailure = new PaymentFailureEvent();


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);

        
    }

    public void MapLoaded()
    {
        onMapLoaded.Invoke();
    }


    


}

