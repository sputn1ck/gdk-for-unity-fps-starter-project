using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using System.Threading.Tasks;
using System;

public class ServerRoomGameModeBehaviour : MonoBehaviour
{
    [Require] RoomStatsWriter roomStatsWriter;
    //[Require] AdvertisingComponentWriter advertisingConmponentWriter;
    [Require] RoomGameModeManagerWriter RoomGameModeManagerWriter;
    [Require] RoomManagerWriter RoomManagerWriter;

    private GameMode currentMode;
    private ModeRotationItem currentGameModeInfo;
    private void OnEnable()
    {

    }
    public void StartRotation()
    {
        SafeGameMode();
    }
    private async Task SafeGameMode()
    {
        try
        {
            await gameModeRoutine();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            if (ServerServiceConnections.ct.IsCancellationRequested)
            {
                return;
            }
            await Task.Delay(1000);
            SafeGameMode();
        }
    }
    private async Task gameModeRoutine()
    {
        /*await GetNextGameMode();
        StartGameMode();
        while (!ServerServiceConnections.ct.IsCancellationRequested)
        {
            var endTime = GameModeManagerWriter.Data.CurrentRound.TimeInfo.StartTime +
                GameModeManagerWriter.Data.CurrentRound.TimeInfo.Duration;
            if (DateTime.UtcNow.ToFileTime() > endTime)
            {
                EndGameMode();
                await GetNextGameMode();
                GameModeManagerWriter.SendStartCountdownEvent(new CoundDownInfo(nextRoundInfo.GameModeId, 5, nextRoundInfo.GameModeName));
                await Task.Delay(5000);
                StartGameMode();

            }
            await Task.Delay(10);
        }*/
        while (!ServerServiceConnections.ct.IsCancellationRequested)
        {
            if (gameModeHasEnded(DateTime.UtcNow.ToFileTime()))
            {
                // ends game mode
                EndGameMode();
                // sets next game mode;
              
                await GetNextGameMode();
                RoomGameModeManagerWriter.SendStartCountdownEvent(new CoundDownInfo(currentGameModeInfo.GamemodeId, 5, currentMode.Name));
                await Task.Delay(5000);
                StartGameMode();

            }
            await Task.Delay(10);
        }
    }

    // TODO connect with backend
    private void StartGameMode()
    {
        Debug.Log("starting gamemode " + currentGameModeInfo.GamemodeId);
        /*
        currentGameMode.ServerOnGameModeStart(this, currentRoundInfo.Settings, currentRoundInfo.Subsidy);
        var RoundInfo = new RoundInfo()
        {
            GameModeInfo = new GameModeInfo(currentRoundInfo.GameModeId, nextRoundInfo.GameModeName),
            TimeInfo = new TimeInfo()
            {
                StartTime = DateTime.UtcNow.ToFileTime(),
                Duration = currentRoundInfo.Settings.SecondDuration * 10000000,
            }
        };
        GameModeManagerWriter.SendUpdate(new GameModeManager.Update()
        {
            CurrentRound = RoundInfo
        });
        ServerGameChat.instance.SendGlobalMessage("server", currentRoundInfo.GameModeName + " has started", MessageType.INFO_LOG);*/
        RoomGameModeManagerWriter.SendNewRoundEvent(new RoundInfo(new GameModeInfo(currentGameModeInfo.GamemodeId, currentMode.name), currentGameModeInfo.TimeInfo));
    }
    private async Task GetNextGameMode()
    {
        UpdateModeCounter();
        /*
        var roundInfo = await ServerServiceConnections.instance.BackendGameServerClient.GetRoundInfo(new Bbhrpc.GetRoundInfoRequest { PlayerInGame = RoomManagerWriter.Data.RoomInfo.ActivePlayers.Count });
        if (roundInfo.Advertisers != null)
        {
            SendAdvertisers(roundInfo.Advertisers);
        }*/
        var gameModeInfo = GetCurrentRound();
        var gameMode = GameModeDictionary.Get(gameModeInfo.GamemodeId);
        currentMode = gameMode;
        currentGameModeInfo = gameModeInfo;
    }

    private void EndGameMode()
    {
        Debug.Log("ending gamemode "+ currentGameModeInfo.GamemodeId);
        var gameModeInfo = GetCurrentRound();
        RoomGameModeManagerWriter.SendEndRoundEvent(new RoundInfo(new GameModeInfo("", gameModeInfo.GamemodeId), gameModeInfo.TimeInfo));
        //currentMode.ServerOnGameModeEnd(null);
    }
    public void SendAdvertisers(Google.Protobuf.Collections.RepeatedField<Bbhrpc.AdvertiserInfo> advertiserInfos)
    {
        List<AdvertiserSource> advertiserSources = new List<AdvertiserSource>();
        foreach (var advertiserInfo in advertiserInfos)
        {
            var advertiserSource = new AdvertiserSource
            {
                Hash = advertiserInfo.Phash,
                Investment = advertiserInfo.Sponsoring,
                Name = advertiserInfo.Name,
                SquareTextureLinks = new List<string>(),
                Url = advertiserInfo.Url
            };
            foreach (var imgUrl in advertiserInfo.SquareBannerUrls)
            {
                advertiserSource.SquareTextureLinks.Add(imgUrl);
            }
            advertiserSources.Add(advertiserSource);
        }
        //advertisingConmponentWriter.SendUpdate(new AdvertisingComponent.Update() { CurrentAdvertisers = advertiserSources });
    }

    private bool gameModeHasEnded( long currentTime)
    {
        
        var gameModeInfo = GetCurrentRound();
        var endTime = gameModeInfo.TimeInfo.StartTime + gameModeInfo.TimeInfo.Duration;

        return currentTime > endTime;
    }

    private ModeRotationItem GetCurrentRound()
    {
        // Get Room Index
        var roomInfo = RoomManagerWriter.Data.RoomInfo;
        int roomIndex;

        roomIndex = roomInfo.CurrentMode % roomInfo.ModeRotation.Count;



        return roomInfo.ModeRotation[roomIndex];
    }

    private void UpdateModeCounter()
    {
        var room = RoomManagerWriter.Data.RoomInfo;
        room.CurrentMode++;
        RoomManagerWriter.SendUpdate(new RoomManager.Update() { RoomInfo = room });
    }
}
