using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Improbable.Gdk.Core;
using Bountyhunt;

[CreateAssetMenu(fileName = "BountyHuntGameMode", menuName = "BBH/GameModes/BountyHunt", order = 2)]
public class BountyHuntGameMode : GameMode
{
    public BountyHuntSettings bountyHuntSettings;
    public override async void OnGameModeStart(ServerGameModeBehaviour serverGameModeBehaviour)
    {
        var totalSats = await ServerServiceConnections.instance.BackendGameServerClient.GetRoundBounty()+ bountyHuntSettings.baseSats;
        serverGameModeBehaviour.BountySpawnerCommandSender.SendStartSpawningCommand(new EntityId(2), new Bountyhunt.StartSpawningRequest()
        {
            TotalDuration = GlobalSettings.SecondDuration,
            TimeBetweenTicks = bountyHuntSettings.timeBetweenTicks,
            MinSpawns = bountyHuntSettings.minSpawns,
            MaxSpawns = bountyHuntSettings.maxSpawns,
            TotalBounty = totalSats,
            Distribution = bountyHuntSettings.distribution
        });
    }

    public override void OnGameModeEnd(ServerGameModeBehaviour serverGameModeBehaviour)
    {
        
    }


    
}

[Serializable]
public struct BountyHuntSettings
{
    public float timeBetweenTicks;
    public int minSpawns;
    public int maxSpawns;
    public Distribution distribution;
    public long baseSats;
}
