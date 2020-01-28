using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chat;
using Improbable.Gdk.Subscriptions;

public class PlayerChat : MonoBehaviour
{
    [Require] PrivateChatCommandReceiver PrivateChatCommandReceiver;

    private void OnEnable()
    {
        PrivateChatCommandReceiver.OnSendMessageRequestReceived += PrivateChatCommandReceiver_OnSendMessageRequestReceived;
    }

    private void PrivateChatCommandReceiver_OnSendMessageRequestReceived(PrivateChat.SendMessage.ReceivedRequest obj)
    {
        ClientEvents.instance.onChatMessage.Invoke(obj.Payload);
        
    }
}
