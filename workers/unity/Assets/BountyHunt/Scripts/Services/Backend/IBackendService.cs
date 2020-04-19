using Bbhrpc;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IBackendPlayerClient
{
    void Setup(string target, int port, string pubkey, string signature);
    void Shutdown();

    Task<string> GetUsername(string pubkey);
    Task<string> SetUsername(string pubkey, string userName);
    Task<Ranking[]> ListRankings(int length, int startIndex, RankType rankType);


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
}
