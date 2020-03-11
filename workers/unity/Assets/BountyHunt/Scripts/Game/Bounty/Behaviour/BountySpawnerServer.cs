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
    [Require] GameStatsWriter GameStatsWriter;
    [Require] PaymentManagerComponentWriter PaymentManagerComponentWriter;

    // Start is called before the first frame update
    void OnEnable()
    {
        BountySpawnerCommandReceiver.OnSpawnBountyPickupRequestReceived += OnSpawnBountyPickupRequestReceived;
        BountySpawnerCommandReceiver.OnStartSpawningRequestReceived += OnStartSpawning;
        ServerEvents.instance.OnBountyInvoicePaid.AddListener(OnBountyInvoicePaid);
    }

    
    private void OnBountyInvoicePaid(BountyInvoice bounty)
    {
        var player = GameStatsWriter.Data.PlayerMap.FirstOrDefault(u => u.Value.Pubkey == bounty.pubkey);
        if (player.Value.Name == null)
        {
            return;
        }
        HunterComponentCommandSender.SendAddBountyCommand(player.Key, new AddBountyRequest(bounty.amount, BountyReason.DONATION));
        PaymentManagerComponentWriter.SendBountyIncreaseEvent(new BountyIncrease(bounty.message, player.Key.Id, bounty.amount));
    }
    private void OnStartSpawning(BountySpawner.StartSpawning.ReceivedRequest obj)
    {
        if (obj.CallerAttributeSet[0] != WorkerUtils.UnityGameLogic)
            return;
        GameStatsWriter.SendUpdate(new GameStats.Update
        {
            RemainingPot = obj.Payload.TotalBounty,
        });
        StartCoroutine(StartSpawning(obj.Payload));
    }
    IEnumerator StartSpawning(StartSpawningRequest request)
    {
        var totalBounty = request.TotalBounty ;
        
        var totalTicks = Mathf.FloorToInt(request.TotalDuration / request.TimeBetweenTicks);
        var remainingSats = totalBounty;
        var tickInfo = GetSatDistribution(totalTicks, totalBounty, request.Distribution);
        for (int i = 0; i < tickInfo.Length; i++)
        {
            SpawnTick(tickInfo[i], request.MinSpawns, request.MaxSpawns);
            remainingSats -= tickInfo[i];
            PrometheusManager.TotalSubsidy.Inc(tickInfo[i]);
            var data = GameStatsWriter.Data;
            GameStatsWriter.SendUpdate(new GameStats.Update() { BountyInCubes = data.BountyInCubes+tickInfo[i], RemainingPot = data.RemainingPot - tickInfo[i] });
            yield return new WaitForSeconds(request.TimeBetweenTicks);
        }
        yield return null;
    }



    private void OnSpawnBountyPickupRequestReceived(BountySpawner.SpawnBountyPickup.ReceivedRequest obj)
    {
        if (obj.CallerAttributeSet[0] != WorkerUtils.UnityGameLogic)
            return;
        if(obj.Payload.BountyValue < 1)
            return;
        var v3 = obj.Payload.Position;
        var pos = new Vector3(v3.X, v3.Y, v3.Z);
        GameStatsWriter.SendUpdate(new GameStats.Update()
        {
            BountyInCubes = GameStatsWriter.Data.BountyInCubes + obj.Payload.BountyValue
        });
        SpawnPickup(pos, obj.Payload.BountyValue);
    }

    // Update is called once per frame

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

    private void SpawnTick(long satsPerTick, int minSpawns, int maxSpawns)
    {
        Debug.LogFormat("spawning tick with {0} sats per tick {1} min spawns {2} max spawns ", satsPerTick, minSpawns, maxSpawns);
        long totalSats = 0;
        long remainingSats = satsPerTick;
        long carrySats = satsPerTick % maxSpawns;
        int plannedSpawns = Random.Range(minSpawns, maxSpawns);
        long satsPerSpawn = (long)Mathf.Clamp((int)satsPerTick / (int)plannedSpawns, 1, int.MaxValue);
        int spawns = 0;
        for (spawns = 0; spawns < maxSpawns; spawns++)
        {
            SpawnPickupAtRandomPosition(satsPerSpawn);
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


    Vector3 getRandomPosition()
    {
        var pos = new Vector3(Random.Range(-140, 140), 50, Random.Range(-140, 140));
        pos = SpawnPoints.SnapToGround(pos);
        return pos;
    }
    private long[] GetSatDistribution(int totalTicks, long totalSats, Distribution distribution)
    {
        switch (distribution) {
            case Distribution.UNIFORM:
                return GetUniformDistribution(totalTicks, totalSats);
            default:
                return GetUniformDistribution(totalTicks, totalSats);
        }
    }

    private long[] GetUniformDistribution(int totalTicks, long totalSats)
    {
        var tickInfo = new long[totalTicks];
        for (int i = 0; i < totalTicks; i++)
        {
            tickInfo[i] = (long)(totalSats / totalTicks);
        }
        return tickInfo;
    }
}
