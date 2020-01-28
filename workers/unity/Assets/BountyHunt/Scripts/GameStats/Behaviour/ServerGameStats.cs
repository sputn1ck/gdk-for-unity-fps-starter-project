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
        var nameMap = GameStatsWriter.Data.PlayerNames;
        var scoreBoard = GameStatsWriter.Data.Scoreboard;
        if (nameMap.ContainsKey(obj.Payload.Id))
        {
            nameMap.Remove(obj.Payload.Id);
            var user = scoreBoard.Board.Find(u => u.Entity == obj.Payload.Id);
            if(scoreBoard.Board.Contains(user))
                scoreBoard.Board.Remove(user);
            GameStatsWriter.SendUpdate(new GameStats.Update() { PlayerNames = nameMap, Scoreboard = scoreBoard });
        }
    }

    private void OnSetNameRequestReceived(GameStats.SetName.ReceivedRequest obj)
    {
        if (obj.CallerAttributeSet[0] != WorkerUtils.UnityGameLogic)
            return;
        var nameMap = GameStatsWriter.Data.PlayerNames;
        if (nameMap.ContainsKey(obj.Payload.Id))
        {
            nameMap[obj.Payload.Id] = obj.Payload.Name;
        }else
        {
            nameMap.Add(obj.Payload.Id, obj.Payload.Name);
        }
        GameStatsWriter.SendUpdate(new GameStats.Update() { PlayerNames = nameMap });
    }
}
