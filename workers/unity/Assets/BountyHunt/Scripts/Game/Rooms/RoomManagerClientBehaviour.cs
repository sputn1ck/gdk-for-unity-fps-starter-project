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
    [Require] RoomStatsReader RoomStatsReader;
    [Require] RoomManagerCommandSender RoomManagerCommandSender;
    [Require] RoomStatsCommandSender RoomStatsCommandSender;
    [Require] EntityId EntityId;


    private Map map;
    private void OnEnable()
    {
        Debug.Log("enabling room " + RoomManagerReader.Data.RoomInfo.RoomId);
        RoomManagerReader.OnRoomStateUpdate += RoomManagerReader_OnRoomStateUpdate;
        ClientGameObjectManager.Instance.AddRoomGo(EntityId, this.gameObject);
    }

    private void RoomManagerReader_OnRoomStateUpdate(RoomState obj)
    {

    }

   
    private void OnDisable()
    {
        
        Deinitialize();
    }

    public void Initialize()
    {
        Debug.Log("Iniailized called on " + transform.name);

        map = Instantiate(MapDictStorage.Instance.GetMap(RoomManagerReader.Data.RoomInfo.MapInfo.MapId));


        // start initializing map
        BBHUIManager.instance.mainMenu.BlendImage(true);
        map.Initialize(this, false, this.transform.position, RoomManagerReader.Data.RoomInfo.MapInfo.MapData, () => {
                RoomManagerCommandSender.SendReadyToJoinCommand(EntityId, new ReadyToJoinRequest(RoomPlayerClientBehaviour.Instance.EntityId), (cb2) => {
                    if (cb2.StatusCode != Improbable.Worker.CInterop.StatusCode.Success)
                    {
                        BBHUIManager.instance.mainMenu.BlendImage(false);
                        Debug.LogError(cb2.Message);
                        return;
                    }
                    BBHUIManager.instance.mainMenu.BlendImage(false);

            });
        });
        
    }

    public void Deinitialize()
    {

        Debug.Log("Deinitialize called on " + transform.name);
        ClientGameObjectManager.Instance.RemoveRoomGo(EntityId);
        RoomStatsReader.RemoveAllCallbacks();
        if (map == null)
        {
            return;
        }
        map.Remove();
        map = null;
    }

}
