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
    private RoomManagerServerBehaviour _serverGameModeBehaviour;
    public override void ServerOnGameModeStart(RoomManagerServerBehaviour serverGameModeBehaviour, GameModeSettings settings, long subsidy)
    {

        
        this.GameModeSettings = settings;

        _serverGameModeBehaviour = serverGameModeBehaviour;
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

    public override void ServerOnGameModeEnd(RoomManagerServerBehaviour serverGameModeBehaviour)
    {

        Debug.Log("end bbh");
        ServerEvents.instance.OnRandomInvoicePaid.RemoveListener(OnDonationPaid);
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

    Vector3 getRandomPosition()
    {
        var pos = new Vector3(UnityEngine.Random.Range(-140, 140), 50, UnityEngine.Random.Range(-140, 140));
        pos = SpawnPoints.SnapToGround(pos);
        return pos;
    }

    public override void ClientOnGameModeStart(ClientGameModeBehaviour clientGameModeBehaviour)
    {

    }

    public override void ClientOnGameModeEnd(ClientGameModeBehaviour clientGameModeBehaviour)
    {
        PlayerServiceConnections.instance.UpdateBackendStats(BountyPlayerAuthorative.instance.HunterComponentReader.Data.Pubkey);
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
