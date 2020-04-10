using LightningAuction.Delivery;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DummyAuctionClient : MonoBehaviour, IAuctionClient
{
    [Header("AuctionEntry Response (Add Bid, GetBidStatus)")]
    [SerializeField]
    public AuctionEntry.Types.State AuctionEntyState;
    // Start is called before the first frame update
    void Start()
    {
        
    }



    public Task<AuctionEntry> AddBid(string message, long amount)
    {
        return Task.FromResult(GetAuctionEntry());
    }

    public void AddDonation(string message, long amount)
    {
        return;
    }

    public Task<AuctionEntry> BidStatus(string entryId)
    {
        return Task.FromResult(GetAuctionEntry());
    }

    public Task<Auction> GetActiveAuction()
    {
        throw new System.NotImplementedException();
    }

    public void CancelBid(string BidId)
    {
        return;
    }

    public void Setup()
    {
        
    }

    public void Shutdown()
    {
        
    }

    private AuctionEntry GetAuctionEntry()
    {
        return new AuctionEntry()
        {
            Amount = 100,
            Description = "description",
            Id = "id",
            Message = "message",
            PaymentRequest = "payment request",
            State = AuctionEntyState
        };
    }
}
