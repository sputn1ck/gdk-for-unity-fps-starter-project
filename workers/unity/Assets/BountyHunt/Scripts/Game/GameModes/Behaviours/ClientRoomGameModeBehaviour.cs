using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using System;

public class ClientRoomGameModeBehaviour : MonoBehaviour
{
    [Require] RoomGameModeManagerReader RoomGameModeManagerReader;

    private void OnEnable()
    {
        RoomGameModeManagerReader.OnNewRoundEvent += NewRound;
        RoomGameModeManagerReader.OnStartCountdownEvent += OnStartCountdown;
        SendGameModeEvent(RoomGameModeManagerReader.Data.CurrentRound);
    }

    private void NewRound(RoundInfo obj)
    {
        SendGameModeEvent(obj);
    }
    private void OnStartCountdown(CoundDownInfo obj)
    {

        var nextGameMode = GameModeDictionary.Get(obj.NextRoundId);
        var currentGameMode = GameModeDictionary.Get(RoomGameModeManagerReader.Data.CurrentRound.GameModeInfo.GameModeId);
        StartCoroutine(CountdownEnumerator(nextGameMode, obj.Countdown, currentGameMode, obj.NextRoundName));
    }
    IEnumerator CountdownEnumerator(GameMode nextGameMode, int duration, GameMode currentGameMode, string gamemodename)
    {
        var currentSecond = duration;
        while (currentSecond > 0)
        {
            ClientEvents.instance.onAnnouncement.Invoke(gamemodename + " starting in " + currentSecond, ChatPanelUI.instance.GetColorFormLogType(Chat.MessageType.DEBUG_LOG));
            yield return new WaitForSeconds(1f);
            currentSecond -= 1;
        }
        yield return new WaitForSeconds(0.1f);
        ClientEvents.instance.onAnnouncement.Invoke(gamemodename + " started", ChatPanelUI.instance.GetColorFormLogType(Chat.MessageType.DEBUG_LOG));

    }
    private void SendGameModeEvent(RoundInfo obj)
    {
        var gameMode = GameModeDictionary.Get(obj.GameModeInfo.GameModeId);
        ClientEvents.instance.onRoundUpdate.Invoke(new RoundUpdateEventArgs()
        {
            gameMode = gameMode,
            remainingTime = (float)getRoundSeconds(obj.TimeInfo)
        });
    }
    private double getRoundSeconds(TimeInfo timeInfo)
    {
        
        var startTime = timeInfo.StartTime;
        var duration = timeInfo.Duration;
        var now = DateTime.UtcNow;
        var diff = DateTime.FromFileTimeUtc(startTime + duration) - now;
        return diff.TotalSeconds;
    }
}
