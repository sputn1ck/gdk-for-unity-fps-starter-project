using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Improbable.Gdk.Subscriptions;

public class ServerRoundBehaviour : MonoBehaviour
{

    [Require] public GameModeManagerWriter GameModeManagerWriter;


    private int gameModeRotationCounter;


    private void OnEnable()
    {
        gameModeRotationCounter = 0;

        StartCoroutine(gameModeEnumerator());
    }

    private void StartGameMode()
    {

        var gameMode = GameModeRotation.Get(gameModeRotationCounter);
        Debug.Log("starting gamemode " + gameMode.Name);
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
    }
    private void SetNextGameMode()
    {
        var nextGameModeId = getNextGameModeInt();
        var gameMode = GameModeRotation.Get(nextGameModeId);
        Debug.Log("setting next gamemode " + gameMode.Name);
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
        return gameModeRotationCounter >= GameModeRotation.Count - 1 ? 0 : gameModeRotationCounter + 1;
    }

}
