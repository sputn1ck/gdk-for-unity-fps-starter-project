using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Improbable.Gdk.Core;

public class ClientGameStats : MonoBehaviour
{
    [Require] GameStatsReader GameStatsReader;
    
    public static ClientGameStats instance;

    private void Awake()
    {
        instance = this;
    }

    void OnEnable()
    {

        initEvents();
        sendScoreBoardEvent(GameStatsReader.Data.PlayerMap);
        sendOutEvents();

        PlayerServiceConnections.instance.BackendPlayerClient.UpdateBackendStats(BountyPlayerAuthorative.instance.HunterComponentReader.Data.Pubkey);
    }

    private void initEvents()
    {
        GameStatsReader.OnPlayerMapUpdate += OnPlayerMapUpdate;
        GameStatsReader.OnGainedKillEventEvent += OnKillEvent;
        GameStatsReader.OnBountyInCubesUpdate += OnBountyInCubesUpdate;
        GameStatsReader.OnBountyOnPlayersUpdate += OnBountyInPlayersUpdate;
        GameStatsReader.OnCarryoverSatsUpdate += (long obj) =>
        {
            ClientEvents.instance.onCarryoverSatsUpdate.Invoke(obj);
            ClientEvents.instance.onGlobalPotUpdate.Invoke(obj);
        };
        GameStatsReader.OnRemainingPotUpdate += (long obj) =>
        {
            ClientEvents.instance.onRemainingPotUpdate.Invoke(obj);
            ClientEvents.instance.onGlobalPotUpdate.Invoke(obj);
        };
    }

    private void sendOutEvents()
    {
        var data = GameStatsReader.Data;
        ClientEvents.instance.onBountyinCubesUpdate.Invoke(data.BountyInCubes);
        ClientEvents.instance.onBountyInPlayersUpdate.Invoke(data.BountyOnPlayers);
        ClientEvents.instance.onGlobalPotUpdate.Invoke(data.RemainingPot);
        if(data.CarryoverSats == 0)
        {
            ClientEvents.instance.onGlobalPotUpdate.Invoke(data.CarryoverSats);
        }
    }
    private void OnBountyInPlayersUpdate(long obj)
    {
        ClientEvents.instance.onBountyInPlayersUpdate.Invoke(obj);
        ClientEvents.instance.onGlobalBountyUpdate.Invoke(obj);
    }

    private void OnBountyInCubesUpdate(long obj)
    {
        ClientEvents.instance.onBountyinCubesUpdate.Invoke(obj);
        ClientEvents.instance.onGlobalLootUpdate.Invoke(obj);
    }

    private void OnPlayerMapUpdate(Dictionary<EntityId, PlayerItem> obj)
    {
        sendScoreBoardEvent(obj);
    }

    private void OnKillEvent(KillInfo obj)
    {

        string killer = IdToName(obj.Killer);
        string victim = IdToName(obj.Victim);
        KillEventArgs args = new KillEventArgs { killer = killer, victim =  victim };
        ClientEvents.instance.onAnyKill.Invoke(args);
        Debug.Log(killer + " killed " + victim);
    }

    private void sendScoreBoardEvent(Dictionary<EntityId, PlayerItem> obj)
    {
        List<ScoreboardUIItem> itemList = new List<ScoreboardUIItem>();

        foreach (var i in obj)
        {
            Utility.Log(i.Value.Name + " earnings: " + i.Value.RoundEarnings, Color.cyan);
            var scoreboarditem = new ScoreboardItem()
            {
                Entity = i.Key,
                Bounty = i.Value.Bounty,
                Kills = i.Value.Kills,
                Deaths = i.Value.Deaths,
                Earnings = i.Value.RoundEarnings
            };
            itemList.Add(new ScoreboardUIItem(i.Value.Name, scoreboarditem));
        }
        EntityId playerID = new EntityId(-1);
        if (Fps.Movement.FpsDriver.instance != null)
        {
            playerID = Fps.Movement.FpsDriver.instance.getEntityID();
        }
        ClientEvents.instance.onScoreboardUpdate.Invoke(itemList, playerID);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string IdToName(EntityId id)
    {
        if (GameStatsReader == null)
        {
            Debug.LogError("No GameStatsReader found!");
            return "ERROR: GameStatsReader not found!";
        }

        if (GameStatsReader.Data.PlayerMap.ContainsKey(id))
        {
            return GameStatsReader.Data.PlayerMap[id].Name;
        }
        return "";
    }
}
