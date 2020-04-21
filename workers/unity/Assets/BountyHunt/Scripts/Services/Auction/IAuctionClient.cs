using LightningAuction.Delivery;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IAuctionClient
{
    Task<AuctionEntry> AddBid(string message, long amount);
    void AddDonation(string message, long amount);
    Task<AuctionEntry> BidStatus(string entryId);
    Task<Auction> GetActiveAuction();
    void CancelBid(string BidId);
    Task Setup();

    void Shutdown();
}

public interface IAuctionController
{
    void Shutdown();
    void Setup();

    Task<StartAuctionResponse> StartAuction(int duration);
}
