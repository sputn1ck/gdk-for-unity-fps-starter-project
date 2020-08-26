using Bountyhunt;
using System.Collections.Generic;

public interface IServerRoomGameStatsMap
{
    void AddBounty(string playerId, long bounty);
    void AddEarnings(string playerId, long earnings);
    void AddKill(string killerId, string victimId);
    void AddPlayer(string pubkey);
    void AddScore(string playerId, long score);
    Dictionary<string, PlayerStats> GetPlayerDictionary();
    PlayerStats GetStats(string playerId);
    void Initialize(Room room);
    void RemovePlayer(string pubkey);
    void Reset();
    void SetBounty(string playerId, long bounty);
    void UpdateDictionary(Dictionary<string, PlayerStats> newMap);
}
