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
public class SatsStackerGameMode : BountyGameMode
{
    private ServerRoomGameModeBehaviour _serverGameModeBehaviour;
    private BountyRoomSpawner BountyRoomSpawner;
    private ServerRoomGameStatsMap ServerRoomGameStatsMap;
    private BountyTickComponentCommandSender BountyTickCommandSender;
    private float timeSinceLastSpawn;
  
    private int currentTick;
    private long[] tickInfos;
    public override void ServerOnGameModeStart(ServerRoomGameModeBehaviour serverGameModeBehaviour)
    {
        Debug.Log("satsstacker started");
        
        // TODO implement bounty spawning
        _serverGameModeBehaviour = serverGameModeBehaviour;
        BountyTickCommandSender = _serverGameModeBehaviour.BountyTickComponentCommandSender;
        BountyRoomSpawner = _serverGameModeBehaviour.gameObject.GetComponent<BountyRoomSpawner>();
        ServerRoomGameStatsMap = _serverGameModeBehaviour.gameObject.GetComponent<ServerRoomGameStatsMap>();
        BountyRoomSpawner.Setup(MapInfo.GetBountySpawnPoints());
        timeSinceLastSpawn = 0;


        var totalTicks = Mathf.FloorToInt(GameModeSettings.SecondDuration / GameModeSettings.SpawnSettings.TimeBetweenSpawns);

        tickInfos = GetSatDistribution(totalTicks, Financing.totalSatAmount, Distribution.UNIFORM);
        currentTick = 0;
        SpawnTick();

        foreach (var player in serverGameModeBehaviour.RoomManagerWriter.Data.RoomInfo.PlayerInfo.ActivePlayers)
        {
            BountyTickCommandSender.SendSetTickIntervalCommand(player.Value, new TickIntervalRequest(true, GameModeSettings.BountySettings.BountyTickTimeSeconds));
        }

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

    private Vector3 getRandomPosition()
    {
        var satsCubeSpawnPoints = MapInfo.GetBountySpawnPoints();
        var spawnIndex = UnityEngine.Random.Range(0, satsCubeSpawnPoints.Length);

        var pos = satsCubeSpawnPoints[spawnIndex].transform.position - _serverGameModeBehaviour.LinkedEntityComponent.Worker.Origin;
        return pos;
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
        foreach(var player in serverGameModeBehaviour.RoomManagerWriter.Data.RoomInfo.PlayerInfo.ActivePlayers)
        {
            BountyTickCommandSender.SendSetTickIntervalCommand(player.Value, new TickIntervalRequest(false, 0));
            bountyTick(player.Key, 1);
        }
        Debug.Log("end bbh");
    }

    public override void PlayerKill(string killer, string victim, Vector3 position)
    {
        DropBountyAtPlayer(victim, position, GameModeSettings.BountySettings.BountyDropPercentageDeath);
    }

    public void DropBountyAtPlayer(string playerId, Vector3 position, double dropPercentage)
    {
        var playerStats = ServerRoomGameStatsMap.GetStats(playerId);

        var player = _serverGameModeBehaviour.RoomManagerWriter.Data.RoomInfo.PlayerInfo.ActivePlayers[playerId];
        // spawn bountycube if victim has bounty
        if (playerStats.Bounty > 0)
        {
            var res = Utility.GetBountyDropInfo(playerStats.Bounty, dropPercentage);
            BountyRoomSpawner.SpawnCube(position, res.dropBounty);
            ServerRoomGameStatsMap.SetBounty(playerId, res.newBounty);
        }
    }

    public override void BountyTick(string player)
    {
        bountyTick(player, GameModeSettings.BountySettings.BountyTickConversion);
    }

    private void bountyTick(string player, double tickpercentage)
    {
        var playerStats = ServerRoomGameStatsMap.GetStats(player);

        if (playerStats.Bounty > 0)
        {
            var tick = Utility.BountyTick(playerStats.Bounty, tickpercentage);
            ServerRoomGameStatsMap.SetBounty(player, playerStats.Bounty - tick);
            ServerRoomGameStatsMap.AddEarnings(player, tick);
            ServerRoomGameStatsMap.AddScore(player, tick);
        }
    }

    public override void OnPlayerJoin(string playerId)
    {
        var player = _serverGameModeBehaviour.RoomManagerWriter.Data.RoomInfo.PlayerInfo.ActivePlayers[playerId];
        BountyTickCommandSender.SendSetTickIntervalCommand(player, new TickIntervalRequest(true, GameModeSettings.BountySettings.BountyTickTimeSeconds));
    }

    public override void OnPlayerLeave(string playerId)
    {
        //var player = _serverGameModeBehaviour.RoomManagerWriter.Data.RoomInfo.PlayerInfo.ActivePlayers[playerId];
        //BountyTickCommandSender.SendSetTickIntervalCommand(player, new TickIntervalRequest(false,0));

        var playerStats = ServerRoomGameStatsMap.GetStats(playerId);
        if(playerStats.Bounty > 0)
        {
                Debug.Log("spawning cube at random position");
                SpawnPickupAtRandomPosition(playerStats.Bounty);
                ServerRoomGameStatsMap.SetBounty(playerId, 0);
            
        }
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
