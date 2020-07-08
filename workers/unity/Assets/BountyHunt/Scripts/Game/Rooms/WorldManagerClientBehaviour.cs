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

    LinkedEntityComponent linkedEntityComponent;

    public static WorldManagerClientBehaviour Instance;

    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        linkedEntityComponent = GetComponent<LinkedEntityComponent>();
        //WorldManagerReader.OnUpdate += WorldManagerWriter_OnUpdate;
    }

    private void WorldManagerWriter_OnUpdate(WorldManager.Update obj)
    {
        
    }

    public void RequestJoinLobby()
    {
        WorldManagerCommandSender.SendJoinRoomCommand(linkedEntityComponent.EntityId, new JoinRoomRequest
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
