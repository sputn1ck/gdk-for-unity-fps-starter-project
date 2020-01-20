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
    public BountyUpdateEvent onBountyUpdate;

    public bool testTrigger;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);

        
    }

    public void MapLoaded()
    {
        onMapLoaded.Invoke();
    }
    public void Update()
    {

        if (testTrigger)
        {
            testTrigger = false;
            onBountyUpdate.Invoke(1, 2, Bountyhunt.BountyReason.OTHER);
        }
    }

    public void TestEvent(long a, long b, Bountyhunt.BountyReason reason)
    {
        Debug.Log(a + ";" + b + ";" + reason);
    }



}

[Serializable]
public class BountyUpdateEvent : UnityEvent<long,long,Bountyhunt.BountyReason> { }
