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
