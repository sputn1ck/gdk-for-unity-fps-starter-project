using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Commands;

public class RoomManagerClientBehaviour : MonoBehaviour
{
    [Require] RoomManagerReader RoomManagerReader;
    [Require] RoomManagerCommandSender RoomManagerCommandSender;

    LinkedEntityComponent LinkedEntityComponent;

    private GameObject mapGo;
    private void OnEnable()
    {

        LinkedEntityComponent = GetComponent<LinkedEntityComponent>();
        Initialize();
    }

    private void OnDisable()
    {
        Destroy(mapGo);
    }

    private void Initialize()
    {
        var mapInfo = MapDictStorage.Instance.GetMap(RoomManagerReader.Data.RoomInfo.MapInfo.MapId);

        mapGo = mapInfo.Initialize(this, false, this.transform.position, RoomManagerReader.Data.RoomInfo.MapInfo.MapData);

        // Ready to join
        RoomManagerCommandSender.SendReadyToJoinCommand(LinkedEntityComponent.EntityId, new ReadyToJoinRequest(RoomPlayerClientBehaviour.Instance.EntityId));
    }

}
