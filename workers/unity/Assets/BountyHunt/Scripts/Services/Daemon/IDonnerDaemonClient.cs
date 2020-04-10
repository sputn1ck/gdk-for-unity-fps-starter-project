using Daemon;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IDonnerDaemonClient
{
    void Setup();
    void Shutdown();
    Task<GetBalanceResponse> GetWalletBalance();
    Task<GetConnectionResponse> GetConnection();
    Task<string> Lncli(string command);
}


