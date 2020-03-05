using Bountyhunt;
using Improbable.Gdk.Subscriptions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientGameModeBehaviour : MonoBehaviour
{
    [Require] GameModeManagerReader GameModeManagerReader;

    private void OnEnable()
    {
        GameModeManagerReader.OnStartCountdownEvent += OnStartCountdown;
        GameModeManagerReader.OnCurrentRoundUpdate += OnCurrentRoundUpdate;
        ClientEvents.instance.onRoundUpdate.Invoke(new RoundUpdateEventArgs()
        {
           remainingTime = (float) getRoundSeconds(),
           gameMode = getCurrentGameMode(),
        });
    }

    private void OnCurrentRoundUpdate(RoundInfo obj)
    {
        ClientEvents.instance.onRoundUpdate.Invoke(new RoundUpdateEventArgs()
        {
            remainingTime = (float)getRoundSeconds(),
            gameMode = getCurrentGameMode(),
        });
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

    private double getRoundSeconds()
    {
        var startTime = GameModeManagerReader.Data.CurrentRound.TimeInfo.StartTime;
        var duration = GameModeManagerReader.Data.CurrentRound.TimeInfo.Duration;
        var now = DateTime.UtcNow;
        var diff = DateTime.FromFileTimeUtc(startTime + duration) - now;
        return diff.TotalSeconds;
    }

    private GameMode getCurrentGameMode()
    {
        var id = GameModeManagerReader.Data.CurrentRound.GameModeInfo.GameModeId;
        return GameModeDictionary.Get(id);
    }
}
