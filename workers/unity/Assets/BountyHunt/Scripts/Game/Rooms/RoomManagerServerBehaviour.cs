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


    LinkedEntityComponent LinkedEntityComponent;
    private void OnEnable()
    {

        LinkedEntityComponent = GetComponent<LinkedEntityComponent>();
        Initialize();
    }


    private void Initialize()
    {
        var mapInfo = MapDictStorage.Instance.GetMap(RoomManagerWriter.Data.RoomInfo.MapInfo.MapId);
       
        mapInfo.Initialize(this, true, this.transform.position, RoomManagerWriter.Data.RoomInfo.MapInfo.MapData);
    }
}
