using Fps;
using Fps.Respawning;
using Improbable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Fps.SchemaExtensions;

[CreateAssetMenu(menuName = "BBH/Skills/Teleport", order = 1)]
public class TeleportSkill : PlayerSkill
{
    public float Distance;
    public override CastResponse ServerCastSkill(ServerPlayerSkillBehaviour player)
    {
        Vector3 enterPosition = player.transform.position;
        JsonUtility.FromJson("", typeof(TeleportPayload));
        var forward2d = new Vector2(player.transform.forward.x, player.transform.forward.z).normalized * this.Distance;
        var teleport = new Vector2(player.transform.position.x + forward2d.x, player.transform.position.z + forward2d.y);
        var pos = new Vector3(teleport.x, 75, teleport.y);
        if (pos.x > 140 || pos.z > 140 || pos.z < -140 || pos.x < -140)
        {
            return new CastResponse()
            {
                ok = false,
                errorMsg = "out of bounds"
            };
        }
        pos = SpawnPoints.SnapToGround(pos);
        Debug.Log("teleport invoice paid!!" + pos);
        Vector3 exitPosition = player.transform.position;

        //var (pos, spawnYaw, spawnPitch) = SpawnPoints.GetRandomSpawnPoint();
        Bountyhunt.EffectInfo info = new Bountyhunt.EffectInfo
        {
            Key = "TeleportEnter",
            Position = enterPosition.ToBbhVector(),
            RotationEuler = player.transform.rotation.eulerAngles.ToBbhVector(),
            Parent = Bountyhunt.EffectParent.ROOM,
            PositionIsLocal = false
        };
        player.effectSpawnerComponentCommandSender.SendSpawnEffectCommand(player.entityId, info);
        info.Key = "TeleportExit";
        info.Position = exitPosition.ToBbhVector();
        player.effectSpawnerComponentCommandSender.SendSpawnEffectCommand(player.entityId, info);

        // Move to a spawn point (position and rotation)
        var newLatest = new ServerResponse
        {
            Position = pos.ToVector3Int(),
            IncludesJump = false,
            Timestamp = player.ServerMovementWriter.Data.Latest.Timestamp,
            TimeDelta = 0
        };

        var serverMovementUpdate = new ServerMovement.Update
        {
            Latest = newLatest
        };
        player.ServerMovementWriter.SendUpdate(serverMovementUpdate);

        player.transform.position = pos + player.LinkedEntityComponent.Worker.Origin;

        var spatialPositionUpdate = new Position.Update
        {
            Coords = Coordinates.FromUnityVector(pos)
        };
        player.spatialPosition.SendUpdate(spatialPositionUpdate);
        return new CastResponse()
        {
            ok = true
        };
    }
    public override void ClientCastSkill(ClientPlayerSkillBehaviour player)
    {
        Debug.Log("casted teleport");
        
    }

    public override void NonAuthorativeCastSkill(NonAuthorativePlayerSkillBehaviour player)
    {
        Debug.LogError("other player casted teleport");
    }

    public struct TeleportPayload
    {
        public float distance { get; set; }
    }
}
