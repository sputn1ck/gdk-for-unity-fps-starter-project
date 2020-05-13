using Improbable.Gdk.Subscriptions;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chat;
using Improbable.Worker.CInterop;
using System.Linq;
using System;
using Improbable.Gdk.Core;
public class ServerGameChat : MonoBehaviour
{
    [Require] ChatComponentCommandReceiver ChatCommandReceiver;
    [Require] ChatComponentWriter ChatWriter;
    [Require] PrivateChatCommandSender privateChatCommandSender;

    public static ServerGameChat instance;
    //[Require] PlayerStateReaderSubscriptionManager PlayerState;
    // Start is called before the first frame update
    void OnEnable()
    {
        if (!Equals(instance, this))
            instance = this;
        ChatCommandReceiver.OnSendMessageRequestReceived += ChatCommandReceiver_OnSendMessageRequestReceived;
        ServerEvents.instance.OnBackendChatEvent.AddListener(OnBackendChatEvent);
    }
    private void OnDisable()
    {
        ServerEvents.instance.OnBackendChatEvent.RemoveListener(OnBackendChatEvent);
    }

    private void OnBackendChatEvent(Bbhrpc.ChatEvent e)
    {
        SendGlobalMessage(e.Sender, e.Message, MessageType.INFO_LOG, e.Announce);
    }

    private void ChatCommandReceiver_OnSendMessageRequestReceived(Chat.ChatComponent.SendMessage.ReceivedRequest obj)
    {
        Debug.Log("chat command received");
        ChatWriter.SendChatMessageEvent(new ChatMessage(DateTime.UtcNow.ToFileTimeUtc(), obj.EntityId.Id, obj.Payload.Name, obj.Payload.Message, MessageType.PLAYER_CHAT, false, ""));

    }

    public void SendServerChatMessage(string message, bool announce = false)
    {

        ChatWriter.SendChatMessageEvent(new ChatMessage(DateTime.UtcNow.ToFileTimeUtc(), 2, "SERVER", message, MessageType.INFO_LOG, announce,""));
    }

    public void SendAuctionStartedChatMessage(string message)
    {

        ChatWriter.SendChatMessageEvent(new ChatMessage(DateTime.UtcNow.ToFileTimeUtc(), 2, "AUCTION_STARTED", message, MessageType.AUCTION_LOG, false,""));
    }

    private void SendGlobalMessage(int id, string sender, string message, MessageType type, bool announce)
    {
        ChatMessage msg = new ChatMessage(DateTime.UtcNow.ToFileTimeUtc(), id, sender, message, type, announce,"");
        ChatWriter.SendChatMessageEvent(msg);
    }
    public void SendGlobalMessage(string sender, string message, MessageType type, bool announce = false)
    {
        SendGlobalMessage(2, sender, message, type, announce);
    }

    public void SendPrivateMessage(long recieverID, long senderID, string name, string message, Chat.MessageType type, bool announce)
    {
        ChatMessage msg = new ChatMessage(DateTime.UtcNow.ToFileTimeUtc(), senderID, name, message, type, announce,"");


        privateChatCommandSender.SendSendMessageCommand(new EntityId(recieverID), msg);
    }

    // Update is called once per frame


    public long testPlayerID;
    public string testPrivateMessage;
    public bool sendPrivateTestMessage;
    void Update()
    {
        if (sendPrivateTestMessage)
        {
            sendPrivateTestMessage = false;

            SendPrivateMessage(testPlayerID, 2, "", testPrivateMessage, MessageType.INFO_LOG, true);

        }

    }
}
