using Bbhrpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Threading;

public interface IBackendPlayerClient
{
    // Basic
    Task Setup(string target, int port, string pubkey, string signature);
    void Shutdown();

    Task<string> GetGameVersion();
    Task WaitForPayment(string invoice, long expiryTimestamp, CancellationToken cancellationToken);

    // Username
    Task<string> GetUsername();
    Task<string> SetUsername(string userName);

    Task<bool> NeedsUsernameChange();

    // Skin Stuff
    Task<SkinInventory> GetSkinInventory();
    Task EquipSkin(string skinId);
    Task<ShopSkin[]> GetAllSkins();
    Task<string[]> GetAllSkinIds();
    Task<string> GetSkinInvoice(string skinId);
    Task<string> GetDonationInvoice(long gameDonation,long devsDonation);

    // Ranking Stuff
    Task<Ranking> GetPlayerRanking();
    Task<Ranking> GetSpecificPlayerRanking(string pubkey);
    Task<GetRankingInfoResponse> GetRankingInfo();
    Task<(Ranking[] rankings, int totalElements)> ListRankings(int length, int startIndex, RankType rankType);

    Task<GetInfoResponse> GetInfo();

    Task<ListAdvertiserResponse> ListAdvertisers();
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
    IEnumerator HandleBackendEvents(CancellationTokenSource ct);
}

public delegate void BackendEventHandler(object obj, BackendStreamResponse e);

public class ExpiredException : Exception
{
    public ExpiredException()
    {

    }
    public ExpiredException(string message) : base(message)
    {

    }
    public ExpiredException(string message, Exception inner) : base(message, inner)
    {

    }
}
