using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
public class ClientGameStats : MonoBehaviour
{
    [Require] GameStatsReader GameStatsReader;
    
    void OnEnable()
    {
        GameStatsReader.OnScoreboardUpdate += OnScoreboardUpdate;    
    }

    private void OnScoreboardUpdate(Scoreboard obj)
    {
        Debug.Log(obj.Board[0].Bounty);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
