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
        GameStatsReader.OnScoreboardUpdate += OnScoreboardUpdate;
        GameStatsReader.OnGainedKillEventEvent += OnKillEvent;
        sendScoreBoardEvent(GameStatsReader.Data.Scoreboard);
    }

    private void OnKillEvent(KillInfo obj)
    {
        var killer = idToName(obj.Killer);
        var victim = idToName(obj.Victim);
        // TODO implement UI;
        Debug.Log(killer + " killed " + victim);
    }

    private void OnScoreboardUpdate(Scoreboard obj)
    {
        sendScoreBoardEvent(obj);

    }
    private void sendScoreBoardEvent(Scoreboard obj)
    {
        List<ScoreboardUIItem> itemList = new List<ScoreboardUIItem>();

        foreach (ScoreboardItem i in obj.Board)
        {
            itemList.Add(new ScoreboardUIItem(idToName(i.Entity), i));
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

    public string idToName(EntityId id)
    {
        if (GameStatsReader == null)
        {
            Debug.LogError("No GameStatsReader found!");
            return "ERROR: GameStatsReader not found!";
        }

        if (GameStatsReader.Data.PlayerNames.ContainsKey(id))
        {
            return GameStatsReader.Data.PlayerNames[id];
        }
        return "";
    }
}
