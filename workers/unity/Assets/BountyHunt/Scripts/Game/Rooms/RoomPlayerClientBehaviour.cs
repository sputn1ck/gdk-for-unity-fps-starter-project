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

    [Require] WorldManagerCommandSender WorldManagerCommandSender;
    [Require] public EntityId EntityId;

    LinkedEntityComponent linkedEntityComponent;

    private string menuReference = "";

    public static RoomPlayerClientBehaviour Instance;
    // Start is called before the first frame update

    void Awake()
    {
        Instance = this;
    }
    void OnEnable()
    {
        linkedEntityComponent = GetComponent<LinkedEntityComponent>();
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (menuReference == "")
                OpenJoinRoomContextMenu();
        }
    }

    public void OpenJoinRoomContextMenu()
    {
        menuReference = "RoomMenuRef";
        var rooms = WorldManagerClientBehaviour.Instance.WorldManagerReader.Data.ActiveRooms;
        var actions = new List<(UnityAction, string)>();
        var cantinalabel = string.Format("Join Cantina");
        UnityAction cantinaAction = () =>
        {
            RequestJoinCantina();
            ContextMenuUI.Instance.Hide(menuReference);
        };
        actions.Add((cantinaAction, cantinalabel));
        foreach ( var room in rooms)
        {
            if (room.Value.RoomId.Contains("cantina"))
            {
                continue;
            }
            var label = string.Format("P:{0}, GM:{1}, MAP:{2}", room.Value.ActivePlayers.Count, room.Value.GamemodeId,room.Value.MapInfo.MapId);
            UnityAction action = () =>
            {
                RequestJoinRoom(room.Key);
                ContextMenuUI.Instance.Hide(menuReference);
            };
            actions.Add((action, label));
        }
        actions.Add((new UnityAction(() =>
        {
            ContextMenuUI.Instance.Hide(menuReference);
        }), "close menu"));
        ContextMenuArgs args = new ContextMenuArgs()
        {
            Headline = "Select Room",
            Actions = actions,
            lifeTime = 10f,
            ReferenceString = menuReference,
            CloseAction = new UnityAction(() => { menuReference = ""; }),
        };
        if (!ContextMenuUI.Instance.Set(args))
        {
            menuReference = "";
        }

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
                StartCoroutine(WaitForJoin(cb.ResponsePayload.Value.Room.RoomId));
            } else
            {
                RequestJoinRoom(cb.ResponsePayload.Value.Room.RoomId);
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