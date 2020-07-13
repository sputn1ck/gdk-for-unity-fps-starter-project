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
    [Require] EntityId EntityId;


    public Dictionary<string, PlayerStats> playerStats;
    private Map map;
    private void OnEnable()
    {
        Debug.Log("enabling room " + RoomManagerReader.Data.RoomInfo.RoomId);
        Initialize();
        RoomManagerReader.OnMapUpdateEvent += OnStatsUpdate;
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
    }

    private void OnDisable()
    {
        map.Remove();
    }

    private void Initialize()
    {
        map = Instantiate(MapDictStorage.Instance.GetMap(RoomManagerReader.Data.RoomInfo.MapInfo.MapId));


        // start initializing map
        BBHUIManager.instance.mainMenu.BlendImage(true);
        map.Initialize(this, false, this.transform.position, RoomManagerReader.Data.RoomInfo.MapInfo.MapData, () => {
            // initializing finished
            RoomManagerCommandSender.SendRequestStatsCommand(EntityId, new Bountyhunt.Empty(),(cb) => {
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

}
