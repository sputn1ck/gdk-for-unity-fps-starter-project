using Daemon;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DummyDaemonClient : MonoBehaviour, IDonnerDaemonClient
{
    public long bufferBalance;
    public long channelMissingBalance;
    public long daemonBalance;

    public async Task Setup()
    {

    }

    public void Shutdown()
    {

    }

    public Task<GetBalanceResponse> GetWalletBalance()
    {
        return Task.FromResult(new GetBalanceResponse
        {
            BufferBalance = bufferBalance,
            ChannelMissingBalance = channelMissingBalance,
            DaemonBalance = daemonBalance,
        });
    }

    public Task<GetConnectionResponse> GetConnection()
    {
        return Task.FromResult(new GetConnectionResponse { });
    }

    public Task<string> Lncli(string command)
    {
        return Task.FromResult("yooooo lncli or what;" + command);
    }
}
