using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Improbable.Gdk.Subscriptions;


public class ServerRoomGameStatsMap : MonoBehaviour, IServerRoomGameStatsMap
{
    [Require] RoomStatsManagerWriter RoomStatsWriter;
    [Require] RoomStatsManagerCommandReceiver RoomStatsCommandReceiver;

    private Dictionary<string, PlayerStats> playerStats;

    private void OnEnable()
    {
        RoomStatsCommandReceiver.OnRequestStatsRequestReceived += RequestStats;
    }

    public void SetBounty(string playerId, long bounty)
    {
        var map = new Dictionary<string, PlayerStats>();
        if (playerStats.TryGetValue(playerId, out var playerStat))
        {
            playerStat.Bounty = bounty;
            map[playerId] = playerStat;
        }
        UpdateDictionary(map);
    }

    public void AddScore(string playerId, long score)
    {
        var map = new Dictionary<string, PlayerStats>();
        if (playerStats.TryGetValue(playerId, out var playerStat))
        {
            playerStat.Score += score;
            map[playerId] = playerStat;
        }
        UpdateDictionary(map);
    }
    public void AddEarnings(string playerId, long earnings)
    {
        var map = new Dictionary<string, PlayerStats>();
        if (playerStats.TryGetValue(playerId, out var playerStat))
        {
            playerStat.SessionEarnings += earnings;
            map[playerId] = playerStat;
        }
        UpdateDictionary(map);
    }

    public void AddBounty(string playerId, long bounty)
    {
        var map = new Dictionary<string, PlayerStats>();
        if (playerStats.TryGetValue(playerId, out var playerStat))
        {
            playerStat.Bounty += bounty;
            map[playerId] = playerStat;
        }
        UpdateDictionary(map);
    }

    public void Initialize(Room room)
    {
        playerStats = new Dictionary<string, PlayerStats>();
        foreach (var player in room.PlayerInfo.ActivePlayers)
        {
            playerStats.Add(player.Key, new PlayerStats(0, 0, 0, 0, false, 0));
        }
        RoomStatsWriter.SendMapUpdateEvent(new PlayerStatsUpdate(playerStats, new List<string>(), true));
    }

    public void Reset()
    {
        var newMap = new Dictionary<string, PlayerStats>();
        foreach (var player in playerStats)
        {
            newMap.Add(player.Key, new PlayerStats(0, 0, 0, 0, player.Value.Active, 0));
        }
        playerStats = newMap;
        RoomStatsWriter.SendMapUpdateEvent(new PlayerStatsUpdate(newMap, new List<string>(), true));
    }

    public void AddKill(string killerId, string victimId)
    {
        var map = new Dictionary<string, PlayerStats>();
        if (playerStats.TryGetValue(killerId, out var killerStats))
        {
            killerStats.Kills++;
            map[killerId] = killerStats;
        }
        if (playerStats.TryGetValue(victimId, out var victimStats))
        {
            victimStats.Deaths++;
            map[victimId] = victimStats;
        }
        UpdateDictionary(map);
    }

    private void RequestStats(RoomStatsManager.RequestStats.ReceivedRequest obj)
    {
        RoomStatsCommandReceiver.SendRequestStatsResponse(obj.RequestId, new PlayerStatsUpdate(playerStats, new List<string>(), true));
    }

    public PlayerStats GetStats(string playerId)
    {
        if (playerStats.TryGetValue(playerId, out var playerStat))
        {
            return playerStat;
        }
        return new PlayerStats();
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
            map.Add(pubkey, new PlayerStats(0, 0, 0, 0, true, 0));
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
    public Dictionary<string, PlayerStats> GetPlayerDictionary()
    {
        return playerStats;
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
