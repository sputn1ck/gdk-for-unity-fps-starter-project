using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Worker.CInterop;
using System.Linq;
using System;
using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.Core;
using Bountyhunt;
using Chat;
using Fps;
using Fps.Movement;

public class ClientGameChat : MonoBehaviour
{
    [Require] ChatComponentCommandSender ChatCommandSender;
    [Require] ChatComponentReader ChatReader;

    public static ClientGameChat instance;
    public string sendMessage;
    public bool triggerMessage;
    private void Awake()
    {
        if (instance != this)
            instance = this;
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        ChatReader.OnChatMessageEvent += ChatReader_OnChatMessageEvent;
    }

    private void ChatReader_OnChatMessageEvent(ChatMessage obj)
    {

        Debug.Log(obj.Sender + ": " + obj.Message);
        /*
        if (obj.Sender == "SERVER")
        {
            ChatPanelUI.instance.SpawnMessage(MessageType.INFO_LOG, obj.Sender, obj.Message);
        }
        else if (obj.Sender == "AUCTION_STARTED") {
            ClientEvents.instance.onNewAuctionStarted.Invoke();
        }
        else
        {
            ChatPanelUI.instance.SpawnMessage(MessageType.PLAYER_CHAT, obj.Sender, obj.Message);
        }
        */
        //ChatPanelUI.instance.SpawnMessage(obj);
        ClientEvents.instance.onChatMessageRecieve.Invoke(obj);
        if (obj.Sender == "AUCTION_STARTED")
        {
            ClientEvents.instance.onNewAuctionStarted.Invoke();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (triggerMessage)
        {
            SendChatMessage(sendMessage);
        }
    }

    public void SendChatMessage(string message)
    {
        if (message == null)
            return;
        ChatCommandSender.SendSendMessageCommand(new EntityId(2), new ChatRequest(FpsDriver.instance.GetComponent<BountyPlayerAuthorative>().HunterComponentReader.Data.Name, message));
    }
}
