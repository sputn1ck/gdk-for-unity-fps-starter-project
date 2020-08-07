using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using System.Threading.Tasks;
using System;
using UnityEngine.Events;
using Improbable.Gdk.Core;

public class ServerRoomGameModeBehaviour : MonoBehaviour
{
    [Require] RoomStatsWriter roomStatsWriter;
    //[Require] AdvertisingComponentWriter advertisingConmponentWriter;
    [Require] RoomGameModeManagerWriter RoomGameModeManagerWriter;
    [Require] RoomManagerWriter RoomManagerWriter;
    [Require] WorldManagerCommandSender WorldManagerCommandSender;

    private GameMode currentMode;
    private ModeRotationItem currentGameModeInfo;
    private TimeInfo currentTimeInfo;
    private UnityAction RotationFinished;
    private bool sendCallback;
    private bool rotationStarted = false;
    private void OnEnable()
    {
        if(RoomManagerWriter.Data.RoomState == RoomState.STARTED)
        {
            StartRotation();
        }
    }
    public void StartRotation()
    {
        if (!rotationStarted)
        { 
            rotationStarted = true;
            SafeGameMode();
        }
    }

    public void AddFinishedAction(UnityAction onRotationFinished)
    {

        RotationFinished = onRotationFinished;
    }
    public void Update()
    {
        if (sendCallback)
        {
            sendCallback = false;
            RotationFinished?.Invoke();
        }
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
        Debug.Log("gamemode routine starting");
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
                UpdateModeCounter();
                //check if rotation is finished
                if (roationHasFinished())
                {
                    sendCallback = true;
                    return;
                }
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
        Debug.LogFormat("starting gamemode {0} ",currentGameModeInfo.GamemodeId, RoomManagerWriter.Data.RoomInfo.GameModeInfo.CurrentMode);
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
        var roundInfo = new RoundInfo(new GameModeInfo(currentGameModeInfo.GamemodeId, currentMode.name), new TimeInfo(DateTime.UtcNow.ToFileTime(), currentGameModeInfo.Duration));
        RoomGameModeManagerWriter.SendUpdate(new RoomGameModeManager.Update()
        {
            CurrentRound = roundInfo
        });
        RoomGameModeManagerWriter.SendNewRoundEvent(roundInfo);
    }
    private async Task GetNextGameMode()
    {

        /* Get Advertisers
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
        var timeInfo = RoomGameModeManagerWriter.Data.CurrentRound.TimeInfo;
        RoomGameModeManagerWriter.SendEndRoundEvent(new RoundInfo(new GameModeInfo("", gameModeInfo.GamemodeId), timeInfo));
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

        var gameModeInfo = RoomGameModeManagerWriter.Data.CurrentRound;
        
        var endTime = gameModeInfo.TimeInfo.StartTime + gameModeInfo.TimeInfo.Duration;

        
        return currentTime > endTime;
    }

    private ModeRotationItem GetCurrentRound()
    {
        // Get Room Index
        var roomInfo = RoomManagerWriter.Data.RoomInfo.GameModeInfo;
        int roomIndex;

        roomIndex = roomInfo.CurrentMode % roomInfo.ModeRotation.Count;



        return roomInfo.ModeRotation[roomIndex];
    }

    private bool roationHasFinished()
    {
        var roomInfo = RoomManagerWriter.Data.RoomInfo.GameModeInfo;
        var totalGameModes = roomInfo.ModeRotation.Count * roomInfo.Repetitions;
        if(roomInfo.CurrentMode > totalGameModes)
        {
            return true;
        }
        Debug.LogFormat("Checkinf if room should end: totalGameModes: {0} currentMode: {1}, result: {2}",totalGameModes, roomInfo.CurrentMode, roomInfo.CurrentMode >= totalGameModes);
        return false;
    }
    private void UpdateModeCounter()
    {
        var room = RoomManagerWriter.Data.RoomInfo;
        room.GameModeInfo.CurrentMode++;
        SendUpdates(room);
    }
    private void SendUpdates(Room room)
    {
        RoomManagerWriter.SendUpdate(new RoomManager.Update()
        {
            RoomInfo = room
        });
        WorldManagerCommandSender.SendUpdateRoomCommand(new EntityId(3), new UpdateRoomRequest()
        {
            Room = room
        });
    }
}
