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
        var room = player.GetComponent<RoomPlayerServerBehaviour>().CurrentRoom;
        var boundaries = Utility.GetRoomBoundaries(room);
        Debug.LogFormat("teleport boundaries: {0} {1} {2} {3}", boundaries.x1, boundaries.x2, boundaries.z1, boundaries.z2);
        if (pos.x > boundaries.x2 || pos.z > boundaries.z2 || pos.z < boundaries.z1 || pos.x < boundaries.x1)
        {
            return new CastResponse()
            {
                ok = false,
                errorMsg = "out of bounds"
            };
        }
        pos = SpawnPoints.SnapToGround(pos);
        Debug.Log("teleport invoice paid!!" + pos);
        Vector3 exitPosition = pos;

        enterPosition -= player.LinkedEntityComponent.Worker.Origin;
        //exitPosition -= player.LinkedEntityComponent.Worker.Origin;

        spawnEffects(player, enterPosition, exitPosition);

        //var (pos, spawnYaw, spawnPitch) = SpawnPoints.GetRandomSpawnPoint();
        
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

    void spawnEffects(ServerPlayerSkillBehaviour player, Vector3 enterPos, Vector3 exitPos)
    {
        Bountyhunt.EffectInfo info = new Bountyhunt.EffectInfo
        {
            Key = "TeleportEnter",
            Position = enterPos.ToBbhVector(),
            RotationEuler = player.transform.rotation.eulerAngles.ToBbhVector(),
            Parent = Bountyhunt.EffectParent.ROOM,
            PositionIsLocal = false
        };
        player.effectSpawnerComponentCommandSender.SendSpawnEffectCommand(player.entityId, info);
        info = new Bountyhunt.EffectInfo
        {
            Key = "TeleportExit",
            Position = exitPos.ToBbhVector(),
            RotationEuler = player.transform.rotation.eulerAngles.ToBbhVector(),
            Parent = Bountyhunt.EffectParent.ROOM,
            PositionIsLocal = false
        };
        player.effectSpawnerComponentCommandSender.SendSpawnEffectCommand(player.entityId, info);

    }
}
