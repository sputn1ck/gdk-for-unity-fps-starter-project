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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup()
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
