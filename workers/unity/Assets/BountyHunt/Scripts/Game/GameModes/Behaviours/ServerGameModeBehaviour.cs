using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Chat;
using Grpc.Core;
using Improbable.Gdk.Subscriptions;
using Unity.Entities;
using System.Threading.Tasks;
using Bbhrpc;

public class ServerGameModeBehaviour : MonoBehaviour
{

    public static ServerGameModeBehaviour instance;
    [Require] public GameModeManagerWriter GameModeManagerWriter;
    [Require] public BountySpawnerCommandSender BountySpawnerCommandSender;
    [Require] public GameStatsWriter GameStatsWriter;


    [Require] AdvertisingComponentWriter advertisingConmponentWriter;
    public GameMode currentGameMode;
    public GameMode nextGameMode;
    public GetRoundInfoResponse currentRoundInfo;
    public GetRoundInfoResponse nextRoundInfo;
    private void OnEnable()
    {
        instance = this;

        //StartCoroutine(gameModeEnumerator());
        SafeGameMode();
    }

    private void StartGameMode()
    {

        currentRoundInfo = nextRoundInfo;
        currentGameMode = nextGameMode;
        currentGameMode.ServerOnGameModeStart(this, currentRoundInfo.Settings, currentRoundInfo.Subsidy);
        var RoundInfo = new RoundInfo()
        {
            GameModeInfo = new GameModeInfo(currentRoundInfo.GameModeId,nextRoundInfo.GameModeName),
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
        ServerGameChat.instance.SendGlobalMessage("server", currentRoundInfo.GameModeName + " has started", MessageType.INFO_LOG);
        GameModeManagerWriter.SendNewRoundEvent(RoundInfo);
    }
    public void SendAdvertisers(Google.Protobuf.Collections.RepeatedField<Bbhrpc.AdvertiserInfo> advertiserInfos)
    {
        List<AdvertiserSource> advertiserSources = new List<AdvertiserSource>();
        foreach(var advertiserInfo in advertiserInfos)
        {
            var advertiserSource = new AdvertiserSource
            {
                Hash = advertiserInfo.Phash,
                Investment = advertiserInfo.Sponsoring,
                Name = advertiserInfo.Name,
                SquareTextureLinks = new List<string>(),
                Url = advertiserInfo.Url
            };
            foreach(var imgUrl in advertiserInfo.SquareBannerUrls)
            {
                advertiserSource.SquareTextureLinks.Add(imgUrl);
            }
            advertiserSources.Add(advertiserSource);
        }
        advertisingConmponentWriter.SendUpdate(new AdvertisingComponent.Update() { CurrentAdvertisers = advertiserSources });
    }
    private void EndGameMode()
    {
        currentGameMode.ServerOnGameModeEnd(this);
        ServerGameChat.instance.SendGlobalMessage("server", currentGameMode.Name + " has ended", MessageType.INFO_LOG);
        var RoundInfo = new RoundInfo()
        {
            GameModeInfo = new GameModeInfo(currentGameMode.GameModeId,currentRoundInfo.GameModeName)
        };
        GameModeManagerWriter.SendEndRoundEvent(RoundInfo);
    }

    private async Task GetNextGameMode()
    {
        var roundInfo = await ServerServiceConnections.instance.BackendGameServerClient.GetRoundInfo(new Bbhrpc.GetRoundInfoRequest { PlayerInGame = GameStatsWriter.Data.PlayerMap.Count });
        if (roundInfo.Advertisers != null)
        {
            SendAdvertisers(roundInfo.Advertisers);
        }
        var gameMode = GameModeDictionary.Get(roundInfo.GameModeId);
        nextRoundInfo = roundInfo;
        nextGameMode = gameMode;

    }
    private async Task SafeGameMode()
    {
        try
        {
            await gameModeRoutine();
        } catch(Exception e)
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
        await GetNextGameMode();
        StartGameMode();
        while(!ServerServiceConnections.ct.IsCancellationRequested)
        {
            var endTime = GameModeManagerWriter.Data.CurrentRound.TimeInfo.StartTime +
                GameModeManagerWriter.Data.CurrentRound.TimeInfo.Duration;
            if (DateTime.UtcNow.ToFileTime() > endTime)
            {
                EndGameMode();
                await GetNextGameMode();
                GameModeManagerWriter.SendStartCountdownEvent(new CoundDownInfo(nextRoundInfo.GameModeId, 5,nextRoundInfo.GameModeName));
                await Task.Delay(5000);
                StartGameMode();

            }
            await Task.Delay(10);
        }
    }


}
