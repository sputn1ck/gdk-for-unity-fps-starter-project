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
    [HideInInspector] public KillEvent onAnyKill = new KillEvent();
    [HideInInspector] public RoundUpdateEvent onRoundUpdate = new RoundUpdateEvent();

    [HideInInspector] public LongEvent onGlobalBountyUpdate = new LongEvent();
    [HideInInspector] public LongEvent onGlobalLootUpdate = new LongEvent();
    [HideInInspector] public LongEvent onGlobalPotUpdate = new LongEvent();


    [HideInInspector] public LongEvent onBountyInPlayersUpdate = new LongEvent();
    [HideInInspector] public LongEvent onBountyinCubesUpdate = new LongEvent();
    [HideInInspector] public LongEvent onCarryoverSatsUpdate = new LongEvent();
    [HideInInspector] public LongEvent onRemainingPotUpdate = new LongEvent();

    [HideInInspector] public IntEvent onPlayerLifeTimeKillsUpdate = new IntEvent();

    [HideInInspector] public IntEvent onPlayerLifeTimeDeathsUpdate = new IntEvent();
    [HideInInspector] public LongEvent onPlayerLifeTimeEarningsUpdate = new LongEvent();

    [HideInInspector] public IntEvent onAllTimeKillsUpdate = new IntEvent();
    [HideInInspector] public IntEvent onAllTimeDeathsUpdate = new IntEvent();
    [HideInInspector] public LongEvent onAllTimeEarningsUpdate = new LongEvent();
    [HideInInspector] public KillsAndDeathsUpdateEvent onPlayerKillsAndDeathsUpdate = new KillsAndDeathsUpdateEvent();

    [HideInInspector] public AllTimeScoreUpdateEvent onAllTimeMostKillsUpdate = new AllTimeScoreUpdateEvent();
    [HideInInspector] public AllTimeScoreUpdateEvent onAllTimeMostDeathsUpdate = new AllTimeScoreUpdateEvent();
    [HideInInspector] public AllTimeScoreUpdateEvent onAllTimeMostEarningsUpdate = new AllTimeScoreUpdateEvent();

    [HideInInspector] public UpdateAdvertisersEvent onUpdateAdvertisers = new UpdateAdvertisersEvent();

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

