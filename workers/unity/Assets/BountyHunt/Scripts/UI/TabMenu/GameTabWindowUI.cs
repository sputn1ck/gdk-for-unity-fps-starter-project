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
    long donationAmount;

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
    long bidAmount;

    //Post Bounty
    public TMP_InputField PostBountyNameInput;
    public TMP_InputField PostBountyAmountInput;
    public Button PostBountyButton;
    long postBountyAmount;

    //Balance
    public TextMeshProUGUI BalanceText;
    long balance;
    
    public void Start()
    {
        donationAmount = long.Parse(donationAmountInput.text);
        bidAmount = long.Parse(auctionAmountInput.text);
        postBountyAmount = long.Parse(PostBountyAmountInput.text);
        UpdateDonationButton();
        UpdateBidButton();
        UpdatePostBountyButton();

        ClientEvents.instance.onNewAuctionStarted.AddListener(GetActiveAuction);
        donateButton.onClick.AddListener(Donate);
        bidButton.onClick.AddListener(Bid);
        PostBountyButton.onClick.AddListener(PostBounty);
        cancelButton.onClick.AddListener(CancelBid);
        statusButton.onClick.AddListener(BidStatus);
        ClientEvents.instance.onGameJoined.AddListener(GameJoined);
        //Invoke("GameJoined", 1f);
        ClientEvents.instance.onBalanceUpdate.AddListener(OnBalanceUpdate);

        donationAmountInput.onValueChanged.AddListener(OnUpdateDonationNumber);
        auctionAmountInput.onValueChanged.AddListener(OnUpdateBidNumber);
        PostBountyAmountInput.onValueChanged.AddListener(OnUpdatePostBountyNumber);
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

    //BALANCE
    public void OnBalanceUpdate(BalanceUpdateEventArgs args)
    {
        BalanceText.text = args.DaemonBalance + Utility.tintedSatsSymbol;
        balance = args.DaemonBalance;

        UpdateDonationButton();
        UpdateBidButton();
        UpdatePostBountyButton();

    }

    //DONATION
    public void Donate()
    {
        string message = donationMessageInput.text;

        PlayerServiceConnections.instance.AuctionClient.AddDonation(message, donationAmount);
    }

    void OnUpdateDonationNumber(string input)
    {
        if (input == "") donationAmount = 0;
        else donationAmount = long.Parse(input);
        UpdateDonationButton();
        
    }

    public void UpdateDonationButton()
    {
        if (donationAmount < 1 || donationAmount > balance)
        {
            donateButton.interactable = false;
        }
        else
        {
            donateButton.interactable = true;
        }
    }


    //POST BOUNTY
    public async void PostBounty()
    {
        string messageString;
        string messageColor;
        string name = PostBountyNameInput.text;
        string pubkey = ClientGameStats.instance.GetPlayerByName(name).Pubkey;

        
        if (String.IsNullOrEmpty(pubkey))
        {
            messageString = "player not found";
            messageColor = Utility.failureColorHex;
        }
        else
        {
            var res = await PlayerServiceConnections.instance.lnd.KeysendBountyIncrease(PlayerServiceConnections.instance.BackendPubkey,pubkey, postBountyAmount);
            if (res.PaymentError != "")
            {
                messageString = "payment failed!";
                messageColor = Utility.failureColorHex;

            }
            else
            {
                messageString = "succesfully increased " + name + "\'s bounty by " + postBountyAmount + Utility.tintedSatsSymbol;
                messageColor = Utility.successColorHex;
            }
        }
        ClientEvents.instance.onChatMessageRecieve.Invoke(new Chat.ChatMessage {Message = messageString, Sender = "Post Bounty", Type = Chat.MessageType.INFO_LOG,Color = messageColor});
        
    }

    void OnUpdatePostBountyNumber(string input)
    {
        if (input == "") postBountyAmount = 0;
        else postBountyAmount = long.Parse(input);
        UpdatePostBountyButton();
    }
    void UpdatePostBountyButton()
    {

        if (postBountyAmount < 1 || postBountyAmount > balance)
        {
            PostBountyButton.interactable = false;
        }
        else
        {
            PostBountyButton.interactable = true;
        }
    }


    //AUCTION
    public async void Bid()
    {
        var lastBid = currentBid;
        string message = auctionMessageInput.text;
        currentBid = await PlayerServiceConnections.instance.AuctionClient.AddBid(message, bidAmount);
        
        if(lastBid != null)
        {
            PlayerServiceConnections.instance.AuctionClient.CancelBid(lastBid.Id);
        }
        BidStatus();
    }

    void OnUpdateBidNumber(string input)
    {
        if (input == "") bidAmount = 0;
        else bidAmount = long.Parse(input);

        UpdateBidButton();
    }

    void UpdateBidButton()
    {
        if (bidAmount < 1 || bidAmount > balance)
        {
            bidButton.interactable = false;
        }
        else
        {
            bidButton.interactable = true;
        }
    }



    public async void BidStatus()
    {
        if (currentBid == null)
        {
            updateHighestBid(0);
            return;
        }
        currentBid = await PlayerServiceConnections.instance.AuctionClient.BidStatus(currentBid.Id);
        UpdateBidStatus();
    }

    public async void CancelBid()
    {
        if (currentBid == null)
            return;
        PlayerServiceConnections.instance.AuctionClient.CancelBid(currentBid.Id);
        BidStatus();
    }

    public async void GetActiveAuction()
    {
        var newAuction = await PlayerServiceConnections.instance.AuctionClient.GetActiveAuction();
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
    
}
