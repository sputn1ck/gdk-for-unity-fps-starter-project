using LightningAuction.Delivery;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DummyAuctionController : MonoBehaviour, IAuctionController
{

    public void Shutdown()
    {

    }

    public void Setup()
    {

    }

    public Task<StartAuctionResponse> StartAuction(int duration)
    {
        return Task.FromResult(new StartAuctionResponse() { Auction = new Auction { Id = "", Duration = 100, StartedAt = (int)System.DateTime.UtcNow.ToFileTime() } });
    }
}
