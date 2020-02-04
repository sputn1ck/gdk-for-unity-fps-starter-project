using System.Collections;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.PlayerLifecycle;
using UnityEngine;
using Fps;
using Fps.Metrics;
using Fps.Health;
using Fps.Guns;
using Fps.WorkerConnectors;
using Fps.Config;
using Improbable.Worker.CInterop;
using Bountyhunt;
using Improbable.Gdk.Subscriptions;
using System.Linq;

public class ServerGameStats : MonoBehaviour
{
    [Require] GameStatsCommandReceiver GameStatsCommandReceiver;
    [Require] GameStatsWriter GameStatsWriter;
    
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
            BackendGameServerBehaviour.instance.AddPlayerHeartbeat(playerMap[obj.Payload.Id].Pubkey, Bbh.PlayerInfoEvent.Types.EventType.Disconnect);

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
}
