using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Commands;
using Improbable.Gdk.Core.Commands;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Fps;
using Bountyhunt;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.Subscriptions;
using Improbable;
using Improbable.Gdk.PlayerLifecycle;

[UpdateAfter(typeof(SpatialOSUpdateGroup))]
public class ServerDisconnectSystem : ComponentSystem
{
    private WorkerSystem workerSystem;
    private CommandSystem commandSystem;

    private ComponentUpdateSystem componentUpdateSystem;
    private EntityQuery query;
    private EntityQuery newQuery;



    protected override void OnCreate()
    {
        base.OnCreate();

        workerSystem = World.GetExistingSystem<WorkerSystem>();
        commandSystem = World.GetExistingSystem<CommandSystem>();
        componentUpdateSystem = World.GetExistingSystem<ComponentUpdateSystem>();

        query = GetEntityQuery(
                ComponentType.ReadOnly<HeartbeatData>(),
                ComponentType.ReadOnly<HunterComponent.Component>(),
                ComponentType.ReadOnly<SpatialEntityId>(),
                ComponentType.ReadOnly<Position.Component>()
            );
        newQuery = GetEntityQuery(
                ComponentType.ReadOnly<HeartbeatData>(),
                ComponentType.ReadOnly<RoomPlayer.Component>(),
                ComponentType.ReadOnly<HunterComponent.Component>(),
                ComponentType.ReadOnly<SpatialEntityId>(),
                ComponentType.ReadOnly<Position.Component>()
            );

    }


    protected override void OnUpdate()
    {
        Entities.With(query).ForEach((ref HunterComponent.Component donnerinfo, ref HeartbeatData heartbeat, ref SpatialEntityId entityId, ref Position.Component pos) =>
        {
            if (heartbeat.NumFailedHeartbeats > PlayerLifecycleConfig.MaxNumFailedPlayerHeartbeats - 1)
            {
                commandSystem.SendCommand(new GameStats.RemoveName.Request { TargetEntityId = new EntityId(2), Payload = new RemoveNameRequest { Id = entityId.EntityId } });
                if (donnerinfo.Bounty > 0)
                {
                    commandSystem.SendCommand<BountySpawner.SpawnBountyPickup.Request>(new BountySpawner.SpawnBountyPickup.Request
                    {
                        TargetEntityId = new EntityId(2),
                        Payload = new SpawnBountyPickupRequest
                        {
                            BountyValue = donnerinfo.Bounty + donnerinfo.Earnings,
                            Position = new Vector3Float
                            {
                                X = (float)pos.Coords.X,
                                Y = (float)pos.Coords.Y,
                                Z = (float)pos.Coords.Z
                            }
                        }
                       
                    });

                }
                if(donnerinfo.Earnings > 0)
                {
                    ServerServiceConnections.instance.lnd.KeysendBufferDeposit(ServerServiceConnections.instance.PlatformPubkey, donnerinfo.Pubkey, donnerinfo.Earnings);
                }

                componentUpdateSystem.SendUpdate<HunterComponent.Update>(new HunterComponent.Update { Bounty = 0, Earnings = 0 }, entityId.EntityId);
            }
        });
        Entities.With(query).ForEach((ref HunterComponent.Component donnerinfo, ref RoomPlayer.Component roomplayer, ref HeartbeatData heartbeat, ref SpatialEntityId entityId, ref Position.Component pos) =>
        {
            if (heartbeat.NumFailedHeartbeats > PlayerLifecycleConfig.MaxNumFailedPlayerHeartbeats - 1)
            {
                commandSystem.SendCommand<WorldManager.RemoveActivePlayer.Request>(new WorldManager.RemoveActivePlayer.Request {
                    TargetEntityId = new EntityId(3),
                    Payload = new RemoveActivePlayerRequest
                    {
                        PlayerPk = donnerinfo.Pubkey
                    }
                });
                commandSystem.SendCommand<RoomManager.RemovePlayer.Request>(new RoomManager.RemovePlayer.Request
                {
                    TargetEntityId = roomplayer.RoomEntityid,
                    Payload = new RemovePlayerRequest(donnerinfo.Pubkey)
                });

            }
        });
    }
    
}
