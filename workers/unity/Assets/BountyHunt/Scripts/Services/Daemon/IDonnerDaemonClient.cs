using Daemon;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public interface IDonnerDaemonClient
{
    Task Setup();
    void Shutdown();
    Task<GetBalanceResponse> GetWalletBalance();
    Task<GetConnectionResponse> GetConnection();
    Task<string> Lncli(string command);

    Task Withdraw(CancellationTokenSource ct,OnBechstring onBechstring, OnWaiting onWaiting, OnFinished onFinished);
}

public delegate void OnFinished(Finished finished);
public delegate void OnWaiting();
public delegate void OnBechstring(CancellationTokenSource ct, string bechstring);

