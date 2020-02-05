using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatsUI : MonoBehaviour
{
    public BountyUIController BalanceUI;
    public BountyUIController SatsPerMinUI;


    private DateTime startTime;
    // Start is called before the first frame update
    void Start()
    {
        ClientEvents.instance.onBalanceUpdate.AddListener(UpdateBalance);
        ClientEvents.instance.onPlayerSpawn.AddListener(PlayerSpawn);
        ClientEvents.instance.onSessionEarningsUpdate.AddListener(SessionUpdate);
    }

    private void UpdateBalance(BalanceUpdateEventArgs args)
    {
        BalanceUI.UpdateSats(args.NewAmount, args.NewAmount - args.OldAmount);
    }

    // Update is called once per frame
    void PlayerSpawn(GameObject player)
    {
        startTime = DateTime.Now;
    }

    void SessionUpdate(SessionEarningsEventArgs e)
    {
        var secondsElapsed = DateTime.Now.Second - startTime.Second;
        var satsPerMinute = (e.NewAmount / (secondsElapsed / 60f));
        SatsPerMinUI.UpdateSats(satsPerMinute, 0);
    }

}
