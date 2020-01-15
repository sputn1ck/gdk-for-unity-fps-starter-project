using Fps.WorkerConnectors;
using Improbable.Gdk.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagManager : MonoBehaviour
{
    public static FlagManager instance;
    public bool defaultRunAuction;
    public int defaultAuctionDuration;
    public int defaultSpawnsPerAuction;
    public double defaultBountyPerTick;
    public int defualtTimePerBountyTick;
    public bool defaultSubsidizeGame;
    public int defaultSubsidyPerMinute;
    public long defaultChannelCost;
    public long defaultChannelSize;
    public int defaultTargetConf;
    
    private Worker worker;

    private const string auctionSeconds = "auction_seconds";
    private const string runAuctions = "run_auctions";
    private const string spawnsPerAuctionFlag = "spawns_per_auction";
    private const string bountyPerTickFlag = "bounty_per_tick";
    private const string timerPerBountyTickFlag = "time_per_bountyTick";
    private const string subsidizeGameFlag = "subsidize_game";
    private const string subSidyPerMinuteFlag = "subsidy_per_min";
    private const string lnChannelCostFlag = "ln_channel_cost";
    private const string lnTargetConfFlag = "ln_target_conf";
    private const string lnChannelSizeFlag = "ln_channel_size";
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<GameLogicWorkerConnector>().OnWorkerCreationFinished += FlagManager_OnWorkerCreationFinished;
        instance = this;
    }

    private void FlagManager_OnWorkerCreationFinished(Worker obj)
    {
        worker = obj;
        Debug.Log("worker initialized");
    }

    public int GetAuctionDuration()
    {
        int auctionDuration;
        if (int.TryParse(worker.GetWorkerFlag(auctionSeconds), out auctionDuration))
            return auctionDuration;
        return defaultAuctionDuration;
    }

    public bool GetShouldRunAuction()
    {
        bool runAuctionBool;
        if (bool.TryParse(worker.GetWorkerFlag(runAuctions), out runAuctionBool))
            return runAuctionBool;
        return defaultRunAuction;
    }

    public int GetSpawnsPerAuction()
    {
        int spawnsPerAuction;
        if (int.TryParse(worker.GetWorkerFlag(spawnsPerAuctionFlag), out spawnsPerAuction))
            return spawnsPerAuction;
        return defaultSpawnsPerAuction;
    }

    public double GetBountyPerTick()
    {
        double bountyPerTick;
        if (double.TryParse(worker.GetWorkerFlag(bountyPerTickFlag), out bountyPerTick))
            return bountyPerTick;
        return defaultBountyPerTick;
    }
    public int GetTimePerBountyTick()
    {
        int timePerBountyTick;
        if (int.TryParse(worker.GetWorkerFlag(timerPerBountyTickFlag), out timePerBountyTick))
            return timePerBountyTick;
        return defualtTimePerBountyTick;
    }
    public bool GetSubsidizeGame()
    {
        bool subsidizeGame;
        if (bool.TryParse(worker.GetWorkerFlag(subsidizeGameFlag), out subsidizeGame))
            return subsidizeGame;
        return defaultSubsidizeGame;
    }
    public int GetSubsidyPerMinute()
    {
        int subsidyPerMinute;
        if (int.TryParse(worker.GetWorkerFlag(subSidyPerMinuteFlag), out subsidyPerMinute))
            return subsidyPerMinute;
        return defaultSubsidyPerMinute;
    }
    public long GetChannelCost()
    {
        long channelCost;
        if (long.TryParse(worker.GetWorkerFlag(lnChannelCostFlag), out channelCost))
            return channelCost;
        return defaultChannelCost;
    }
    public int GetTargetConf()
    {
        int targetConf;
        if (int.TryParse(worker.GetWorkerFlag(lnTargetConfFlag), out targetConf))
            return targetConf;
        return defaultTargetConf;
    }
    public long GetChannelSize()
    {
        long channelSize;
        if (long.TryParse(worker.GetWorkerFlag(lnChannelSizeFlag), out channelSize))
            return channelSize;
        return defaultChannelSize;
    }
}
