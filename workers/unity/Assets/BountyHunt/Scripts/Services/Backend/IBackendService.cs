using Bbhrpc;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IBackendPlayerClient
{
    Task Setup(string target, int port, string pubkey, string signature);
    void Shutdown();

    Task<string> GetUsername();
    Task<string> SetUsername(string userName);

    Task<bool> NeedsUsernameChange();
    Task<(Ranking[] rankings, int totalElements)> ListRankings(int length, int startIndex, RankType rankType);
    
    Task<SkinInventory> GetSkinInventory();
    void EquipSkin(string skinId);
    Task<ShopSkin[]> GetAllSkins();
    Task<string[]> GetAllSkinIds();
    Task<string> GetSkinInvoice(string skinId);

    Task<string> GetGameVersion();

    Task<int> GetPlayerRank(string playername, RankType rankType);

    Task<bool> WaitForPayment(string invoice, long expiry);
}

public interface IBackendServerClient
{
    void Setup(string target, int port, string pubkey, string message);
    void Shutdown();
    void StartListening();
    Task<User> GetUser(string pubkey);
    void AddKill(string killer, string victim);
    void AddEarnings(string user, long earnings);
    void AddPlayerHeartbeat(string user, long bounty, int kills, int deaths);
    void AddPlayerDisconnect(string user);
    Task<GetRoundInfoResponse> GetRoundInfo(GetRoundInfoRequest request);
    Task<string> GetUserSkin(string pubkey);
}
