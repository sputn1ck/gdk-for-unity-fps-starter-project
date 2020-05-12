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

    private ConcurrentQueue<EventStreamRequest> toBackendQueue;
    private ConcurrentQueue<BackendStreamResponse> fromBackendQueue;
    private Thread gameServerListenThread;
    private Thread backendListenThread;

    private string pubkey;
    private string message;

    public void Setup(string target, int port, string pubkey, string message)
    {
        rpcChannel = new Grpc.Core.Channel(target, port,Grpc.Core.ChannelCredentials.Insecure);
        _client = new GameServerService.GameServerServiceClient(rpcChannel);
        toBackendQueue = new ConcurrentQueue<EventStreamRequest>();
        fromBackendQueue = new ConcurrentQueue<BackendStreamResponse>();
        this.pubkey = pubkey;
        this.message = message;
    }
    public IEnumerator HandleBackendEvents(CancellationTokenSource ct)
    {
        while (!ct.IsCancellationRequested)
        {
            if (fromBackendQueue.TryDequeue(out BackendStreamResponse e))
            {
                switch (e.EventCase)
                {
                    case BackendStreamResponse.EventOneofCase.Kick:
                        ServerEvents.instance.OnBackendKickEvent.Invoke(e.Kick);
                        break;
                    default:
                        break;
                }
            }
            yield return new WaitForEndOfFrame();
        }
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
        StartGameServerStream();
        StartBackendListening();
    }
    public void StartGameServerStream()
    {
        gameServerListenThread = new Thread(async () =>
        {
            while (!ServerServiceConnections.ct.IsCancellationRequested)
            {
                await eventStream();
                Thread.Sleep(1000);
            }
        });
        gameServerListenThread.Start();
    }
    public void StartBackendListening()
    {
        backendListenThread = new Thread(async () =>
        {
            while (!ServerServiceConnections.ct.IsCancellationRequested)
            {
                await ListenBackend();
                Thread.Sleep(1000);
            }
        });
        backendListenThread.Start();
    }
    public async Task ListenBackend()
    {
        var request = new BackendStreamRequest();

        try
        {
            using (var _eventStream = _client.BackendStream(request, GetPubkeyCalloptions()))
            {
                Debug.Log("Backend  Stream listening successfully started");
                while (!rpcChannel.ShutdownToken.IsCancellationRequested)
                {
                    var res = await _eventStream.ResponseStream.MoveNext();
                    fromBackendQueue.Enqueue(_eventStream.ResponseStream.Current);
                }

            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        if (!rpcChannel.ShutdownToken.IsCancellationRequested)
        {
            await Task.Delay(1000);
            StartBackendListening();
        }
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
        toBackendQueue.Enqueue(request);
        
    }

    private async Task eventStream()
    {
        using (var call = _client.EventStream(GetPubkeyCalloptions()))
        {
            Debug.Log("opening stream");
            toBackendQueue.Enqueue(new EventStreamRequest());
            while (!ServerServiceConnections.ct.IsCancellationRequested)
            {
                EventStreamRequest current;
                if (toBackendQueue.TryDequeue(out current))
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
                                toBackendQueue.Enqueue(current);
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
        var co = new CallOptions(headers: md,  cancellationToken: rpcChannel.ShutdownToken);
        return co;
    }

    public async Task<string> GetUserSkin(string pubkey)
    {
        var res = await _client.GetUserSkinAsync(new GetUserSkinRequest { Pubkey = pubkey }, GetPubkeyCalloptions());
        return res.EquippedSkin;
    }
}


