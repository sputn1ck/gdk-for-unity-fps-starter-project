using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Commands;
using System.Linq;
using UnityEngine.Events;
using Improbable.Gdk.QueryBasedInterest;

public class RoomPlayerClientBehaviour : MonoBehaviour 
{
    [Require] RoomPlayerReader RoomPlayerReader;
    [Require] WorldManagerCommandSender WorldManagerCommandSender;
    [Require] public EntityId EntityId;

    LinkedEntityComponent linkedEntityComponent;

    public static RoomPlayerClientBehaviour Instance;
   public RoomBaseInfo CurrentRoom;

    private EntityId currentRoomId = new EntityId(0);
    // Start is called before the first frame update

    void Awake()
    {
        Instance = this;
    }
    void OnEnable()
    {
        linkedEntityComponent = GetComponent<LinkedEntityComponent>();
        RoomPlayerReader.OnRoomEntityidUpdate += OnNewRoom;
        RoomPlayerReader.OnActiveRoomUpdate += RoomPlayerReader_OnActiveRoomUpdate;
    }


    private void RoomPlayerReader_OnActiveRoomUpdate(RoomBaseInfo obj)
    {
        CurrentRoom = obj;
    }

    private void OnNewRoom(EntityId obj)
    {
        Debug.Log("new room " + obj.Id);
        if(currentRoomId == obj)
        {
            Debug.Log("Weird behaviour the second");
            return;
        }
        if(currentRoomId.Id != 0)
        {
            var oldRoomGo = ClientGameObjectManager.Instance.GetRoomGO(currentRoomId);
            if(oldRoomGo != null)
            {
                oldRoomGo.GetComponent<RoomManagerClientBehaviour>()?.Deinitialize();
            }
        }
        currentRoomId = obj;
        StartCoroutine(WaitForMap(obj));
    }

    IEnumerator WaitForMap(EntityId obj)
    {
        Debug.Log("Waiting for map " + obj.Id);
        GameObject newRoomGo = null;
        while(newRoomGo == null)
        {
            newRoomGo = ClientGameObjectManager.Instance.GetRoomGO(obj);
            yield return new WaitForEndOfFrame();
        }
        ClientGameObjectManager.Instance.ActiveRoom = newRoomGo;
        ClientGameObjectManager.Instance.ActiveRoomEntityId = obj;
        Debug.Log("Room loaded " + obj.Id);
        newRoomGo.GetComponent<RoomManagerClientBehaviour>().Initialize();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            OpenJoinRoomContextMenu();
        }
    }

    public void OpenJoinRoomContextMenu()
    {
        var rooms = WorldManagerClientBehaviour.Instance.WorldManagerReader.Data.ActiveRooms;
        var actions = new List<(UnityAction, string)>();
        var cantinalabel = string.Format("Join Cantina");
        UnityAction cantinaAction = () =>
        {
            RequestJoinCantina();
            ContextMenuUI.Instance.CloseCurrentAndShowNext();
        };
        actions.Add((cantinaAction, cantinalabel));
        foreach ( var room in rooms)
        {
            if (room.Value.Info.RoomId.Contains("cantina"))
            {
                continue;
            }
            var mode = room.Value.GameModeInfo.ModeRotation[room.Value.GameModeInfo.CurrentMode % room.Value.GameModeInfo.ModeRotation.Count];
            var label = string.Format("P:{0}, GM:{1}, MAP:{2}", room.Value.PlayerInfo.ActivePlayers.Count, mode.GamemodeId,room.Value.Info.MapInfo.MapId);
            UnityAction action = () =>
            {
                RequestJoinRoom(room.Key);
                ContextMenuUI.Instance.CloseCurrentAndShowNext();
            };
            actions.Add((action, label));
        }
        ContextMenuArgs args = new ContextMenuArgs()
        {
            Headline = "Select Room",
            Actions = actions,
            Type = ContextMenuType.REPLACE
        };
        ContextMenuUI.Instance.Set(args);
    }
    public void RequestJoinCantina()
    {
        WorldManagerCommandSender.SendGetCantinaCommand(new EntityId(3), new GetCantinaRequest() {
            PlayerId = EntityId
        },(cb) => {
            if (cb.StatusCode != Improbable.Worker.CInterop.StatusCode.Success)
            {
                Debug.LogError(cb.Message);
                return;
            }
            if (cb.ResponsePayload.Value.NewlyCreated)
            {
                StartCoroutine(WaitForJoin(cb.ResponsePayload.Value.Room.Info.RoomId));
            } else
            {
                RequestJoinRoom(cb.ResponsePayload.Value.Room.Info.RoomId);
            }
            
        });
    }
    public IEnumerator WaitForJoin(string roomid)
    {
        yield return new WaitForSeconds(2f);
        RequestJoinRoom(roomid);

    }
    public void RequestJoinRoom(string roomid)
    {
        WorldManagerCommandSender.SendJoinRoomCommand(new EntityId(3), new JoinRoomRequest
        {
            RoomId = roomid,
            PlayerId = linkedEntityComponent.EntityId,
        }, (cb) => {
            if (cb.StatusCode != Improbable.Worker.CInterop.StatusCode.Success)
            {
                Debug.LogError(cb.Message);
                return;
            }
            BBHUIManager.instance.mainMenu.BlendImage(true);

        });
    }

    
}
