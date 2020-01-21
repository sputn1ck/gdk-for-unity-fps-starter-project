using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using LightningAuction.Delivery;
using System;

public class GameTabWindowUI : TabMenuWindowUI
{
    //Donation
    public TMP_InputField donationMessageInput;
    public TMP_InputField donationAmountInput;
    public Button donateButton;

    //Teleport
    public TMP_InputField teleportXInput;
    public TMP_InputField teleportYInput;
    public Button teleportButton;

    //Auction
    public TextMeshProUGUI highestBidText;
    public TextMeshProUGUI timeLeftText;
    public TextMeshProUGUI StatusText;
    public TMP_InputField auctionMessageInput;
    public TMP_InputField auctionAmountInput;
    public Button bidButton;
    public Button statusButton;
    public Button cancelButton;
    double timeUntilEnd = 0;
    DateTime AuctionEndTime;
    Auction currentAuction = null;
    AuctionEntry currentBid = null;

    //TODO
    /*
    public void Start()
    {

        ClientEvents.instance.onNewAuctionStarted.AddListener(GetActiveAuction);
        donateButton.onClick.AddListener(Donate);
        teleportButton.onClick.AddListener(Teleport);
        bidButton.onClick.AddListener(Bid);
        cancelButton.onClick.AddListener(CancelBid);
        statusButton.onClick.AddListener(BidStatus);
        Invoke("GameJoined", 1f);
        
    }

    public void GameJoined()
    {
        GetActiveAuction();
        InvokeRepeating("BidStatus", 2f, 10f);
    }
    private void Update()
    {
        UpdateAuctionTime();
        //Auction
        if (timeUntilEnd < 0)
            timeUntilEnd = 0;
        string auctionTimeLeft = SecondsToTimeString(timeUntilEnd);
        timeLeftText.text = "time left: " + auctionTimeLeft;
    }


    
    //DONATION
    public void Donate()
    {
        string message = donationMessageInput.text;
        long amount = long.Parse(donationAmountInput.text);

        AuctionClient.instance.AddDonation(message, amount);
    }

    //TELEPORT
    public void Teleport()
    {
        int x = int.Parse(teleportXInput.text);
        int y = int.Parse(teleportYInput.text);

        DonnerPlayerAuthorative.instance.SendTeleportRequest(new Vector2(x, y));

    }

    //AUCTION
    public async void Bid()
    {
        var lastBid = currentBid;
        string message = auctionMessageInput.text;
        long amount = long.Parse(auctionAmountInput.text);
        currentBid = await AuctionClient.instance.AddBid(message, amount);
        
        if(lastBid != null)
        {
            AuctionClient.instance.CancelBid(lastBid.Id);
        }
        BidStatus();
    }

    public async void BidStatus()
    {
        if (currentBid == null)
        {
            updateHighestBid(0);
            return;
        }
        currentBid = await AuctionClient.instance.BidStatus(currentBid.Id);
        UpdateBidStatus();
    }

    public async void CancelBid()
    {
        if (currentBid == null)
            return;
        AuctionClient.instance.CancelBid(currentBid.Id);
        BidStatus();
    }

    public async void GetActiveAuction()
    {
        var newAuction = await AuctionClient.instance.GetActiveAuction();
        if((currentAuction == null && newAuction != null ) || (newAuction != null  && currentAuction.Id != newAuction.Id))
        {
            currentAuction = newAuction;
            updateHighestBid(0);
            AuctionEndTime = DonnerUtils.UnixTimeToDateTime(currentAuction.StartedAt + currentAuction.Duration);
        }
    }
    public void UpdateAuctionTime()
    {
        //AuctionEndTime = Time.time + secondsLeft;
        var timeLeft = AuctionEndTime.Subtract(DateTime.UtcNow).TotalSeconds;
        timeUntilEnd = timeLeft;
    }

    public void updateHighestBid (long bidAmount)
    {
        highestBidText.text = "highest own bid: " + bidAmount + "<sprite=0>";
    }

    public void UpdateBidStatus()
    {
        if (currentBid == null)
            return;
        switch (currentBid.State)
        {
            case AuctionEntry.Types.State.AuctionentryAccepted:
                StatusText.text = "Status: Accepted";
                updateHighestBid(currentBid.Amount);
                break;
            case AuctionEntry.Types.State.AuctionentryCanceleD:
                StatusText.text = "Status: Canceled";
                updateHighestBid(0);
                break;
            case AuctionEntry.Types.State.AuctionentryOpen:
                StatusText.text = "Status: Open";
                updateHighestBid(0);
                break;
            case AuctionEntry.Types.State.AuctionentrySettled:
                StatusText.text = "Status: WON AUCTION!!";
                updateHighestBid(currentBid.Amount);
                break;

        }
    }

    public void resetAuction()
    {
        updateHighestBid(0);
        
    }

    private string SecondsToTimeString(double secs)
    {
        int s = (int)secs;
        int seconds = s % 60;
        int minutes = (s / 60) % 60;
        int hours = s / 3600;

        string secString = "" + seconds;
        if (seconds < 10) secString = "0" + seconds;
        string minString = "" + minutes;
        if (seconds < 10) minString = "0" + minutes;

        string timeString = hours + ":" + minString + ":" + secString;
        return timeString;
    }
    */
}
