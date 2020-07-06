using Fps;
using Fps.Respawning;
using Improbable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Fps.SchemaExtensions;

[CreateAssetMenu(menuName = "BBH/Skills/LobbyTeleport", order = 1)]
public class LobbyTeleportSkill : PlayerSkill
{
    public override CastResponse ServerCastSkill(ServerPlayerSkillBehaviour player)
    {
        var teleport = new Vector2(0,0);
        var pos = new Vector3(teleport.x, 75, teleport.y);
        pos = SpawnPoints.SnapToGround(pos);

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
