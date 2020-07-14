using Improbable.Gdk.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformConverter :MonoBehaviour, IConvertToEntity
{
    

    public void Convert(WorldCommandSender worldCommandSender, Map map, Vector3 workerOrigin)
    {
        if (worldCommandSender == null)
        {
            Destroy(gameObject);
            return;
        }
        var cube = DonnerEntityTemplates.MovingPlatform(transform.position - workerOrigin, transform.localScale, transform.rotation, map.EntityId);

        worldCommandSender.SendCreateEntityCommand(new Improbable.Gdk.Core.Commands.WorldCommands.CreateEntity.Request(cube), callback: (cb) => {
            if (cb.StatusCode != Improbable.Worker.CInterop.StatusCode.Success)
            {
                Debug.LogError(cb.Message);
                return;
            }
            Destroy(gameObject);
        });
    }
}
