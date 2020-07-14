using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Improbable.Gdk.Core;

public class RoomBoundObjectBehaviour : MonoBehaviour
{
    [Require] RoomBoundObjectReader RoomBoundObjectReader;
    [Require] RoomManagerCommandSender RoomManagerCommandSender;
    [Require] EntityId EntityId;
    private void OnEnable()
    {
        AddToRoom();
    }

    private void AddToRoom()
    {
        var id = RoomBoundObjectReader.Data.RoomEntityId;
        RoomManagerCommandSender.SendAddRoomboundObjectCommand(id, new AddRoomBountyObjectRequest(EntityId));
    }
}
