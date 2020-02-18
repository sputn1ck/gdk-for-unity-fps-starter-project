using Chat;
using Grpc.Core;
using LightningAuction.Delivery;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AuctionClient
{
    Channel rpcChannel;
    LightningAuctionBidder.LightningAuctionBidderClient auctionClient;
    public void Setup()
    {
        rpcChannel = new Channel("167.172.175.172:5113", ChannelCredentials.Insecure);
        auctionClient = new LightningAuctionBidder.LightningAuctionBidderClient(rpcChannel);
    }

    public void Shutdown()
    {
        Task t = Task.Run(async () => await rpcChannel.ShutdownAsync());
        t.Wait(5000);
    }

    // TODO implement
    public async void AddDonation(string message, long amount)
    {
        
        try
        {
            var res = await auctionClient.SimpleChatAsync(new SimpleChatRequest { Amount = amount, Message = message });
            var invoice = await PlayerServiceConnections.instance.lnd.PayInvoice(res.PayReq);
            // TODO  check for donation siye

        }
        catch (RpcException e)
        {
           
                ChatPanelUI.instance.SpawnMessage(MessageType.ERROR_LOG ,"DONATION", e.Message, false);

        }
    }
    //TODO implement
    public async Task<AuctionEntry> AddBid(string message, long amount)
    {
        
        try
        {
            var activeAuction = await auctionClient.GetAuctionAsync(new GetAuctionRequest { AuctionId = "active" });
            var res = await auctionClient.BidAsync(new BidRequest { AuctionId = activeAuction.Auction.Id, Amount = amount, Message = message });
            var invoice = await PlayerServiceConnections.instance.lnd.PayInvoice(res.Entry.PaymentRequest);
            ChatPanelUI.instance.SpawnMessage(MessageType.INFO_LOG, "AUCTIONBID", "payment sent on route, check for payment status in ui");
            return res.Entry;
        }
        catch (RpcException e)
        {
            ChatPanelUI.instance.SpawnMessage(MessageType.ERROR_LOG, "AUCTIONBID", e.Message);
            return null;
        }
    }
    public async Task<AuctionEntry> BidStatus(string entryId)
    {
        
        try
        {
            var res = await auctionClient.GetBidAsync(new GetBidRequest { EntryId = entryId });
            return res.Entry;
        }
        catch (RpcException e)
        {
            ChatPanelUI.instance.SpawnMessage(MessageType.ERROR_LOG, "AUCTIONBID", e.Message);
            return null;
        }
    }

    public async Task<Auction> GetActiveAuction()
    {
        try
        {
            var activeAuction = await auctionClient.GetAuctionAsync(new GetAuctionRequest { AuctionId = "active" });
            return activeAuction.Auction;
        }
        catch (Exception e)
        {

            return null;
        }



    }
    
    public async void CancelBid(string BidId)
    {
        try
        {
            var res = await auctionClient.CancelBidAsync(new CancelBidRequest { EntryId = BidId });

        }
        catch (RpcException e)
        {
            ChatPanelUI.instance.SpawnMessage(MessageType.ERROR_LOG, "CANCELBID", e.Message);
        }
    }
}
