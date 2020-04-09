using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.Core;
using Fps;
using Fps.Movement;

public class ClientPaymentManager : MonoBehaviour
{

    [Require] private PaymentManagerComponentReader PaymentManagerComponentReader;

    private void OnEnable()
    {

        PaymentManagerComponentReader.OnAuctionWinnerUpdate += OnAuctionWinner;
        PaymentManagerComponentReader.OnBountyIncreaseEvent += OnBountyIncrease;
        PaymentManagerComponentReader.OnRandomInvoicePaidEvent += OnRandomInvoicePaidEvent;
        ClientEvents.instance.onAuctionMessageUpdate.Invoke(PaymentManagerComponentReader.Data.AuctionWinner.Text, PaymentManagerComponentReader.Data.AuctionWinner.Sats);
    }

    private void OnRandomInvoicePaidEvent(NewWinnerMessage obj)
    {
        ClientEvents.instance.onChatMessageRecieve.Invoke(new Chat.ChatMessage()
        {
            EntityId = 2,
            Timestamp = 0,
            Sender = "Donation",
            Message =obj.Text + ";"+ obj.Sats +" sats",
            Type = Chat.MessageType.INFO_LOG
        });
    }

    private void OnBountyIncrease(BountyIncrease obj)
    {

        if(obj.ReceiverId == FpsDriver.instance.getEntityID().Id)
        {
            ClientEvents.instance.onChatMessageRecieve.Invoke(new Chat.ChatMessage()
            {
                EntityId = 2,
                Timestamp = 0,
                Sender = "Payments",
                Message = "bounty increased by " + obj.Sats + " sats",
                Type = Chat.MessageType.INFO_LOG,
                ShowAnnouncement = true
            });
        } else
        {

            var player = ClientGameStats.instance.GetPlayerByID(new EntityId(obj.ReceiverId));
            ClientEvents.instance.onChatMessageRecieve.Invoke(new Chat.ChatMessage()
            {
                EntityId = 2,
                Timestamp = 0,
                Sender = "Payments",
                Message = player.Name + " bounty was increased by " + obj.Sats + "sats",
                Type = Chat.MessageType.INFO_LOG
            });
        }
    }

    private void OnAuctionWinner(NewWinnerMessage obj)
    {
        ClientEvents.instance.onAuctionMessageUpdate.Invoke(obj.Text, obj.Sats);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
