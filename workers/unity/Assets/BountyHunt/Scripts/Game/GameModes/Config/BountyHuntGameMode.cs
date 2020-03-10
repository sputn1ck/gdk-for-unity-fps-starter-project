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

[CreateAssetMenu(fileName = "BountyHuntGameMode", menuName = "BBH/GameModes/BountyHunt", order = 2)]
public class BountyHuntGameMode : GameMode
{
    public BountyHuntSettings bountyHuntSettings;
    private ServerGameModeBehaviour _serverGameModeBehaviour;
    public override async void ServerOnGameModeStart(ServerGameModeBehaviour serverGameModeBehaviour)
    {
        _serverGameModeBehaviour = serverGameModeBehaviour;
        var totalSats = await ServerServiceConnections.instance.BackendGameServerClient.GetRoundBounty() +FlagManager.instance.GetBaseSubsidiy()+ bountyHuntSettings.baseSats + serverGameModeBehaviour.GameStatsWriter.Data.CarryoverSats;
        serverGameModeBehaviour.GameStatsWriter.SendUpdate(new GameStats.Update()
        {
            CarryoverSats = 0
        });
        serverGameModeBehaviour.BountySpawnerCommandSender.SendStartSpawningCommand(new EntityId(2), new Bountyhunt.StartSpawningRequest()
        {
            TotalDuration = GlobalSettings.SecondDuration,
            TimeBetweenTicks = bountyHuntSettings.timeBetweenSpawns,
            MinSpawns = bountyHuntSettings.minSpawns,
            MaxSpawns = bountyHuntSettings.maxSpawns,
            TotalBounty = totalSats,
            Distribution = bountyHuntSettings.distribution
        });
        ServerEvents.instance.OnRandomInvoicePaid.AddListener(OnDonationPaid);
    }

    public override void ServerOnGameModeEnd(ServerGameModeBehaviour serverGameModeBehaviour)
    {

        ServerEvents.instance.OnRandomInvoicePaid.RemoveListener(OnDonationPaid);
    }


    private void OnDonationPaid(RandomInvoice e)
    {
        var pos = getRandomPosition();
        _serverGameModeBehaviour.BountySpawnerCommandSender.SendSpawnBountyPickupCommand(new EntityId(2), new SpawnBountyPickupRequest
        {
            BountyValue = e.amount,
            Position = new Vector3Float(pos.x,pos.y,pos.z)
        });
        ServerGameChat.instance.SendGlobalMessage("DONATION", e.message, Chat.MessageType.INFO_LOG, e.amount >= 250);
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
        PlayerServiceConnections.instance.BackendPlayerClient.UpdateBackendStats(BountyPlayerAuthorative.instance.HunterComponentReader.Data.Pubkey);
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