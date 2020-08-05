using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Commands;
using System.Linq;

public class WorldManagerClientBehaviour : MonoBehaviour
{
    [Require] public WorldManagerReader WorldManagerReader;
    [Require] WorldManagerCommandSender WorldManagerCommandSender;
    [Require] EntityId EntityId;


    public static WorldManagerClientBehaviour Instance;

    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        WorldManagerReader.OnUpdate += WorldManagerReader_OnUpdate;
    }


    private void WorldManagerReader_OnUpdate(WorldManager.Update obj)
    {
        if (obj.ActivePlayers.HasValue)
        {
            ClientGameObjectManager.Instance.PlayerInfos = obj.ActivePlayers.Value;
        }
    }

    public void RequestJoinLobby()
    {
        WorldManagerCommandSender.SendJoinRoomCommand(EntityId, new JoinRoomRequest
        {
            RoomId = "cantina-1"
        }, (cb) => {
            if (cb.StatusCode != Improbable.Worker.CInterop.StatusCode.Success)
            {
                Debug.LogError(cb.Message);
            }
            
        });
    }

}
