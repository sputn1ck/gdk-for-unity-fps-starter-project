using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Chat;

public class ChatMessageUI : MonoBehaviour
{
    [HideInInspector]public ChatPanelUI chatPanel;
    public TextMeshProUGUI chatText;

    public MessageType type;
    public string sender;
    public string message;
    public Color color;

    bool disappear = false;
    float disappearTime;

    
    /// <param name="color">default Color if Color == Color.Clear</param>
    public void setMessage(MessageType type, string sender, string message,Color color)
    {
        this.type = type;
        this.sender = sender;
        this.message = message;
        this.color = color;

        updateMessage();
    }

    void updateMessage()
    {
        string text = "";
        if (sender != "")
        {
            text += "[" + sender + "]: ";
        }
        text += message;
        chatText.text = text;

        if (color == Color.clear)
        {
            chatText.color = chatPanel.GetColorFormLogType(type);
        }
        else chatText.color = color;


    }

    public void Activate()
    {
        disappear = false;
        gameObject.SetActive(true);

    }

    private void Deactivate()
    {
        disappear = false;
        gameObject.SetActive(false);
    }

    public void DeactivateAfterTime(float seconds)
    {
        disappear = true;
        disappearTime = Time.time+seconds;
    }

    private void Update()
    {
        if (disappear)
        {
            if(disappearTime < Time.time)
            {
                Deactivate();
            }
        }
    }
}
