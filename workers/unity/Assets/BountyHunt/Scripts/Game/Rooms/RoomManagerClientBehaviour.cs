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


    LinkedEntityComponent LinkedEntityComponent;
    private void OnEnable()
    {

        LinkedEntityComponent = GetComponent<LinkedEntityComponent>();
        Initialize();
    }


    private void Initialize()
    {
        var mapInfo = MapDictStorage.Instance.GetMap(RoomManagerReader.Data.RoomInfo.MapInfo.MapId);

        mapInfo.Initialize(this, false, this.transform.position, RoomManagerReader.Data.RoomInfo.MapInfo.MapData);

        // Ready to join
    }

}
