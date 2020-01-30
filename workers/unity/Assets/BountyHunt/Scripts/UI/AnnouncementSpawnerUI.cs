using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnnouncementSpawnerUI : MonoBehaviour
{
    public AnnouncementMessageUI messagePrefab;
    public float animationDuration;

    List<AnnouncementMessageUI> activeMessages = new List<AnnouncementMessageUI>();
    List<AnnouncementMessageUI> inactiveMessages = new List<AnnouncementMessageUI>();


    private void Start()
    {
        ClientEvents.instance.onAnnouncement.AddListener(Announce);
    }

    public void Announce(string message, Color color)
    {

        AnnouncementMessageUI msg;

        if (inactiveMessages.Count == 0)
        {
            msg = Instantiate(messagePrefab,transform);
        }
        else
        {
            msg = inactiveMessages[0];
            inactiveMessages.Remove(msg);
        }
        msg.gameObject.SetActive(true);

        activeMessages.Add(msg);

        msg.play(message, color, animationDuration);

        StartCoroutine(deactivateMessageAfterTime(msg, animationDuration));
    }

    IEnumerator deactivateMessageAfterTime(AnnouncementMessageUI msg, float duration)
    {
        yield return new WaitForSeconds(duration);
        activeMessages.Remove(msg);
        inactiveMessages.Add(msg);
        msg.gameObject.SetActive(false);
    }

    [Header ("testing")]
    public string testMessage;
    public Color testColor;
    public bool test = false;
    public void AnnounceTest ()
    {
        ClientEvents.instance.onAnnouncement.Invoke(testMessage,testColor);
    }

    private void Update()
    {
        if (test)
        {
            test = false;
            AnnounceTest();
        }
    }

}
