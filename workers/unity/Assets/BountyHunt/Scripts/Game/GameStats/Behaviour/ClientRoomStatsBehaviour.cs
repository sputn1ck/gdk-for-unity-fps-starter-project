using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Improbable.Gdk.Core;

public class ClientRoomStatsBehaviour : MonoBehaviour
{
    [Require] RoomStatsReader RoomStatsReader;
    [Require] RoomStatsCommandSender RoomStatsCommandSender;

    [Require] EntityId EntityId;

    public Dictionary<string, PlayerStats> playerStats;
    // Start is called before the first frame update
    void OnEnable()
    {
        RoomStatsReader.OnMapUpdateEvent += OnStatsUpdate;
        RequestStats();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void RequestStats()
    {
        RoomStatsCommandSender.SendRequestStatsCommand(EntityId, new Bountyhunt.Empty(), (cb) =>
        {
            if (cb.StatusCode != Improbable.Worker.CInterop.StatusCode.Success)
            {
                BBHUIManager.instance.mainMenu.BlendImage(false);
                Debug.LogError(cb.Message);
                return;
            }
            OnStatsUpdate(cb.ResponsePayload.Value);
            Debug.Log("Got stats response");
        });
    }

    private void OnStatsUpdate(PlayerStatsUpdate obj)
    {
        if (obj.ReplaceMap)
        {
            playerStats = obj.PlayerStats;
        }
        else
        {
            foreach (var kv in obj.PlayerStats)
            {
                playerStats[kv.Key] = kv.Value;
            }
        }
        if (obj.RemovePlayers != null)
        {
            foreach (var keys in obj.RemovePlayers)
            {
                playerStats.Remove(keys);
            }
        }
        SendStatMapEvent();
    }

    private void SendStatMapEvent()
    {
        var scoreboardUIItems = new List<ScoreboardUIItem>();
        foreach(var playerStat in playerStats)
        {
            if(ClientGameObjectManager.Instance.PlayerInfos.TryGetValue(playerStat.Key, out var playerInfo)){

                scoreboardUIItems.Add(new ScoreboardUIItem(playerInfo.Name, new ScoreboardItem(playerInfo.EntityId, playerStat.Value.Bounty, playerStat.Value.Kills, playerStat.Value.Deaths, playerStat.Value.SessionEarnings)));
            }
        }
        ClientEvents.instance.onScoreboardUpdate.Invoke(scoreboardUIItems, ClientGameObjectManager.Instance.AuthorativePlayerEntityId);
    }

}
