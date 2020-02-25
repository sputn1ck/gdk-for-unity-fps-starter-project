using Bountyhunt;
using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientGameModeBehaviour : MonoBehaviour
{
    [Require] GameModeManagerReader GameModeManagerReader;

    private void OnEnable()
    {
        GameModeManagerReader.OnStartCountdownEvent += OnStartCountdown;
    }

    private void OnStartCountdown(CoundDownInfo obj)
    {
        var gameMode = GameModeDictionary.Get(obj.NextRoundId);
        StartCoroutine(CountdownEnumerator(gameMode.Name, obj.Countdown));
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator CountdownEnumerator(string gameModeName, int duration)
    {
        var currentSecond = duration;
        while(currentSecond > 0)
        {
            ClientEvents.instance.onAnnouncement.Invoke(gameModeName + " starting in " + currentSecond, ChatPanelUI.instance.GetColorFormLogType(Chat.MessageType.DEBUG_LOG));
            yield return new WaitForSeconds(1f);
            currentSecond -= 1;
        }
        yield return new WaitForSeconds(0.1f);
        ClientEvents.instance.onAnnouncement.Invoke(gameModeName + " started", ChatPanelUI.instance.GetColorFormLogType(Chat.MessageType.DEBUG_LOG));

    }
}
