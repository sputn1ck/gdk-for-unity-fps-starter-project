using System.Collections.Generic;
using UnityEngine;
using Fps.Config;
using Bountyhunt;
using Improbable.Gdk.Subscriptions;
using System.Linq;
using Improbable.Gdk.Core;
using Bbhrpc;

public class ServerGameStats : MonoBehaviour
{
    [Require] GameStatsCommandReceiver GameStatsCommandReceiver;
    [Require] GameStatsWriter GameStatsWriter;
    [Require] HunterComponentCommandSender HunterCommandSender;

    private Dictionary<EntityId, GameObject> PlayerDict;
    public static ServerGameStats Instance;

    private void Awake()
    {
        Instance = this;
        PlayerDict = new Dictionary<EntityId, GameObject>();

    }

    private void OnEnable()
    {
        GameStatsCommandReceiver.OnSetNameRequestReceived += OnSetNameRequestReceived;
        GameStatsCommandReceiver.OnRemoveNameRequestReceived += OnRemoveNameRequestReceived;
        GameStatsCommandReceiver.OnUpdateSatsInCubesRequestReceived += GameStatsCommandReceiver_OnUpdateSatsInCubesRequestReceived;

        ServerEvents.instance.OnBackendKickEvent.AddListener(OnKickEvent);
    }
    private void OnDisable()
    {
        ServerEvents.instance.OnBackendKickEvent.RemoveListener(OnKickEvent);
    }

    public void AttachPlayer(EntityId playerId, GameObject playerGO)
    {
        if (PlayerDict.ContainsKey(playerId))
        {
            PlayerDict[playerId] = playerGO;
        } else
        {
            PlayerDict.Add(playerId, playerGO);
        }
    }

    public GameObject GetPlayerGameObject(EntityId playerId)
    {
        if (PlayerDict.ContainsKey(playerId))
        {
            return PlayerDict[playerId];
        }
        return null;
    }

    public void RemovePlayerGameObject(EntityId playerId)
    {
        if (PlayerDict.ContainsKey(playerId))
        {
            PlayerDict.Remove(playerId);
        }
    }
    private void  OnKickEvent(KickEvent e)
    {

        var user = GameStatsWriter.Data.PlayerMap.FirstOrDefault(u => u.Value.Name == e.UserName);
        if(user.Value.Name == e.UserName)
        {
            HunterCommandSender.SendKickPlayerCommand(user.Key, new KickPlayerRequest(user.Value.Name), OnKickCallback);
            RemovePlayer(user.Key);
        } else
        {
            user = GameStatsWriter.Data.PlayerMap.FirstOrDefault(u => u.Value.Pubkey == e.UserPubkey);
            if (user.Value.Pubkey == e.UserPubkey)
            {
                HunterCommandSender.SendKickPlayerCommand(user.Key, new KickPlayerRequest(user.Value.Name), OnKickCallback);

                RemovePlayer(user.Key);
            }
        }
        
    }
    private void OnKickCallback(HunterComponent.KickPlayer.ReceivedResponse res)
    {
        if  (res.StatusCode == Improbable.Worker.CInterop.StatusCode.Success)
        {
            //TODO user  name
            ServerGameChat.instance.SendGlobalMessage("KICK", res.RequestPayload.PlayerName+" has been kicked", Chat.MessageType.INFO_LOG);
            
        }
    }
    private void GameStatsCommandReceiver_OnUpdateSatsInCubesRequestReceived(GameStats.UpdateSatsInCubes.ReceivedRequest obj)
    {
        GameStatsWriter.SendUpdate(new GameStats.Update
        {
            BountyInCubes = GameStatsWriter.Data.BountyInCubes + obj.Payload.Amount,
        });
    }

    private void OnRemoveNameRequestReceived(GameStats.RemoveName.ReceivedRequest obj)
    {
        if (obj.CallerAttributeSet[0] != WorkerUtils.UnityGameLogic)
            return;
        RemovePlayer(obj.Payload.Id);

    }

    private void RemovePlayer(EntityId id)
    {
        var playerMap = GameStatsWriter.Data.PlayerMap;
        if (playerMap.ContainsKey(id))
        {
            ServerServiceConnections.instance.BackendGameServerClient.AddPlayerDisconnect(playerMap[id].Pubkey);
            playerMap.Remove(id);
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
