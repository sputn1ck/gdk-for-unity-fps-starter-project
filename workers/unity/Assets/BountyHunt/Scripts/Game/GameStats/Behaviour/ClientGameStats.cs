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
        GameStatsReader.OnPlayerMapUpdate += OnPlayerMapUpdate;
        GameStatsReader.OnGainedKillEventEvent += OnKillEvent;
        sendScoreBoardEvent(GameStatsReader.Data.PlayerMap);
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
            var scoreboarditem = new ScoreboardItem()
            {
                Entity = i.Key,
                Bounty = i.Value.Bounty,
                Kills = i.Value.Kills,
                Deaths = i.Value.Deaths
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
