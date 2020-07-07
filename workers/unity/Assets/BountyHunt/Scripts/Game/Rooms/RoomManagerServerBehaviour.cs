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

    private void OnEnable()
    {
        RoomManagerCommandReceiver.OnStartRoomRequestReceived += OnStart;
    }

    private void OnStart(RoomManager.StartRoom.ReceivedRequest obj)
    {
        Debug.LogFormat("{0} has started", RoomManagerWriter.Data.RoomInfo.RoomId);
    }
}
