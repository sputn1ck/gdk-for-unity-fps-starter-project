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

    private Channel rpcChannel;

    private string pubkey;
    private string signature;
    public async Task Setup(string target,int port, string pubkey, string signature)
    {
        rpcChannel = new Grpc.Core.Channel(target, port, Grpc.Core.ChannelCredentials.Insecure);
        client = new GameClientService.GameClientServiceClient(rpcChannel);
        publicClient = new PublicService.PublicServiceClient(rpcChannel);
        skinClient = new SkinService.SkinServiceClient(rpcChannel);
        this.pubkey = pubkey;
        this.signature = signature;
        
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

    public async Task<Ranking[]> ListRankings(int length, int startIndex, RankType rankType)
    {
        var res = await publicClient.ListRankingsAsync(new ListRankingsRequest
        {
            RankType = rankType,
            Length = length,
            StartIndex = startIndex
        });
        return res.Rankings.ToArray();
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

    public async void EquipSkin(string skinId)
    {
        await skinClient.EquipSkinAsync(new EquipSkinRequest() { Id = skinId }, GetPubkeyCalloptions());
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

        //Debug.Log(rpcChannel.State + "state, shutdownToken: " + rpcChannel.ShutdownToken.IsCancellationRequested);
        Task t = Task.Run(async () => await rpcChannel.ShutdownAsync());
        t.Wait(5000);
        //Debug.Log(rpcChannel.State + "state, shutdownToken: " + rpcChannel.ShutdownToken.IsCancellationRequested);
    }

    public async Task<Ranking[]> GetTop100EarningsRankings()
    {
        var res = await publicClient.ListRankingsAsync(new ListRankingsRequest
        {
            RankType = RankType.Earnings,
            Length = 100,
            StartIndex = 0
        });
        return res.Rankings.ToArray();
    }

    public Task<string[]> GetAllSkinIds()
    {
        throw new NotImplementedException();
    }
}
