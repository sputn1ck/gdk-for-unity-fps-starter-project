using System.Collections;
using System.Collections.Generic;
using Bountyhunt;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Bbhrpc;
using PlayerStats = Bountyhunt.PlayerStats;

namespace Tests
{
    public class GameModeTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void SatsStackerIntegrationTest()
        {

            // Setup
            var spawnPoints = new EmptySpawnPoints();
            var spawner = new EmptySpawner();
            var statMap = new TestGameStatsMap();
            var ss = new SatsStackerGameMode();
            ss.Initialize(GetDefaultSettings(), spawnPoints, GetFinancing(), statMap, spawner, new Vector3(0, 0, 0));

            // add player and bounty
            var p1 = "p1";
            ss.ServerOnGameModeStart();


            statMap.AddPlayer(p1);
            statMap.AddBounty(p1, 100);
            ss.GameModeUpdate(15);

            var newStats = statMap.GetStats(p1);
            Assert.AreEqual(95, newStats.Bounty);
            Assert.AreEqual(5, newStats.SessionEarnings);

            ss.GameModeUpdate(20);
            Assert.GreaterOrEqual(spawner.cubesSpawned, 1);

        }


        
        // returns base settings with 100% drop on death, 5% bounty conversion every 10 secs and 2-5 spawns every 30 secs
        private GameModeSettings GetDefaultSettings()
        {
            var settings = new GameModeSettings
            {
                SecondDuration = 300,
                BaseSettings = new BaseSettings { ClearBountyOnEnd = true, ClearPickupsOnEnd = true, ClearStatsOnEnd = true, TeleportPlayerOnStart = true },
                BountySettings = new BountySettings { BountyDropPercentageDeath = 1, BountyTickConversion = 0.05, BountyTickTimeSeconds = 10 },
                SpawnSettings = new SpawnSettings { MaxSpawnsPerSpawn = 5, MinSpawnsPerSpawn = 2, Distribution = BountyDistribution.Uniform, TimeBetweenSpawns = 30 },
            };
            return settings;
        }
        private GameModeFinancing GetFinancing()
        {
            var financing = new GameModeFinancing
            {
                totalSatAmount = 1000
            };
            return financing;
        }
        internal class EmptySpawner : IBountyRoomSpawner
        {
            public int cubesSpawned;
            public void Setup(SatsCubeSpawnPoint[] satsCubeSpawnPoints)
            {
                
            }

            public void SpawnCube(Vector3 position, long satAmount)
            {
                cubesSpawned++;
            }

            public void SpawnCubeAtEntity(Improbable.Gdk.Core.EntityId entity, long satAmount)
            {
                cubesSpawned++;
            }
        }
        internal class EmptySpawnPoints : IBountySpawnPointer
        {
            public SatsCubeSpawnPoint[] GetBountySpawnPoints()
            {
                return new SatsCubeSpawnPoint[] {
                    new SatsCubeSpawnPoint()
                    {

                    }
                };
            }
        }

    }

    public class TestGameStatsMap : IServerRoomGameStatsMap
    {
        public Dictionary<string, PlayerStats> pStats;
        public TestGameStatsMap()
        {
            pStats = new Dictionary<string, PlayerStats>();
        }
        public void AddBounty(string playerId, long bounty)
        {
            if(pStats.TryGetValue(playerId, out var stats)){
                stats.Bounty += bounty;
                pStats[playerId] = stats;
            }
        }

        public void AddEarnings(string playerId, long earnings)
        {
            if (pStats.TryGetValue(playerId, out var stats))
            {
                stats.SessionEarnings += earnings;
                pStats[playerId] = stats;
            }
        }

        public void AddKill(string killerId, string victimId)
        {
            if (pStats.TryGetValue(killerId, out var killerStats))
            {
                killerStats.Kills++;
                pStats[killerId] = killerStats;
            }
            if (pStats.TryGetValue(victimId, out var victimStats))
            {
                victimStats.Deaths++;
                pStats[victimId] = victimStats;
            }
        }

        public void AddPlayer(string pubkey)
        {
            pStats.Add(pubkey, new PlayerStats() { Active = true });
        }

        public void AddScore(string playerId, long score)
        {
            if (pStats.TryGetValue(playerId, out var stats))
            {
                stats.Score += score;
                pStats[playerId] = stats;
            }
        }

        public Dictionary<string, PlayerStats> GetPlayerDictionary()
        {
            return pStats;
        }

        public PlayerStats GetStats(string playerId)
        {
            if (pStats.TryGetValue(playerId, out var stats))
            {
                return stats;
            }
            return new PlayerStats();
        }

        public void Initialize(Room room)
        {
            throw new System.NotImplementedException();
        }

        public void RemovePlayer(string pubkey)
        {
            if (pStats.TryGetValue(pubkey, out var stats))
            {
                stats.Active = false;
                pStats[pubkey] = stats;
            }
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }

        public void SetBounty(string playerId, long bounty)
        {
            if (pStats.TryGetValue(playerId, out var stats))
            {
                stats.Bounty = bounty;
                pStats[playerId] = stats;
            }
        }

        public void UpdateDictionary(Dictionary<string, PlayerStats> newMap)
        {
            var tempMap = new Dictionary<string, PlayerStats>(pStats.Count, pStats.Comparer);
            foreach (var kv in newMap)
            {
                if (tempMap.ContainsKey(kv.Key))
                {
                    tempMap[kv.Key] = kv.Value;
                }
                else
                {
                    tempMap.Add(kv.Key, kv.Value);
                }

            }
            pStats = tempMap;
        }
    }
}
