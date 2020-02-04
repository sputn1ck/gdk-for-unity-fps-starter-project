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


    public void Setup(string target)
    {
        rpcChannel = new Grpc.Core.Channel(target, 8899,Grpc.Core.ChannelCredentials.Insecure);
        _client = new GameService.GameServiceClient(rpcChannel);
        eventQueue = new ConcurrentQueue<EventStreamRequest>();
        ct = new CancellationTokenSource();
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

    
    public void AddToQueue(EventStreamRequest request)
    {
        eventQueue.Enqueue(request);
        
    }

    private async Task eventStream()
    {
        using (var call = _client.EventStream())
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

}


