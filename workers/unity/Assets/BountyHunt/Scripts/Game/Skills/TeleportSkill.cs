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
    public override void ServerCastSkill(ServerPlayerSkillBehaviour player)
    {
        JsonUtility.FromJson("", typeof(TeleportPayload));
        var forward2d = new Vector2(player.transform.forward.x, player.transform.forward.z).normalized * this.Distance;
        var teleport = new Vector2(player.transform.position.x + forward2d.x, player.transform.position.z + forward2d.y);
        var pos = new Vector3(teleport.x,75, teleport.y);
        pos = SpawnPoints.SnapToGround(pos);
        Debug.Log("teleport invoice paid!!" + pos);


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
