using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bbhrpc;
using System.Collections.Concurrent;
using System.Threading;
using System;
using System.Threading.Tasks;
using Grpc.Core;

public class BackendGameserverClient : IBackendServerClient
{

    private GameServerService.GameServerServiceClient _client;

    private Grpc.Core.Channel rpcChannel;

    private ConcurrentQueue<EventStreamRequest> eventQueue;

    private Thread listenThread;

    private string pubkey;
    private string message;

    public void Setup(string target, int port, string pubkey, string message)
    {
        rpcChannel = new Grpc.Core.Channel(target, port,Grpc.Core.ChannelCredentials.Insecure);
        _client = new GameServerService.GameServerServiceClient(rpcChannel);
        eventQueue = new ConcurrentQueue<EventStreamRequest>();
        this.pubkey = pubkey;
        this.message = message;
    }

    public async Task<User> GetUser(string pubkey)
    {
        var res = await _client.GetUserAsync(new GetUserRequest() { Pubkey = pubkey}, GetPubkeyCalloptions());
        return res.User;
    }

    public void Shutdown()
    {
        Debug.Log(rpcChannel.State + "state, shutdownToken: " + rpcChannel.ShutdownToken.IsCancellationRequested);
        Task t = Task.Run(async () => await rpcChannel.ShutdownAsync());
        t.Wait(5000);
        Debug.Log(rpcChannel.State + "state, shutdownToken: " + rpcChannel.ShutdownToken.IsCancellationRequested);

    }
    public void StartListening()
    {
        listenThread = new Thread(async () =>
        {
            while (!ServerServiceConnections.ct.IsCancellationRequested)
            {
                await eventStream();
                Thread.Sleep(1000);
            }
        });
        listenThread.Start();
    }


    public void AddKill(string killer, string victim)
    {
        AddToQueue(new EventStreamRequest { Kill = new Bbhrpc.KillEvent() { Killer = killer, Victim = victim } });
    }

    public void AddEarnings(string user, long earnings)
    {
        AddToQueue(new EventStreamRequest { Earnings = new Bbhrpc.EarningsEvent { Amt = earnings, User = user } });
    }

    public void AddPlayerHeartbeat(string user, long bounty, int kills, int deaths)
    {
        AddToQueue(new EventStreamRequest
        {
            PlayerInfo = new Bbhrpc.PlayerInfoEvent()
            {
                User = user,
                EventType = PlayerInfoEvent.Types.EventType.Heartbeat,
                CurrentBounty = bounty,
                CurrentDeaths = deaths,
                CurrentKills = kills
            }
        });
    }
    public void AddPlayerDisconnect(string user)
    {
        AddToQueue(new EventStreamRequest { PlayerInfo = new PlayerInfoEvent() { User = user, EventType = PlayerInfoEvent.Types.EventType.Disconnect } });
    }

    private void AddToQueue(EventStreamRequest request)
    {
        eventQueue.Enqueue(request);
        
    }

    private async Task eventStream()
    {
        using (var call = _client.EventStream(GetPubkeyCalloptions()))
        {
            Debug.Log("opening stream");
            eventQueue.Enqueue(new EventStreamRequest());
            while (!ServerServiceConnections.ct.IsCancellationRequested)
            {
                EventStreamRequest current;
                if (eventQueue.TryDequeue(out current))
                {
                    if (current != null)
                    {
                        try
                        {
                            await call.RequestStream.WriteAsync(current);
                        }
                        catch (Grpc.Core.RpcException e)
                        {
                            if (e.Status.Detail == "failed to connect to all addresses")
                            {
                                Debug.Log("failed to connect");
                                eventQueue.Enqueue(current);
                                await call.RequestStream.CompleteAsync();

                            }
                            Debug.Log("ERROR " + e);
                            return;
                        }

                    }
                }
            }

            await call.RequestStream.CompleteAsync();
        }
    }

    public async Task<GetRoundInfoResponse> GetRoundInfo(GetRoundInfoRequest request)
    {
        return await _client.GetRoundInfoAsync(request,GetPubkeyCalloptions());
    }
    

    private CallOptions GetPubkeyCalloptions()
    {
        var md = new Metadata();
        md.Add("pubkey", pubkey);
        md.Add("sig", message);
        var co = new CallOptions(headers: md);
        return co;
    }

}


