using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
public class ServerRoomStatsBehaviour : MonoBehaviour
{
    [Require] RoomStatsWriter RoomStatsWriter;
    [Require] RoomStatsCommandReceiver RoomStatsCommandReceiver;

    RoomManagerServerBehaviour rm;

    void Awake()
    {
        rm = GetComponent<RoomManagerServerBehaviour>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
