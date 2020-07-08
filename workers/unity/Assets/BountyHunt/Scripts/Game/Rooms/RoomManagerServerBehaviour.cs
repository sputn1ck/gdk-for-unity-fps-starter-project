using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Commands;


public class RoomManagerServerBehaviour : MonoBehaviour
{
    [Require] RoomManagerWriter RoomManagerWriter;
    [Require] RoomManagerCommandReceiver RoomManagerCommandReceiver;


    LinkedEntityComponent LinkedEntityComponent;
    private void OnEnable()
    {

        LinkedEntityComponent = GetComponent<LinkedEntityComponent>();
        RoomManagerCommandReceiver.OnStartRoomRequestReceived += OnStart;
        Initialize();
    }

    private void OnStart(RoomManager.StartRoom.ReceivedRequest obj)
    {
        Debug.LogFormat("{0} has started", RoomManagerWriter.Data.RoomInfo.RoomId);
    }

    private void Initialize()
    {
        var mapInfo = MapDictStorage.Instance.GetMap(RoomManagerWriter.Data.RoomInfo.MapInfo.MapId);
       
        mapInfo.Initialize(this, true, this.transform.position, RoomManagerWriter.Data.RoomInfo.MapInfo.MapData);
    }
}
