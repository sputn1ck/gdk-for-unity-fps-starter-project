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


    public Dictionary<string, PlayerStats> playerStats;
    private Map map;
    private void OnEnable()
    {
        Debug.Log("enabling room " + RoomManagerReader.Data.RoomInfo.RoomId);
        ClientGameObjectManager.Instance.AddRoomGo(EntityId, this.gameObject);
    }

    private void OnStatsUpdate(PlayerStatsUpdate obj)
    {
        // TODO send scoreboard update
        if (obj.ReplaceMap)
        {
            playerStats = obj.PlayerStats;
        } else
        {
            foreach(var kv in obj.PlayerStats)
            {
                playerStats[kv.Key] = kv.Value;
            }
        }
        if(obj.RemovePlayers != null)
        {
            foreach (var keys in obj.RemovePlayers)
            {
                playerStats.Remove(keys);
            }
        }
    }

    private void OnDisable()
    {

        ClientGameObjectManager.Instance.RemoveRoomGo(EntityId);
        Deinitialize();
    }

    public void Initialize()
    {
        Debug.Log("Iniailized called on " + transform.name);

        RoomStatsReader.OnMapUpdateEvent += OnStatsUpdate;
        map = Instantiate(MapDictStorage.Instance.GetMap(RoomManagerReader.Data.RoomInfo.MapInfo.MapId));


        // start initializing map
        BBHUIManager.instance.mainMenu.BlendImage(true);
        map.Initialize(this, false, this.transform.position, RoomManagerReader.Data.RoomInfo.MapInfo.MapData, () => {
            // initializing finished
            RoomStatsCommandSender.SendRequestStatsCommand(EntityId, new Bountyhunt.Empty(),(cb) => {
                if (cb.StatusCode != Improbable.Worker.CInterop.StatusCode.Success)
                {
                    BBHUIManager.instance.mainMenu.BlendImage(false);
                    Debug.LogError(cb.Message);
                    return;
                }
                // TODO update scoreboard
                playerStats = cb.ResponsePayload.Value.PlayerStats;
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
        });
        
    }
    public void Deinitialize()
    {
        Debug.Log("Deinitialize called on " + transform.name);
        RoomStatsReader.RemoveAllCallbacks();
        if (map == null)
        {
            return;
        }
        map.Remove();
        map = null;
    }

}
