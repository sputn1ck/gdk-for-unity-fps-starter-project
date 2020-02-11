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
        
        ClientEvents.instance.onSessionEarningsUpdate.AddListener(SessionUpdate);

        startTime = DateTime.Now;
    }

    private void UpdateBalance(BalanceUpdateEventArgs args)
    {
        BalanceUI.UpdateSats(args.NewAmount, args.NewAmount - args.OldAmount);
    }



    void SessionUpdate(SessionEarningsEventArgs e)
    {
        var secondsElapsed = DateTime.Now.Second - startTime.Second;
        var satsPerMinute = (e.NewAmount / (secondsElapsed / 60f));
        SatsPerMinUI.UpdateSats(satsPerMinute, 0);
    }

}
