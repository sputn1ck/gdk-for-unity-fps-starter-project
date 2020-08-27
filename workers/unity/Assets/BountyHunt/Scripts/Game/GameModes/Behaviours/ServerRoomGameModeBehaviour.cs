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

    [Require] RoomGameModeManagerWriter RoomGameModeManagerWriter;
    [Require] public RoomManagerWriter RoomManagerWriter;
    [Require] WorldManagerCommandSender WorldManagerCommandSender;
    [Require] RoomAdvertingManagerWriter RoomAdvertingManagerWriter;
    [Require] RoomGameModeManagerCommandReceiver RoomGameModeManagerCommandReceiver;
    [Require] public WorldCommandSender WorldCommandSender;
    [Require] public BountyTickComponentCommandSender BountyTickComponentCommandSender;



    public LinkedEntityComponent LinkedEntityComponent;
    public ServerRoomGameStatsMap ServerRoomGameStatsMap;
    public GameMode currentMode;
    private ModeRotationItem currentGameModeInfo;
    private TimeInfo currentTimeInfo;
    private UnityAction RotationFinished;
    private bool sendCallback;
    private bool rotationStarted = false;
    private BountyRoomSpawner BountyRoomSpawner;
    private RoomManagerServerBehaviour RoomManagerServerBehaviour;
    

    private bool inGameMode;
    private Map mapInfo;

    private void Awake()
    {
        LinkedEntityComponent = GetComponent<LinkedEntityComponent>();
        ServerRoomGameStatsMap = GetComponent<ServerRoomGameStatsMap>();
        BountyRoomSpawner = GetComponent<BountyRoomSpawner>();
        RoomManagerServerBehaviour = GetComponent<RoomManagerServerBehaviour>();
    }
    private void OnEnable()
    {
        RoomGameModeManagerCommandReceiver.OnAddBountyRequestReceived += RoomGameModeManagerCommandReceiver_OnAddBountyRequestReceived;
        RoomGameModeManagerCommandReceiver.OnAddKillRequestReceived += RoomGameModeManagerCommandReceiver_OnAddKillRequestReceived;
        if (RoomManagerWriter.Data.RoomState == RoomState.STARTED)
        {
            StartRotation();
        }
    }


    private void RoomGameModeManagerCommandReceiver_OnAddKillRequestReceived(RoomGameModeManager.AddKill.ReceivedRequest obj)
    {

        ServerRoomGameStatsMap.AddKill(obj.Payload.KillerId, obj.Payload.VictimId);
        if (currentMode is IKillGameMode)
        {
            (currentMode as IKillGameMode).PlayerKill(obj.Payload.KillerId,obj.Payload.VictimId,obj.Payload.Coordinates.ToUnityVector());
        }
    }

    private void RoomGameModeManagerCommandReceiver_OnAddBountyRequestReceived(RoomGameModeManager.AddBounty.ReceivedRequest obj)
    {

        ServerRoomGameStatsMap.AddBounty(obj.Payload.PlayerId, obj.Payload.Bounty);
    }

    public void Setup(Map mapInfo)
    {
        this.mapInfo = mapInfo;
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
        if (currentMode is IUpdateGameMode)
        {
            (currentMode as IUpdateGameMode).GameModeUpdate(Time.deltaTime);
            
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
        await GetNextGameMode();
        StartGameMode();
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
                //RoomGameModeManagerWriter.SendStartCountdownEvent(new CoundDownInfo(currentGameModeInfo.GamemodeId, 5, currentMode.Name));
                //await Task.Delay(5000);
                StartGameMode();

            }
            await Task.Delay(10);
        }
    }


    // TODO connect with backend
    private void StartGameMode()
    {
        Debug.LogFormat("starting gamemode {0} ",currentGameModeInfo.GamemodeId, RoomManagerWriter.Data.RoomInfo.GameModeInfo.CurrentMode);
        currentMode.ServerOnGameModeStart();
        var roundInfo = new RoundInfo(new GameModeInfo(currentGameModeInfo.GamemodeId, currentMode.name), new TimeInfo(DateTime.UtcNow.ToFileTime(), currentGameModeInfo.Duration));
        RoomGameModeManagerWriter.SendUpdate(new RoomGameModeManager.Update()
        {
            CurrentRound = roundInfo
        });
        RoomGameModeManagerWriter.SendNewRoundEvent(roundInfo);
        inGameMode = true;
        var room = RoomManagerWriter.Data.RoomInfo;
        room.GameModeInfo.CurrentModeStart = DateTime.UtcNow.ToFileTimeUtc();
        RoomManagerServerBehaviour.SendUpdates(room);
    }
    private async Task GetNextGameMode()
    {
        long subsidySats = 0;
        if (!RoomManagerWriter.Data.RoomInfo.FinanceInfo.FixedAdvertisers.HasValue)
        {
            // TODO multiserver safe
            var advertisers = await ServerServiceConnections.instance.BackendGameServerClient.GetAdvertisers(RoomManagerWriter.Data.RoomInfo.PlayerInfo.ActivePlayers.Count,0);
            SendAdvertisers(advertisers.Advertisers);
            subsidySats = advertisers.Subsidy;
        }
        var gameModeInfo = GetCurrentRound();
        var gameMode = Instantiate(GameModeDictionary.Get(gameModeInfo.GamemodeId));
        var settings = await ServerServiceConnections.instance.BackendGameServerClient.GetGameModeSettings(gameMode.GameModeId);
        gameMode.Initialize(settings, mapInfo, new GameModeFinancing() { totalSatAmount = subsidySats}, ServerRoomGameStatsMap, BountyRoomSpawner, LinkedEntityComponent.Worker.Origin);
        currentMode = gameMode;
        currentGameModeInfo = gameModeInfo;
    }

    private void EndGameMode()
    {
        inGameMode = false;
        Debug.Log("ending gamemode "+ currentGameModeInfo.GamemodeId);
        var gameModeInfo = GetCurrentRound();
        var timeInfo = RoomGameModeManagerWriter.Data.CurrentRound.TimeInfo;
        RoomGameModeManagerWriter.SendEndRoundEvent(new RoundInfo(new GameModeInfo("", gameModeInfo.GamemodeId), timeInfo));

        if(currentMode != null)
            currentMode.ServerOnGameModeEnd();
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
        RoomAdvertingManagerWriter.SendUpdate(new RoomAdvertingManager.Update()
        {
            CurrentAdvertisers = advertiserSources
        });
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
        var roomInfo = RoomManagerWriter.Data.RoomInfo.GameModeInfo;
        return Utility.GetCurrentRound(roomInfo.ModeRotation, roomInfo.CurrentMode);
    }


    private bool roationHasFinished()
    {
        if(currentMode == null)
        {
            return false;
        }

        var roomInfo = RoomManagerWriter.Data.RoomInfo.GameModeInfo;
        return Utility.roationHasFinished(roomInfo.ModeRotation, roomInfo.Repetitions, roomInfo.CurrentMode);
    }
    private void UpdateModeCounter()
    {
        var room = RoomManagerWriter.Data.RoomInfo;
        room.GameModeInfo.CurrentMode++;
        RoomManagerServerBehaviour.SendUpdates(room);
    }
    
}
