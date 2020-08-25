using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Commands;
using UnityEngine.Events;

public class RoomManagerClientBehaviour : MonoBehaviour
{
    [Require] RoomManagerReader RoomManagerReader;
    [Require] RoomStatsManagerReader RoomStatsReader;
    [Require] RoomManagerCommandSender RoomManagerCommandSender;
    [Require] RoomStatsManagerCommandSender RoomStatsCommandSender;
    [Require] EntityId EntityId;


    public Map map;
    public UnityAction OnMapLoaded;
    private void OnEnable()
    {
        Debug.Log("enabling room " + RoomManagerReader.Data.RoomInfo.Info.RoomId);
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

        map = Instantiate(MapDictStorage.Instance.GetMap(RoomManagerReader.Data.RoomInfo.Info.MapInfo.MapId));


        // start initializing map
        BBHUIManager.instance.mainMenu.BlendImage(true);
        map.Initialize(this, false, this.transform.position, RoomManagerReader.Data.RoomInfo.Info.MapInfo.MapData, () => {
                RoomManagerCommandSender.SendReadyToJoinCommand(EntityId, new ReadyToJoinRequest(RoomPlayerClientBehaviour.Instance.EntityId, RoomPlayerClientBehaviour.Instance.RoomPlayerReader.Data.Pubkey), (cb2) => {
                    if (cb2.StatusCode != Improbable.Worker.CInterop.StatusCode.Success)
                    {
                        BBHUIManager.instance.mainMenu.BlendImage(false);
                        Debug.LogError(cb2.Message);
                        return;
                    }
                    BBHUIManager.instance.mainMenu.BlendImage(false);
                    OnMapLoaded?.Invoke();
            });
        });
        
    }

    public void Deinitialize()
    {

        Debug.Log("Deinitialize called on " + transform.name);
        if(RoomStatsReader != null)
        {
            RoomStatsReader.RemoveAllCallbacks();
        }
        if (map == null)
        {
            return;
        }
        map.Remove();
        map = null;
    }

}
