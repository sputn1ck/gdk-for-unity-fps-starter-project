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
        GameModeManagerReader.OnCurrentRoundUpdate += GameModeManagerReader_OnCurrentRoundUpdate;
        // 
    }

    private void GameModeManagerReader_OnCurrentRoundUpdate(RoundInfo obj)
    {
        Debug.Log("midgame update");
    }

    private void OnStartCountdown(CoundDownInfo obj)
    {
        var gameMode = GameModeDictionary.Get(obj.NextRoundId);
        StartCoroutine(CountdownEnumerator(gameMode.Name, obj.Countdown));
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
