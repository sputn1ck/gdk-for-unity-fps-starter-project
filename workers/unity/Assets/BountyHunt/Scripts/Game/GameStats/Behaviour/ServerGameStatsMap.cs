using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Improbable.Gdk.Subscriptions;

public class ServerGameStatsMap : MonoBehaviour
{

    [Require] RoomStatsWriter RoomStatsWriter;
    [Require] RoomStatsCommandReceiver RoomStatsCommandReceiver;

    private Dictionary<string, PlayerStats> playerStats;

    private void OnEnable()
    {
        RoomStatsCommandReceiver.OnRequestStatsRequestReceived += RequestStats;
        RoomStatsCommandReceiver.OnAddKillRequestReceived += AddKill;
        
    }
    public void Initialize(Room room)
    {
        playerStats = new Dictionary<string, PlayerStats>();
        foreach (var player in room.ActivePlayers)
        {
            playerStats.Add(player, new PlayerStats(0, 0, 0, 0, false));
        }
        RoomStatsWriter.SendMapUpdateEvent(new PlayerStatsUpdate(playerStats, new List<string>(), true));
    }

    private void AddKill(RoomStats.AddKill.ReceivedRequest obj)
    {
        var map = new Dictionary<string, PlayerStats>();
        if(playerStats.TryGetValue(obj.Payload.KillerId, out var killerStats))
        {
            killerStats.Kills++;
            map[obj.Payload.KillerId] = killerStats;
        }
        if (playerStats.TryGetValue(obj.Payload.VictimId, out var victimStats))
        {
            victimStats.Deaths++;
            map[obj.Payload.VictimId] = victimStats;
        }
        UpdateDictionary(map);
    }

    private void RequestStats(RoomStats.RequestStats.ReceivedRequest obj)
    {
        RoomStatsCommandReceiver.SendRequestStatsResponse(obj.RequestId, new PlayerStatsUpdate(playerStats, new List<string>(), true));
    }

    
    public void AddPlayer(string pubkey)
    {
        var map = new Dictionary<string, PlayerStats>();
        if (playerStats.TryGetValue(pubkey, out var player))
        {
            player.Active = true;
            map[pubkey] = player;
        }
        else
        {
            map.Add(pubkey, new PlayerStats(0, 0, 0, 0, true));
        }
        UpdateDictionary(map);
    }

    public void RemovePlayer(string pubkey)
    {
        var map = new Dictionary<string, PlayerStats>();
        if (playerStats.TryGetValue(pubkey, out var player))
        {
            player.Active = false;
            map[pubkey] = player;

            UpdateDictionary(map);
        }
        
    }
    public void UpdateDictionary(Dictionary<string, PlayerStats> newMap)
    {
        foreach (var kv in newMap)
        {
            if (playerStats.ContainsKey(kv.Key))
            {
                playerStats[kv.Key] = kv.Value;
            }
            else
            {
                playerStats.Add(kv.Key, kv.Value);
            }

        }
        RoomStatsWriter.SendMapUpdateEvent(new PlayerStatsUpdate(newMap, new List<string>(), false));
    }
}
