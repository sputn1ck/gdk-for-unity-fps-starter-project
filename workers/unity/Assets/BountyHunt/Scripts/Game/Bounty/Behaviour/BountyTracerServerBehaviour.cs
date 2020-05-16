using Improbable;
using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bountyhunt;
using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Commands;

public class BountyTracerServerBehaviour : MonoBehaviour
{
    [Require] PositionWriter positionWriter;
    [Require] TracerComponentWriter tracerWriter;
    [Require] private EntityId entityId;
    [Require] private WorldCommandSender worldCommandSender;

    private GameObject playerObj;
    public void OnEnable()
    {
        playerObj = ServerGameStats.Instance.GetPlayerGameObject(new EntityId(tracerWriter.Data.AttachedHunter));
        StartCoroutine(MoveBountyTracerEnumerator());          
    }
    public void UpdatePosition(Vector3 newPos)
    {

        var spatialPositionUpdate = new Position.Update
        {
            Coords = Coordinates.FromUnityVector(newPos)
        };
        positionWriter.SendUpdate(spatialPositionUpdate);
        transform.position = newPos;
    }

    private IEnumerator MoveBountyTracerEnumerator()
    {
        while (!ServerServiceConnections.ct.IsCancellationRequested)
        {
            if(playerObj != null)
            {
                UpdatePosition(playerObj.transform.position);
            } else
            {
                Debug.Log("destroying bounty tracer");
                worldCommandSender.SendDeleteEntityCommand(new WorldCommands.DeleteEntity.Request(this.entityId));
                Destroy(this.gameObject);
            }
            yield return new WaitForSeconds(10f);
        }
    }
}
