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

    public bool randomBalance;
    public async Task Setup()
    {

    }

    public void Shutdown()
    {

    }

    public async Task<GetBalanceResponse> GetWalletBalance()
    {
        await Task.Delay(UnityEngine.Random.Range(10, 100));
        GetBalanceResponse res;
        if (randomBalance)
        {
            res = new GetBalanceResponse
            {
                BufferBalance = UnityEngine.Random.Range(0, 4999),
                ChannelMissingBalance = UnityEngine.Random.Range(0, 5000),
                DaemonBalance = UnityEngine.Random.Range(0, int.MaxValue),
            };
        } else
        {
            res = new GetBalanceResponse
            {
                BufferBalance = bufferBalance,
                ChannelMissingBalance = channelMissingBalance,
                DaemonBalance = daemonBalance,
            };
        }
        return res;
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
