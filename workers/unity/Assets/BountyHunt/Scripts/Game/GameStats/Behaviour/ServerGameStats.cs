using System.Collections.Generic;
using UnityEngine;
using Fps.Config;
using Bountyhunt;
using Improbable.Gdk.Subscriptions;
using System.Linq;
using Improbable.Gdk.Core;

public class ServerGameStats : MonoBehaviour
{
    [Require] GameStatsCommandReceiver GameStatsCommandReceiver;
    [Require] GameStatsWriter GameStatsWriter;
    [Require] GameModeManagerReader GameModeManagerReader;
    private void OnEnable()
    {
        GameStatsCommandReceiver.OnSetNameRequestReceived += OnSetNameRequestReceived;
        GameStatsCommandReceiver.OnRemoveNameRequestReceived += OnRemoveNameRequestReceived;
    }



    private void OnRemoveNameRequestReceived(GameStats.RemoveName.ReceivedRequest obj)
    {
        if (obj.CallerAttributeSet[0] != WorkerUtils.UnityGameLogic)
            return;
        var playerMap = GameStatsWriter.Data.PlayerMap;
        if (playerMap.ContainsKey(obj.Payload.Id))
        {
            ServerServiceConnections.instance.BackendGameServerClient.AddPlayerDisconnect(playerMap[obj.Payload.Id].Pubkey);

            playerMap.Remove(obj.Payload.Id);
            GameStatsWriter.SendUpdate(new GameStats.Update() { PlayerMap = playerMap });
        }

    }

    private void OnSetNameRequestReceived(GameStats.SetName.ReceivedRequest obj)
    {
        if (obj.CallerAttributeSet[0] != WorkerUtils.UnityGameLogic)
            return;
        var playerMap = GameStatsWriter.Data.PlayerMap;
        var prev = playerMap.FirstOrDefault(u => u.Value.Pubkey == obj.Payload.Pubkey);
        if(prev.Value.Pubkey != "")
        {
            playerMap.Remove(prev.Key);
        }
        if (playerMap.ContainsKey(obj.Payload.Id))
        {
            var player = playerMap[obj.Payload.Id];
            player.Name = obj.Payload.Name;
            playerMap[obj.Payload.Id] = player;
        }else
        {
            var player = new PlayerItem { Name = obj.Payload.Name, Pubkey = obj.Payload.Pubkey };
            playerMap.Add(obj.Payload.Id, player);
        }
        GameStatsWriter.SendUpdate(new GameStats.Update() { PlayerMap = playerMap });
    }


    public void ResetScoreboard()
    {
        var newMap = new Dictionary<EntityId, PlayerItem>();
        foreach (var player in GameStatsWriter.Data.PlayerMap)
        {
            newMap.Add(player.Key, new PlayerItem(player.Value.Name, player.Value.Pubkey, 0,0,0,0));
        }

        GameStatsWriter.SendUpdate(new GameStats.Update()
        {
            LastRoundScores = GameStatsWriter.Data.PlayerMap,
            PlayerMap = newMap
        });
    }
}
