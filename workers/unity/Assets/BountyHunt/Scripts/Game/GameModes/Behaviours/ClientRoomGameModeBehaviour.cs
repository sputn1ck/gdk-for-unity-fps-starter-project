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
        SendGameModeEvent(RoomGameModeManagerReader.Data.CurrentRound);
    }

    private void NewRound(RoundInfo obj)
    {
        SendGameModeEvent(obj);
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
