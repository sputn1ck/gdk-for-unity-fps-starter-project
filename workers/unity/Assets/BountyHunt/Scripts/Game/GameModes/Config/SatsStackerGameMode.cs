using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Improbable.Gdk.Core;
using Bountyhunt;
using Fps.Respawning;
using Bbhrpc;

[CreateAssetMenu(fileName = "SatsStackerGameMode", menuName = "BBH/GameModes/SatsStacker", order = 2)]
public class SatsStackerGameMode : GameMode
{
    private ServerRoomGameModeBehaviour _serverGameModeBehaviour;
    private BountyRoomSpawner BountyRoomSpawner;

    private float timeSinceLastSpawn;
  
    private int currentTick;
    private long[] tickInfos;
    public override void ServerOnGameModeStart(ServerRoomGameModeBehaviour serverGameModeBehaviour)
    {

        
        // TODO implement bounty spawning
        _serverGameModeBehaviour = serverGameModeBehaviour;
        BountyRoomSpawner = _serverGameModeBehaviour.gameObject.GetComponent<BountyRoomSpawner>();
        BountyRoomSpawner.Setup(MapInfo.GetBountySpawnPoints());
        timeSinceLastSpawn = 0;


        var totalTicks = Mathf.FloorToInt(GameModeSettings.SecondDuration / GameModeSettings.SpawnSettings.TimeBetweenSpawns);

        tickInfos = GetSatDistribution(totalTicks, Financing.totalSatAmount, Distribution.UNIFORM);
        currentTick = 0;
        SpawnTick();
        /*
        var totalSats = subsidy + serverGameModeBehaviour.GameStatsWriter.Data.CarryoverSats;
        serverGameModeBehaviour.GameStatsWriter.SendUpdate(new GameStats.Update()
        {
            CarryoverSats = 0
        });
        
        serverGameModeBehaviour.BountySpawnerCommandSender.SendStartSpawningCommand(new EntityId(2), new Bountyhunt.StartSpawningRequest()
        {
            TotalDuration = GameModeSettings.SecondDuration,
            TimeBetweenTicks = GameModeSettings.SpawnSettings.TimeBetweenSpawns,
            MinSpawns = GameModeSettings.SpawnSettings.MinSpawnsPerSpawn,
            MaxSpawns = GameModeSettings.SpawnSettings.MaxSpawnsPerSpawn,
            TotalBounty = totalSats,
            Distribution = (Distribution)GameModeSettings.SpawnSettings.Distribution
        });*/
        //ServerEvents.instance.OnRandomInvoicePaid.AddListener(OnDonationPaid);

    }

    public override void GameModeUpdate(float deltaTime)
    {
        timeSinceLastSpawn += deltaTime;
        if(timeSinceLastSpawn >= GameModeSettings.SpawnSettings.TimeBetweenSpawns)
        {
            timeSinceLastSpawn = 0;
            SpawnTick();
        }
    }

    private void SpawnTick()
    {
        if(currentTick >= tickInfos.Length)
        {
            return;
        }
        var spawnPoints = MapInfo.GetBountySpawnPoints();
        var satsPerTick = tickInfos[currentTick];
        var minSpawns = GameModeSettings.SpawnSettings.MinSpawnsPerSpawn;
        var maxSpawns = Mathf.Clamp(spawnPoints.Length, 1, GameModeSettings.SpawnSettings.MaxSpawnsPerSpawn);

        Debug.LogFormat("spawning tick with {0} sats per tick {1} min spawns {2} max spawns ", satsPerTick, minSpawns, maxSpawns);
        long totalSats = 0;
        long remainingSats = satsPerTick;
        long carrySats = satsPerTick % maxSpawns;
        int plannedSpawns = UnityEngine.Random.Range(minSpawns, maxSpawns);
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
        currentTick++;

    }
    void SpawnPickupAtRandomPosition(long sats)
    {
        if (sats < 1)
            return;
        var pos = getRandomPosition();

        //pos.y = pos.y + 1f;
        BountyRoomSpawner.SpawnCube(pos, sats);

    }

    private Transform getRandomPosition()
    {
        var satsCubeSpawnPoints = MapInfo.GetBountySpawnPoints();
        var spawnIndex = UnityEngine.Random.Range(0, satsCubeSpawnPoints.Length);
        return satsCubeSpawnPoints[spawnIndex].transform;
    }
    private long[] GetSatDistribution(int totalTicks, long totalSats, Distribution distribution)
    {
        switch (distribution)
        {
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
    public override void ServerOnGameModeEnd(ServerRoomGameModeBehaviour serverGameModeBehaviour)
    {

        Debug.Log("end bbh");
    }


    private void OnDonationPaid(RandomInvoice e)
    {
        /*
        var pos = getRandomPosition();
        _serverGameModeBehaviour.BountySpawnerCommandSender.SendSpawnBountyPickupCommand(new EntityId(2), new SpawnBountyPickupRequest
        {
            BountyValue = e.amount,
            Position = new Vector3Float(pos.x,pos.y,pos.z)
        });
        ServerGameChat.instance.SendGlobalMessage("DONATION", e.message, Chat.MessageType.INFO_LOG, e.amount >= 250);*/
    }


    public override void ClientOnGameModeStart(RoomManagerClientBehaviour clientGameModeBehaviour)
    {

    }

    public override void ClientOnGameModeEnd(RoomManagerClientBehaviour clientGameModeBehaviour)
    {
       //PlayerServiceConnections.instance.UpdateBackendStats(BountyPlayerAuthorative.instance.HunterComponentReader.Data.Pubkey);
    }

    
}

[Serializable]
public struct BountyHuntSettings
{
    public float timeBetweenSpawns;
    public int minSpawns;
    public int maxSpawns;
    public Distribution distribution;
    public long baseSats;
}
