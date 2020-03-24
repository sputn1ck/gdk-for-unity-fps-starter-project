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

    private void OnEnable()
    {
        instance = this;
        gameModeRotationCounter = 0;

        StartCoroutine(gameModeEnumerator());
    }

    private async void StartGameMode()
    {
        
        var gameMode = GameModeDictionary.Get(gameModeRotationCounter);
        currentGameMode = gameMode;

        // TODO Get Settings

        var roundInfo = await ServerServiceConnections.instance.BackendGameServerClient.GetRoundInfo(new Bbh.GetRoundInfoRequest { GameMode = (Bbh.GameMode)gameModeRotationCounter });
        currentGameMode.ServerOnGameModeStart(this, roundInfo.Settings, roundInfo.Subsidy);
        var RoundInfo = new RoundInfo()
        {
            GameModeInfo = new GameModeInfo(gameModeRotationCounter),
            TimeInfo = new TimeInfo()
            {
                StartTime = DateTime.UtcNow.ToFileTime(),
                Duration = gameMode.GameModeSettings.SecondDuration * 10000000,
            }
        };
        GameModeManagerWriter.SendUpdate(new GameModeManager.Update()
        {
            CurrentRound = RoundInfo
        });
        ServerGameChat.instance.SendGlobalMessage("server", gameMode.Name + " has started", MessageType.INFO_LOG);
        GameModeManagerWriter.SendNewRoundEvent(RoundInfo);
    }

    private void EndGameMode()
    {
        currentGameMode.ServerOnGameModeEnd(this);
        var gameMode = GameModeDictionary.Get(gameModeRotationCounter);
        ServerGameChat.instance.SendGlobalMessage("server", gameMode.Name + " has ended", MessageType.INFO_LOG);
    }
    private void SetNextGameMode()
    {
        nextGameModeId = getNextGameModeInt();
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
