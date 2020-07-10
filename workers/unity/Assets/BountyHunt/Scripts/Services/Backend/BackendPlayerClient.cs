using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bbhrpc;
using System.Collections.Concurrent;
using System.Threading;
using System;
using Grpc.Core;
using System.Threading.Tasks;
using System.Linq;

public class BackendPlayerClient : IBackendPlayerClient
{
    public GameClientService.GameClientServiceClient client;
    public PublicService.PublicServiceClient publicClient;
    public SkinService.SkinServiceClient skinClient;
    public AdvertiserService.AdvertiserServiceClient adClient;
    private Channel rpcChannel;

    private string pubkey;
    private string signature;

    private CancellationTokenSource CancellationTokenSource;

    

    public async Task Setup(string target,int port, string pubkey, string signature)
    {
        rpcChannel = new Grpc.Core.Channel(target, port, Grpc.Core.ChannelCredentials.Insecure);
        client = new GameClientService.GameClientServiceClient(rpcChannel);
        publicClient = new PublicService.PublicServiceClient(rpcChannel);
        skinClient = new SkinService.SkinServiceClient(rpcChannel);
        adClient = new AdvertiserService.AdvertiserServiceClient(rpcChannel);
        this.CancellationTokenSource = new CancellationTokenSource();
        this.pubkey = pubkey;
        this.signature = signature;
        await GetUsername();
        
        
    }

    public async Task<string> GetUsername()
    {
        
        var res = await client.GetUsernameAsync(new GetUsernameRequest() { }, GetPubkeyCalloptions());
        return res.Name;
    }

    public async Task<string> SetUsername(string userName)
    {
        var res = await client.SetUsernameAsync(new SetUsernameRequest() { Name = userName }, GetPubkeyCalloptions());
        return res.Name;
    }
    
    public async Task<(Ranking[] rankings, int totalElements)> ListRankings(int length, int startIndex, RankType rankType)
    {
        var res = await publicClient.ListRankingsAsync(new ListRankingsRequest
        {
            RankType = rankType,
            Length = length,
            StartIndex = startIndex
        });
        return (res.Rankings.ToArray(), res.TotalElements);
    }

    public async Task<SkinInventory> GetSkinInventory()
    {
        var res = await skinClient.GetSkinInventoryAsync(new GetSkinInventoryRequest(), GetPubkeyCalloptions());
        return res.SkinInventory;
    }

    public async Task<ShopSkin[]> GetAllSkins()
    {
        var res = await skinClient.ListSkinsAsync(new ListSkinsRequest(), GetPubkeyCalloptions());
        return res.ShopItems.ToArray();
    }

    public async Task EquipSkin(string skinId)
    {
        try
        {
            await skinClient.EquipSkinAsync(new EquipSkinRequest() { Id = skinId }, GetPubkeyCalloptions());
        }
        catch(Exception e)
        {
            throw e;
        }

    }
    public async Task<string> GetSkinInvoice(string skinId)
    {
        var res = await skinClient.BuySkinAsync(new BuySkinRequest { Id = skinId }, GetPubkeyCalloptions());
        return res.Invoice;
    }

    private CallOptions GetPubkeyCalloptions()
    {
        var md = new Metadata();
        md.Add("pubkey", pubkey);
        md.Add("sig", signature);
        var co = new CallOptions(headers: md);
        return co;
    }

    public void Shutdown()
    {
        CancellationTokenSource?.Cancel();
        //Debug.Log(rpcChannel.State + "state, shutdownToken: " + rpcChannel.ShutdownToken.IsCancellationRequested);
        Task t = Task.Run(async () => await rpcChannel.ShutdownAsync());
        t.Wait(5000);
        //Debug.Log(rpcChannel.State + "state, shutdownToken: " + rpcChannel.ShutdownToken.IsCancellationRequested);
    }

    public async Task<string[]> GetAllSkinIds()
    {
        var res = await skinClient.ListSkinsAsync(new ListSkinsRequest(), GetPubkeyCalloptions());
        var skinIds = new string[res.ShopItems.Count];
        for (int i = 0; i < skinIds.Length; i++) { 
            skinIds[i] = res.ShopItems[i].Id;
        }
        return skinIds;
    }

    public async Task<string> GetGameVersion()
    {
        var info = await publicClient.GetInfoAsync(new GetInfoRequest());
        return info.GameInfo.GameVersion;
    }

    public async Task<bool> NeedsUsernameChange()
    {
        var name = await GetUsername();
        bool setName;
        if (pubkey == name)
        {
            setName = true;
        }
        else
        {
            setName = false;

        }
        return setName;
    }

    public async Task WaitForPayment(string invoice, long expiryTimestamp, CancellationToken cancellationToken)
    {
        await Task.Run(async () =>
        {
            long startTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long endTime = expiryTimestamp;
            int time = (int)(endTime - startTime);
            CancellationTokenSource expiryToken = new CancellationTokenSource();
            expiryToken.CancelAfter(time*1000);
            var callToken = CancellationTokenSource.CreateLinkedTokenSource(expiryToken.Token, cancellationToken);
            using (var call = publicClient.SubscribeInvoiceStream(new SubscribeInvoiceStreamRequest { Invoice = invoice }, cancellationToken: CancellationTokenSource.Token))
            {
                try
                {
                    var res = await call.ResponseStream.MoveNext(callToken.Token);
                    if (res)
                    {
                        if (call.ResponseStream.Current.Payed)
                            return;
                    }
                }
                catch (RpcException e)
                {
                    if (e.StatusCode == StatusCode.Cancelled)
                    {
                        throw new ExpiredException();
                    }
                    throw e;
                }

            }
        });
    }


    public async Task<Ranking> GetPlayerRanking()
    {
        var res = await publicClient.GetRankingAsync(new GetRankingRequest() { Pubkey = this.pubkey });
        return res.Ranking;
    }

    public async Task<Ranking> GetSpecificPlayerRanking(string pubkey)
    {
        var res = await publicClient.GetRankingAsync(new GetRankingRequest() { Pubkey = pubkey });
        return res.Ranking;
    }

    public async Task<GetRankingInfoResponse> GetRankingInfo()
    {
        var res = await publicClient.GetRankingInfoAsync(new GetRankingInfoRequest());
        return res;
    }

    public async Task<string> GetDonationInvoice(long gameDonation, long devsDonation)
    {
        var res = await publicClient.GetDonationInvoiceAsync(new GetDonationInvoiceRequest
        {
            DevAmount = devsDonation,
            GameAmount = gameDonation,
            Benefactor = pubkey,
        });
        return res.Invoice;
    }

    public async Task<GetInfoResponse> GetInfo()
    {
        var res = await publicClient.GetInfoAsync(new GetInfoRequest());
        return res;
    }

    public async Task<ListAdvertiserResponse> ListAdvertisers()
    {
        var res = await adClient.ListAdvertisersAsync(new ListAdvertisersRequest());
        return res;
    }

    public async Task<string> GetPlayerSatsInvoice(string pHash, long psats)
    {
        var req = new DepositAdvertiserRequest { Amount = psats, Phash = pHash };
        var res = await adClient.DepositAdvertiserAsync(req);
        return res.Invoice;
    }

    public async Task<string> GetBountyInvoice(string pubkey, long amount)
    {
        var res = await publicClient.AddBountyAsync(new AddBountyRequest()
        {
            Amount = amount,
            Pubkey = pubkey,
        });
        return res.Invoice;
    }
}
