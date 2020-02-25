using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillFeedUI : MonoBehaviour
{
    public static KillFeedUI instance;
    public KillFeedMessageUI feedMessagePrefab;

    List<KillFeedMessageUI> inactiveMessages;
    List<KillFeedMessageUI> activeMessages;

    public int maxMessageCount;

    [Space(20)]
    public string testMessage = "Max killed Moritz";
    public bool sendTest = false;


    void Awake()
    {
        instance = this;
        inactiveMessages = new List<KillFeedMessageUI>();
        activeMessages = new List<KillFeedMessageUI>();
    }

    public void NewMessage(string msg)
    {
        KillFeedMessageUI feedMessage;
        if (inactiveMessages.Count > 0)
        {
            feedMessage = inactiveMessages[0];
            inactiveMessages.Remove(feedMessage);
        }
        else feedMessage = Instantiate(feedMessagePrefab,transform);

        activeMessages.Add(feedMessage);
        feedMessage.gameObject.SetActive(true);
        feedMessage.SetNew(msg);

        if(activeMessages.Count > maxMessageCount)
        {
            activeMessages[0].Hide();
        }

    }

    //toCall From corresponding KillFeedMessageUI
    public void AddMessageToInactive(KillFeedMessageUI feedMessage)
    {
        feedMessage.gameObject.SetActive(false);
        inactiveMessages.Add(feedMessage);
    }

    //toCall From corresponding KillFeedMessageUI
    public void RemoveMessageFromActive(KillFeedMessageUI feedMessage)
    {
        activeMessages.Remove(feedMessage);
    }


    private void Update()
    {
        if (sendTest)
        {
            sendTest = false;
            NewMessage(testMessage);
        }
    }

}
