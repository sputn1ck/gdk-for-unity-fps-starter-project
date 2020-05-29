using Bbhrpc;
using Bountyhunt;
using Improbable.Gdk.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BountyBossGameMode", menuName = "BBH/GameModes/BountyBoss", order = 3)]
public class BountyBossGameMode : GameMode
{

    private ServerGameModeBehaviour _serverGameModeBehaviour;
    public override void ServerOnGameModeStart(ServerGameModeBehaviour serverGameModeBehaviour, GameModeSettings settings, long subsidy)
    {

        this.GameModeSettings = settings;

        _serverGameModeBehaviour = serverGameModeBehaviour;

        // setting subsidy + resetting carryover sats
        var totalSats = subsidy + serverGameModeBehaviour.GameStatsWriter.Data.CarryoverSats;
        serverGameModeBehaviour.GameStatsWriter.SendUpdate(new GameStats.Update()
        {
            CarryoverSats = 0
        });

        // Get Random Player
        // TODO use server game mode objects
        var playerGOs = GameObject.FindGameObjectsWithTag("Player");
        if (playerGOs.Length < 2)
            return;
        List<BountyPlayerServer> bountyPlayers = new List<BountyPlayerServer>();
        foreach (var go in playerGOs)
        {
            var bp = go.GetComponent<BountyPlayerServer>();
            if (bp != null) {
                bountyPlayers.Add(bp);
            }
        }
        var bountyKing = bountyPlayers[UnityEngine.Random.Range(0, bountyPlayers.Count)];
        bountyKing.IncreaseBounty(subsidy);
        ServerGameChat.instance.SendGlobalMessage("SERVER", bountyKing.HunterComponentWriter.Data.Name + " IS THE NEW BOUNTY KING, HUNT HIM DOWN", Chat.MessageType.INFO_LOG);
        ServerGameChat.instance.SendPrivateMessage(bountyKing.entityId.Id, 2, "SERVER", "YOU ARE THE BOUNTY KING", Chat.MessageType.ERROR_LOG, true);
    }


    public override void ServerOnGameModeEnd(ServerGameModeBehaviour serverGameModeBehaviour)
    {
    }

    public override void ClientOnGameModeStart(ClientGameModeBehaviour clientGameModeBehaviour)
    {
    }

    public override void ClientOnGameModeEnd(ClientGameModeBehaviour clientGameModeBehaviour)
    {
    }
}
