using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightningAuction.Delivery;
using Grpc.Core;

using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using System.Threading;
using System.Threading.Tasks;

public class AuctionController 
{

    public string host = "167.172.175.172:5113";
    public string signature = "d9haxokjpg1idsrsm5bycs1whnmyqdyc5xrtpx9ph5kd3zwfesxjenxxt4b8g8x4usuh7t7ajym6a8bcu78itmo5tmowig7nwha9zqux";


    LightningAuctionAdmin.LightningAuctionAdminClient adminClient;
    Channel rpcChannel;
    CallOptions callOptions;
    Worker worker;
    CancellationTokenSource cancellationToken;
    public void Setup()
    {

        cancellationToken = new CancellationTokenSource();

        rpcChannel = new Channel(host, ChannelCredentials.Insecure);
        adminClient = new LightningAuctionAdmin.LightningAuctionAdminClient(rpcChannel);
    }


    IEnumerator AuctionEnumerator()
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (FlagManager.instance.GetShouldRunAuction())
            {
                var duration = FlagManager.instance.GetAuctionDuration();
                StartAuction(duration);
                yield return new WaitForSeconds(duration + 15f);
            }
            yield return null;
        }
        yield return null;

    }
    // Todo implement
    public async Task<StartAuctionResponse> StartAuction(int duration)
    {
        var req = new StartAuctionRequest
        {
            Duration = duration
        };
        try
        {
            var res = await adminClient.StartAuctionAsync(req, GetCallOptions());

            return res;
            //serverChat.SendAuctionStartedChatMessage("new auction started! Duration: " + req.Duration + " seconds");
        }
        catch (RpcException e)
        {
            Debug.Log(e);
            return null;
        }

    }

    CallOptions GetCallOptions()
    {
        var md = new Metadata();
        md.Add("signature", signature);
        var co = new CallOptions(headers: md);
        return co;
    }
    public void Shutdown()
    {

        cancellationToken.Cancel();
        Task t = Task.Run(async () => await rpcChannel.ShutdownAsync());
        t.Wait(5000);
    }


}
