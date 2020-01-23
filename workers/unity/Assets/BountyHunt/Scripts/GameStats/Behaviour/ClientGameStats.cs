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
        ClientEvents.instance.onScoreboardUpdate.Invoke(obj.Board);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string idToName(EntityId id)
    {
        if (GameStatsReader.Data.PlayerNames.ContainsKey(id))
        {
            return GameStatsReader.Data.PlayerNames[id];
        }
        return "";
    }
}
