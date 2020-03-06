using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatisticsTabWindowUI : TabMenuWindowUI
{

    //player
    public TextMeshProUGUI GameServerBalanceText;
    public TextMeshProUGUI BufferBalanceText;
    public TextMeshProUGUI DeamonBalanceText;
    public TextMeshProUGUI LightingChannelCostText;
    public TextMeshProUGUI TotalBalanceText;
    public TextMeshProUGUI currentEarningsText;
    public TextMeshProUGUI currentBountyText;
    public TextMeshProUGUI currentKillsAndDeathsText;
    public TextMeshProUGUI lifeTimeKillsAndDeathsText;
    public TextMeshProUGUI lifeTimeEarningsText;
           
    //game 
    public TextMeshProUGUI currentPotText;
    public TextMeshProUGUI currentBountyOnPlayersText;
    public TextMeshProUGUI currentLootOnMapText;
    public TextMeshProUGUI totalGameSatsText;
    public TextMeshProUGUI totalPlayersLifeTimeKillsText;
    public TextMeshProUGUI allTimeMostKillsText;
    public TextMeshProUGUI totalPlayersLifetimeDeathsText;
    public TextMeshProUGUI allTimeMostDeathsText;
    public TextMeshProUGUI totalPlayersLifetimeEarnings;
    public TextMeshProUGUI allTimeMostEarnings;


    void Start()
    {
        //player
        ClientEvents.instance.onBalanceUpdate.AddListener(onBalanceUpdate);
        ClientEvents.instance.onBountyUpdate.AddListener(onBountyUpdate);
        ClientEvents.instance.onSessionEarningsUpdate.AddListener(onEarningsUpdate);

        ClientEvents.instance.onPlayerKillsAndDeathsUpdate.AddListener(onPlayerKillsAndDeathsUpdate);

        ClientEvents.instance.onPlayerLifeTimeKillsUpdate.AddListener(onPlayerLifeTimeKillsUpdate);
        ClientEvents.instance.onPlayerLifeTimeDeathsUpdate.AddListener(onPlayerLifeTimeDeathsUpdate);
        ClientEvents.instance.onPlayerLifeTimeEarningsUpdate.AddListener(onPlayerLifeTimeEarningsUpdate);

        //game
        ClientEvents.instance.onGlobalBountyUpdate.AddListener(onGlobalBountyUpdate);
        ClientEvents.instance.onGlobalLootUpdate.AddListener(onGlobalLootUpdate);
        ClientEvents.instance.onGlobalPotUpdate.AddListener(onGlobalPotUpdate);

        ClientEvents.instance.onAllTimeKillsUpdate.AddListener(onAllTimeKillsUpdate);
        ClientEvents.instance.onAllTimeMostKillsUpdate.AddListener(onAllTimeMostKillsUpdate);

        ClientEvents.instance.onAllTimeDeathsUpdate.AddListener(onAllTimeDeathsUpdate);
        ClientEvents.instance.onAllTimeMostDeathsUpdate.AddListener(onAllTimeMostDeathsUpdate);

        ClientEvents.instance.onAllTimeEarningsUpdate.AddListener(onAllTimeEarningsUpdate);
        ClientEvents.instance.onAllTimeMostEarningsUpdate.AddListener(onAllTimeMostEarningsUpdate);
    }

    //player
    void onBalanceUpdate(BalanceUpdateEventArgs args)
    {
        GameServerBalanceText.text = args.GameServerBalance + Utility.tintedSatsSymbol;
        BufferBalanceText.text = args.BufferBalance + Utility.tintedSatsSymbol;
        DeamonBalanceText.text = args.DeamonBalance + Utility.tintedSatsSymbol;
        if (args.ChannelCost <= 0) LightingChannelCostText.gameObject.SetActive(false);
        else
        {
            LightingChannelCostText.gameObject.SetActive(true);
            LightingChannelCostText.text = "-" + args.BufferBalance + Utility.tintedSatsSymbol;
        }

        long total = args.GameServerBalance + args.BufferBalance + args.DeamonBalance - args.ChannelCost;
        TotalBalanceText.text = total + Utility.tintedSatsSymbol;
        UITinter tinter = TotalBalanceText.GetComponent<UITinter>();
        if (total < 0) tinter.tint = TintColor.Error;
        else tinter.tint = TintColor.Primary;
        tinter.updateColor();
    }

    void onBountyUpdate(BountyUpdateEventArgs args)
    {
        currentBountyText.text = args.NewAmount + Utility.tintedSatsSymbol;
    }

    void onEarningsUpdate(SessionEarningsEventArgs args)
    {
        currentEarningsText.text = args.NewAmount + Utility.tintedSatsSymbol;
    }

    void onPlayerKillsAndDeathsUpdate(KillsAndDeathsUpdateEventArgs args)
    {
        currentKillsAndDeathsText.text = args.kills + " / " + args.deaths;
    }


    int killsLT = 0;
    int deathsLT = 0;
    void onPlayerLifeTimeKillsUpdate(int arg)
    {
        killsLT = arg;
        lifeTimeKillsAndDeathsText.text = killsLT + " / " + deathsLT;
    }
    void onPlayerLifeTimeDeathsUpdate(int arg)
    {
        deathsLT = arg;
        lifeTimeKillsAndDeathsText.text = killsLT + " / " + deathsLT;
    }

    void onPlayerLifeTimeEarningsUpdate(long arg) { lifeTimeEarningsText.text = Utility.SatsToShortString(arg, true); }


    //Game

    long globalBounty, globalLoot, globalPot = 0;
    void onGlobalBountyUpdate(long arg)
    {
        globalBounty = arg;
        currentBountyOnPlayersText.text = arg + Utility.tintedSatsSymbol;
        updateTotal();
    }
    void onGlobalLootUpdate(long arg)
    {
        globalLoot = arg;
        currentLootOnMapText.text = arg + Utility.tintedSatsSymbol;
        updateTotal();
    }
    void onGlobalPotUpdate(long arg) {
        globalPot = arg;
        currentPotText.text = arg + Utility.tintedSatsSymbol;
        updateTotal();
    }

    void updateTotal()
    {
        long total = globalBounty + globalLoot + globalPot;
        totalGameSatsText.text = total + Utility.tintedSatsSymbol;
    }

    void onAllTimeKillsUpdate (int arg){ totalPlayersLifeTimeKillsText.text = arg.ToString(); }
    void onAllTimeMostKillsUpdate (AllTimeScoreUpdateArgs args) { allTimeMostKillsText.text = args.score + " by " + args.name; }

    void onAllTimeDeathsUpdate(int arg){ totalPlayersLifetimeDeathsText.text = arg.ToString(); }
    void onAllTimeMostDeathsUpdate (AllTimeScoreUpdateArgs args) { allTimeMostDeathsText.text = args.score + " by " + args.name; }
    void onAllTimeEarningsUpdate(long arg) { totalPlayersLifetimeEarnings.text = Utility.SatsToShortString(arg,true); }
    void onAllTimeMostEarningsUpdate(AllTimeScoreUpdateArgs args) {  allTimeMostEarnings.text = Utility.SatsToShortString(args.score, true) + " by " + args.name; }


}
