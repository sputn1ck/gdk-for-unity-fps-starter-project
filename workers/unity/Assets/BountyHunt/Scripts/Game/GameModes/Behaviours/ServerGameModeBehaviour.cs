using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Chat;
using Grpc.Core;
using Improbable.Gdk.Subscriptions;
using Unity.Entities;

public class ServerGameModeBehaviour : MonoBehaviour
{

    public static ServerGameModeBehaviour instance;
    [Require] public GameModeManagerWriter GameModeManagerWriter;
    [Require] public BountySpawnerCommandSender BountySpawnerCommandSender;
    [Require] public GameStatsWriter GameStatsWriter;

    private int gameModeRotationCounter;
    public GameMode currentGameMode;
    private int nextGameModeId;
    private ServerGameStats ServerGameStats;

    private void OnEnable()
    {
        instance = this;
        gameModeRotationCounter = 0;

        StartCoroutine(gameModeEnumerator());
        ServerGameStats = GetComponent<ServerGameStats>();
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
        currentGameMode = gameMode;
        currentGameMode.OnGameModeStart(this);
        GameModeManagerWriter.SendUpdate(new GameModeManager.Update()
        {
            CurrentRound = RoundInfo
        });
        ServerGameChat.instance.SendGlobalMessage("server", gameMode.Name + " has started", MessageType.INFO_LOG);
        GameModeManagerWriter.SendNewRoundEvent(RoundInfo);
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
        nextGameModeId = getNextGameModeInt();
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
            }
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
                GameModeManagerWriter.SendStartCountdownEvent(new CoundDownInfo(nextGameModeId, 5));
                yield return new WaitForSeconds(5f);
                ServerGameStats.ResetScoreboard();
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
