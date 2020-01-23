using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bbh;
using System.Collections.Concurrent;
using System.Threading;
using System;

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
        
        ct.Cancel();
        listenThread.Abort();
    }
    public void StartListening()
    {
        listenThread = new Thread(async () => {
            using (var call = _client.EventStream())
            {
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
                            } catch (Grpc.Core.RpcException e)
                            {
                                
                            }
                            
                        }
                            
                    }
                }
            }
        });
        listenThread.Start();
    }

    public void AddToQueue(EventStreamRequest request)
    {
        eventQueue.Enqueue(request);
    }

}


