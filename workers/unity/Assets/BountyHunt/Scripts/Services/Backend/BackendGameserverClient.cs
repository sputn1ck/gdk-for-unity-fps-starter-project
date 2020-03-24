using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bbh;
using System.Collections.Concurrent;
using System.Threading;
using System;
using System.Threading.Tasks;
using Grpc.Core;

public class BackendGameserverClient
{

    private GameService.GameServiceClient _client;

    private Grpc.Core.Channel rpcChannel;

    private ConcurrentQueue<Bbh.EventStreamRequest> eventQueue;

    private CancellationTokenSource ct;
    private Thread listenThread;

    private string pubkey;
    private string message;

    public void Setup(string target, int port, string pubkey, string message)
    {
        rpcChannel = new Grpc.Core.Channel(target, port,Grpc.Core.ChannelCredentials.Insecure);
        _client = new GameService.GameServiceClient(rpcChannel);
        eventQueue = new ConcurrentQueue<EventStreamRequest>();
        ct = new CancellationTokenSource();
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

        
        ct.Cancel();
        Task t = Task.Run(async () => await rpcChannel.ShutdownAsync());
        t.Wait(5000);
        Debug.Log(rpcChannel.State + "state, shutdownToken: " + rpcChannel.ShutdownToken.IsCancellationRequested);

    }
    public void StartListening()
    {
        listenThread = new Thread(async () =>
        {
            while (!ct.IsCancellationRequested)
            {
                await eventStream();
                Thread.Sleep(1000);
            }
        });
        listenThread.Start();
    }

    public async Task<long> GetRoundBounty()
    {
        return 0;
    }
    public void AddKill(string killer, string victim)
    {
        AddToQueue(new Bbh.EventStreamRequest { Kill = new Bbh.KillEvent() { Killer = killer, Victim = victim } });
    }

    public void AddEarnings(string user, long earnings)
    {
        AddToQueue(new Bbh.EventStreamRequest { Earnings = new Bbh.EarningsEvent { Amt = earnings, User = user } });
    }

    public void AddPlayerHeartbeat(string user, long bounty, int kills, int deaths)
    {
        AddToQueue(new Bbh.EventStreamRequest
        {
            PlayerInfo = new Bbh.PlayerInfoEvent()
            {
                User = user,
                EventType = Bbh.PlayerInfoEvent.Types.EventType.Heartbeat,
                CurrentBounty = bounty,
                CurrentDeaths = deaths,
                CurrentKills = kills
            }
        });
    }
    public void AddPlayerDisconnect(string user)
    {
        AddToQueue(new Bbh.EventStreamRequest { PlayerInfo = new Bbh.PlayerInfoEvent() { User = user, EventType = Bbh.PlayerInfoEvent.Types.EventType.Disconnect } });
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
            while (!ct.IsCancellationRequested)
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


