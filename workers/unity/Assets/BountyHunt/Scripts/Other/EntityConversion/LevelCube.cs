using Improbable.Gdk.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCube : MonoBehaviour, IConvertToEntity
{
    public void Convert(WorldCommandSender worldCommandSender, Map map, Vector3 Workerorigin)
    {
        if (worldCommandSender == null)
        {
            Destroy(gameObject);
            return;
        }
        var cube = DonnerEntityTemplates.LevelCube(transform.position-Workerorigin, transform.localScale, transform.rotation,map.EntityId);

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
