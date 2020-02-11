using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;
using Fps;
using Chat;

public class ChatPanelUI : MonoBehaviour
{
    public static ChatPanelUI instance;
    public int maxMessageCount;
    public ChatMessageUI chatMessagePrefab;
    public Transform MessagesContainer;

    public Color playerChatColor;
    public Color devLogColor;
    public Color systemLogColor;
    public Color debugLogColor;
    public Color actionLogColor;
    public InGameScreenManagerUI inGameScreenManager;


    List<ChatMessageUI> allMessages =  new List<ChatMessageUI>();

    public TMP_InputField chatInput;
    public float showMessageTime = 10f;
    public CanvasGroup InputCanvasGroup;
    public string placeHolderText = "Press T to chat...";
    bool justSubmittedChat;

    public bool chatting => chatInput.isFocused||justSubmittedChat;

    private void Awake()
    {
        if (instance != this)
            instance = this;
    }
    private void Start()
    {
        //DeactivateInputfield();
        chatInput.onSubmit.AddListener(Submit);
        ClientEvents.instance.onChatMessageRecieve.AddListener(SpawnMessage);
        ClientEvents.instance.onPaymentSucces.AddListener(PaymentSucces);
        ClientEvents.instance.onPaymentFailure.AddListener(PaymentFailuer);
    }

    public void PaymentSucces(PaymentSuccesArgs e)
    {
        SpawnMessage(MessageType.DEBUG_LOG, "PAYMENT SUCCESS", e.amount + " " + e.descripion);
    }

    public void PaymentFailuer(PaymentFailureArgs e)
    {
        SpawnMessage(MessageType.ERROR_LOG, "PAYMENT FAILURE", e.message);

    }
    public void SpawnMessage(MessageType type, string sender, string message)
    {
        if (!showLogType(type)) return;

        ChatMessageUI msg;

        if (allMessages.Count >= maxMessageCount)
        {
            msg = allMessages[0];
            allMessages.RemoveAt(0);
            msg.transform.SetAsLastSibling();
        }
        else
        {
            msg = Instantiate(chatMessagePrefab,MessagesContainer);
            msg.chatPanel = this;
            msg.transform.SetAsLastSibling();
        }

        msg.setMessage(type, sender, message);

        allMessages.Add(msg);
        msg.Activate();
        msg.DeactivateAfterTime(showMessageTime);

    }

    public void SpawnMessage(MessageType type, string sender, string message, bool announce)
    {
        SpawnMessage(type, sender, message);
        if (announce)
        {
            ClientEvents.instance.onAnnouncement.Invoke(message, GetColorFormLogType(type));
        }
    }

    public void SpawnMessage(ChatMessage message)
    {
        SpawnMessage(message.Type, message.Sender, message.Message, message.ShowAnnouncement);
    }

    public Color GetColorFormLogType(MessageType type)
    {
        switch (type)
        {
            case MessageType.PLAYER_CHAT:
                return playerChatColor;
            case MessageType.ERROR_LOG:
                return devLogColor;
            case MessageType.INFO_LOG:
                return systemLogColor;
            case MessageType.DEBUG_LOG:
                return debugLogColor;
            case MessageType.AUCTION_LOG:
                return actionLogColor;
            default:
                return Color.magenta;
        }
    }

    public void StartChatInput()
    {
        ActivateInputfield();
        ActivateAllMessages();
    }


    public void Submit(string msg)
    {
        chatInput.text = "";
        justSubmittedChat = true;
        StartCoroutine(resetJustSubmitted());
        InputCanvasGroup.alpha = 0;
        InputCanvasGroup.blocksRaycasts = false;
        InputCanvasGroup.interactable = false;
        if (chatInput.wasCanceled) return;
        if (msg == "") return;
        ClientGameChat.instance.SendChatMessage(msg);
    }

    IEnumerator resetJustSubmitted()
    {
        yield return new WaitForEndOfFrame();
        justSubmittedChat = false;
    }

    public void ActivateAllMessages()
    {
        foreach (ChatMessageUI msg in allMessages)
        {
            msg.Activate();
        }
    }

    public void DeactivateAllMessagesAfterTime()
    {
        foreach (ChatMessageUI msg in allMessages)
        {
            msg.DeactivateAfterTime(showMessageTime);
        }
    }

    void ActivateInputfield()
    {

        InputCanvasGroup.alpha = 1;
        InputCanvasGroup.interactable = true;
        chatInput.Select();
        chatInput.ActivateInputField();
        chatInput.placeholder.GetComponent<TextMeshProUGUI>().text = "";

    }

    static bool showLogType(MessageType type)
    {
        switch (type)
        {
            case MessageType.PLAYER_CHAT:
                return PlayerPrefs.GetInt("ShowPlayerChat", 1) != 0;
                break;
            case MessageType.ERROR_LOG:
                return PlayerPrefs.GetInt("ShowErrorLog", 1) != 0;
                break;
            case MessageType.INFO_LOG:
                return PlayerPrefs.GetInt("ShowInfoLog", 1) != 0;
                break;
            case MessageType.DEBUG_LOG:
                return PlayerPrefs.GetInt("ShowDebugLog", 0) != 0;
                break;
            case MessageType.AUCTION_LOG:
                return PlayerPrefs.GetInt("ShowInfoLog", 1) != 0;
                break;
            default:
                return true;
                break;
        }
    }



}
