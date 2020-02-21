using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Chat;
using Improbable.Gdk.Subscriptions;
using Unity.Entities;

public class ServerGameModeBehaviour : MonoBehaviour
{

    [Require] public GameModeManagerWriter GameModeManagerWriter;

    private int gameModeRotationCounter;
    private GameMode currentGameMode;


    private void OnEnable()
    {
        gameModeRotationCounter = 0;

        StartCoroutine(gameModeEnumerator());

    }

    private void StartGameMode()
    {

        var gameMode = GameModeDictionary.Get(gameModeRotationCounter);
        var RoundInfo = new RoundInfo()
        {
            GameModeInfo = new GameModeInfo(gameModeRotationCounter),
            TimeInfo = new TimeInfo()
            {
                StartTime = DateTime.UtcNow.ToFileTime(),
                Duration = gameMode.GlobalSettings.NanoSeconds,
            }
        };

        GameModeManagerWriter.SendUpdate(new GameModeManager.Update()
        {
            CurrentRound = RoundInfo
        });

        GameModeManagerWriter.SendNewRoundEvent(RoundInfo);
        currentGameMode = gameMode;
        currentGameMode.OnGameModeStart(this);
        ServerGameChat.instance.SendGlobalMessage("server", gameMode.Name + " has started", MessageType.INFO_LOG);
    }

    private void EndGameMode()
    {
        currentGameMode.OnGameModeEnd(this);
        var gameMode = GameModeDictionary.Get(gameModeRotationCounter);
        var RoundInfo = new RoundInfo()
        {
            GameModeInfo = new GameModeInfo(gameModeRotationCounter),
            TimeInfo = new TimeInfo()
            {
                StartTime = DateTime.UtcNow.ToFileTime(),
                Duration = gameMode.GlobalSettings.NanoSeconds,
            }
        };
        GameModeManagerWriter.SendEndRoundEvent(RoundInfo);
        ServerGameChat.instance.SendGlobalMessage("server", gameMode.Name + " has ended", MessageType.INFO_LOG);
    }
    private void SetNextGameMode()
    {
        var nextGameModeId = getNextGameModeInt();
        var gameMode = GameModeDictionary.Get(nextGameModeId);
        GameModeManagerWriter.SendUpdate(new GameModeManager.Update()
        {
            NextRound = new RoundInfo()
            {
                GameModeInfo = new GameModeInfo(nextGameModeId),
                TimeInfo = new TimeInfo()
                {
                    StartTime = GameModeManagerWriter.Data.CurrentRound.TimeInfo.StartTime + GameModeManagerWriter.Data.CurrentRound.TimeInfo.Duration,
                    Duration = gameMode.GlobalSettings.NanoSeconds,
                }
            },
        });
    }

    private IEnumerator gameModeEnumerator()
    {
        StartGameMode();
        SetNextGameMode();
        while (!ServerServiceConnections.ct.IsCancellationRequested)
        {
            var endTime = GameModeManagerWriter.Data.CurrentRound.TimeInfo.StartTime +
                GameModeManagerWriter.Data.CurrentRound.TimeInfo.Duration;
            if (DateTime.UtcNow.ToFileTime() > endTime)
            {
                EndGameMode();
                //Todo send out starts in event
                yield return new WaitForSeconds(5f);
                gameModeRotationCounter = getNextGameModeInt();
                StartGameMode();
                SetNextGameMode();
            }

            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    private int getNextGameModeInt()
    {
        return gameModeRotationCounter >= GameModeDictionary.Count - 1 ? 0 : gameModeRotationCounter + 1;
    }

}
