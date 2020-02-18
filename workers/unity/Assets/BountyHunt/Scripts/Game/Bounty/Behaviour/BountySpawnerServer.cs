using Fps.Respawning;
using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Commands;
using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Bountyhunt;
using Fps.Config;
using System.Linq;

[WorkerType(WorkerUtils.UnityGameLogic)]
public class BountySpawnerServer : MonoBehaviour
{
    [Require] WorldCommandSender WorldCommandSender;
    [Require] BountySpawnerCommandReceiver BountySpawnerCommandReceiver;
    [Require] HunterComponentCommandSender HunterComponentCommandSender;
    [Require] GameStatsReader GameStatsReader;

    public int spawnAmount;
    public bool spawnTrigger;
    private CancellationTokenSource cancellationToken;
    // Start is called before the first frame update
    void OnEnable()
    {
        cancellationToken = new CancellationTokenSource();
        BountySpawnerCommandReceiver.OnSpawnBountyPickupRequestReceived += OnSpawnBountyPickupRequestReceived;
        ServerEvents.instance.OnRandomInvoicePaid.AddListener(OnRandomInvoicePaid);
        ServerEvents.instance.OnAuctionInvoicePaid.AddListener(OnAuctionPaid);
        ServerEvents.instance.OnBountyInvoicePaid.AddListener(OnBountyInvoicePaid);
    }

    private void OnBountyInvoicePaid(BountyInvoice bounty)
    {
        var player = GameStatsReader.Data.PlayerMap.FirstOrDefault(u => u.Value.Pubkey == bounty.pubkey);
        if (player.Value.Name == null)
        {
            return;
        }
        HunterComponentCommandSender.SendAddBountyCommand(player.Key, new AddBountyRequest(bounty.amount, BountyReason.DONATION));
    }

    private void OnRandomInvoicePaid(string memo, long amount)
    {
        SpawnPickupAtRandomPosition(amount);
    }

    private void OnAuctionPaid(AuctionInvoice auction)
    {
        StartCoroutine(AuctionSpawning(auction.Amount, FlagManager.instance.GetSpawnsPerAuction(), FlagManager.instance.GetAuctionDuration()));
    }
    private void OnSpawnBountyPickupRequestReceived(BountySpawner.SpawnBountyPickup.ReceivedRequest obj)
    {
        if (obj.CallerAttributeSet[0] != WorkerUtils.UnityGameLogic)
            return;
        if(obj.Payload.BountyValue < 1)
            return;
        var v3 = obj.Payload.Position;
        var pos = new Vector3(v3.X, v3.Y, v3.Z);
        SpawnPickup(pos, obj.Payload.BountyValue);
    }

    private void Start()
    {

        StartCoroutine(SubsidyEnumerator());
    }
    // Update is called once per frame
    void Update()
    {
        if (spawnTrigger)
        {
            spawnTrigger = false;
            SpawnTick(spawnAmount);
        }
    }
    void SpawnPickup(Vector3 position, long sats)
    {
        var bountypickup = DonnerEntityTemplates.BountyPickup(position, sats);
        WorldCommandSender.SendCreateEntityCommand(new WorldCommands.CreateEntity.Request(bountypickup));
    }
    void SpawnPickupAtRandomPosition(long sats)
    {
        if (sats < 1)
            return;
        var pos = getRandomPosition();

        //pos.y = pos.y + 1f;
        var bountypickup = DonnerEntityTemplates.BountyPickup(pos, sats);
        WorldCommandSender.SendCreateEntityCommand(new WorldCommands.CreateEntity.Request(bountypickup));


    }

    private void SpawnTick(long satsPerTick, int maxSpawns = 20)
    {
        long totalSats = 0;
        long remainingSats = satsPerTick;
        long carrySats = satsPerTick % maxSpawns;
        long satsPerSpawn = (long)Mathf.Clamp((int)satsPerTick / (int)maxSpawns, 1, int.MaxValue);
        int spawns = 0;
        for (spawns = 0; spawns < maxSpawns; spawns++)
        {
            SpawnPickupAtRandomPosition(satsPerSpawn);
            //long nextSats = GetNextSats(remainingSats);
            remainingSats = remainingSats - satsPerSpawn;
            totalSats += satsPerSpawn;
            if (remainingSats < 1)
                break;
        }
        if (carrySats > 0 && satsPerTick > maxSpawns)
        {
            SpawnPickupAtRandomPosition(carrySats);
            totalSats += carrySats;
            spawns++;
        }
        

    }

    IEnumerator SubsidyEnumerator()
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (FlagManager.instance.GetSubsidizeGame())
            {
                var duration = FlagManager.instance.GetAuctionDuration();
                SpawnTick(FlagManager.instance.GetSubsidyPerMinute() / 2, UnityEngine.Random.Range(FlagManager.instance.GetMinSpawns(), FlagManager.instance.GetMaxSpawns()));
                //PrometheusManager.TotalSubsidy.Inc(FlagManager.instance.GetSubsidyPerMinute() / 2);
                yield return new WaitForSeconds(30f);
            }
            yield return null;
        }
        yield return null;

    }

    IEnumerator AuctionSpawning(long auctionAmount, int totalTicks, float totalTimeSeconds)
    {
        Debug.Log("AuctionSpawning started");
        int spawnCounter = 0;
        float timePerTickSeconds = totalTimeSeconds / totalTicks;
        long satsPerTick = auctionAmount / totalTicks;
        for (spawnCounter = 0; spawnCounter < totalTicks; spawnCounter++)
        {
            SpawnTick(satsPerTick, Random.Range(FlagManager.instance.GetMinSpawns(), FlagManager.instance.GetMaxSpawns()));
            yield return new WaitForSeconds(timePerTickSeconds);
        }
        Debug.Log("AuctionSpawning finished");
    }

    Vector3 getRandomPosition()
    {
        var pos = new Vector3(Random.Range(-140, 140), 50, Random.Range(-140, 140));
        pos = SpawnPoints.SnapToGround(pos);
        return pos;
    }

    public void OnApplicationQuit()
    {
        cancellationToken.Cancel();
    }
}
