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
public class SatsStackerGameMode : GameMode, IKillGameMode, IUpdateGameMode, IPlayerJoinLeaveEvents
{
    // player ticks
    private float timeSinceLastTick;

    // bounty spawn ticks
    private float timeSinceLastSpawn;
    private int currentTick;
    private long[] tickInfos;

    #region GameModeBase
    public override void ServerOnGameModeStart()
    {
        Debug.Log("satsstacker started");
        
        // TODO implement bounty spawning
        bountyRoomSpawner.Setup(MapInfo.GetBountySpawnPoints());
        


        var totalTicks = Mathf.FloorToInt(GameModeSettings.SecondDuration / GameModeSettings.SpawnSettings.TimeBetweenSpawns);
        tickInfos = GetSatDistribution(totalTicks, Financing.totalSatAmount, Distribution.UNIFORM);
        currentTick = 0;
        timeSinceLastSpawn = 0;
        SpawnTick();

    }

    public override void ServerOnGameModeEnd()
    {

        Debug.Log("end bbh");
    }


    #endregion
    #region PlayerJoinLeave
    public void OnPlayerJoin(string playerId)
    {
       
    }

    public void OnPlayerLeave(string playerId)
    {
        //var player = _serverGameModeBehaviour.RoomManagerWriter.Data.RoomInfo.PlayerInfo.ActivePlayers[playerId];
        //BountyTickCommandSender.SendSetTickIntervalCommand(player, new TickIntervalRequest(false,0));

        var playerStats = serverRoomGameStatsMap.GetStats(playerId);
        if (playerStats.Bounty > 0)
        {
            Debug.Log("spawning cube at random position");
            SpawnPickupAtRandomPosition(playerStats.Bounty);
            serverRoomGameStatsMap.SetBounty(playerId, 0);

        }
    }
    #endregion
    #region KillGameMode
    public void PlayerKill(string killer, string victim, Vector3 position)
    {
        DropBountyAtPlayer(victim, position, GameModeSettings.BountySettings.BountyDropPercentageDeath);
    }
    public void DropBountyAtPlayer(string playerId, Vector3 position, double dropPercentage)
    {
        var playerStats = serverRoomGameStatsMap.GetStats(playerId);

        // spawn bountycube if victim has bounty
        if (playerStats.Bounty > 0)
        {
            var res = Utility.GetBountyDropInfo(playerStats.Bounty, dropPercentage);
            bountyRoomSpawner.SpawnCube(position, res.dropBounty);
            serverRoomGameStatsMap.SetBounty(playerId, res.newBounty);
        }
    }
    #endregion
    #region UpdateGameMode
    public void GameModeUpdate(float deltaTime)
    {
        timeSinceLastSpawn += deltaTime;
        if (timeSinceLastSpawn >= GameModeSettings.SpawnSettings.TimeBetweenSpawns)
        {
            timeSinceLastSpawn = 0;
            SpawnTick();
        }
        timeSinceLastTick += deltaTime;
        if (timeSinceLastTick >= GameModeSettings.BountySettings.BountyTickTimeSeconds)
        {
            timeSinceLastTick = 0;
            bountyTick();
        }
    }


    #endregion
    #region CubeSpawning
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
        bountyRoomSpawner.SpawnCube(pos, sats);

    }

    private Vector3 getRandomPosition()
    {
        var satsCubeSpawnPoints = MapInfo.GetBountySpawnPoints();
        var spawnIndex = UnityEngine.Random.Range(0, satsCubeSpawnPoints.Length);

        var pos = satsCubeSpawnPoints[spawnIndex].GetPosition() - workerOrigin;
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
    #endregion
    #region BountyTick
    private void bountyTick()
    {
        var playerDict = serverRoomGameStatsMap.GetPlayerDictionary();
        var tmpDict = new Dictionary<string, Bountyhunt.PlayerStats>(playerDict.Count, playerDict.Comparer);
        foreach (var player in playerDict)
        {
            if (!player.Value.Active && player.Value.Bounty > 0)
                continue;
            var pStats = player.Value;
            var tick = Utility.BountyTick(player.Value.Bounty, GameModeSettings.BountySettings.BountyTickConversion);
            pStats.Bounty -= tick;
            pStats.SessionEarnings += tick;
            pStats.Score += tick;
            tmpDict[player.Key] = pStats;
        }
        serverRoomGameStatsMap.UpdateDictionary(tmpDict);
    }
    #endregion

    



    

    
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
