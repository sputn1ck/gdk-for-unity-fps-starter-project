using System.Text;
using UnityEngine;
using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.PlayerLifecycle;

using Improbable.Gdk.TransformSynchronization;
using Fps;
using Fps.Config;
using Fps.Respawning;
using Fps.Health;
using Fps.Guns;
using Fps.SchemaExtensions;
using Bountyhunt;
using System.Collections.Generic;
using Chat;
using Improbable.Gdk.QueryBasedInterest;

public class DonnerEntityTemplates
{
    public static EntityTemplate Spawner(Coordinates spawnerCoordinates)
    {
        var position = new Position.Snapshot(spawnerCoordinates);
        var metadata = new Metadata.Snapshot("PlayerCreator");
        var template = new EntityTemplate();
        template.AddComponent(position, WorkerUtils.UnityGameLogic);
        template.AddComponent(metadata, WorkerUtils.UnityGameLogic);
        template.AddComponent(new Persistence.Snapshot(), WorkerUtils.UnityGameLogic);
        template.AddComponent(new PlayerCreator.Snapshot(), WorkerUtils.UnityGameLogic);

        template.SetReadAccess(WorkerUtils.UnityGameLogic);
        template.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.UnityGameLogic);

        return template;
    }

    public static EntityTemplate Player(EntityId entityId, string clientWorkerId, byte[] serializedArguments)
    {
        var client = EntityTemplate.GetWorkerAccessAttribute(clientWorkerId);
        // checks login
        // todo reaadd
        var loginData = UnityEngine.JsonUtility.FromJson<LoginData>(Encoding.ASCII.GetString(serializedArguments));
        var playerName = loginData.PlayerName;
        var (spawnPosition, spawnYaw, spawnPitch) = SpawnPoints.GetRandomSpawnPointStatic();

        var serverResponse = new ServerResponse
        {
            Position = spawnPosition.ToVector3Int()
        };

        var rotationUpdate = new RotationUpdate
        {
            Yaw = spawnYaw.ToInt1k(),
            Pitch = spawnPitch.ToInt1k()
        };

        var pos = new Position.Snapshot { Coords = Coordinates.FromUnityVector(spawnPosition) };
        var serverMovement = new ServerMovement.Snapshot { Latest = serverResponse };
        var clientMovement = new ClientMovement.Snapshot { Latest = new ClientRequest() };
        var clientRotation = new ClientRotation.Snapshot { Latest = rotationUpdate };
        var shootingComponent = new ShootingComponent.Snapshot();
        var gunComponent = new GunComponent.Snapshot { GunId = loginData.RequestedWeapon};
        var gunStateComponent = new GunStateComponent.Snapshot { IsAiming = false };
        var healthComponent = new HealthComponent.Snapshot
        {
            Health = PlayerHealthSettings.MaxHealth,
            MaxHealth = PlayerHealthSettings.MaxHealth,
        };

        var healthRegenComponent = new HealthRegenComponent.Snapshot
        {
            CooldownSyncInterval = PlayerHealthSettings.SpatialCooldownSyncInterval,
            DamagedRecently = false,
            RegenAmount = PlayerHealthSettings.RegenAmount,
            RegenCooldownTimer = 0,
            RegenInterval = PlayerHealthSettings.RegenInterval,
            RegenPauseTime = PlayerHealthSettings.RegenAfterDamageCooldown,
        };

        var checkoutQuery = InterestQuery.Query(Constraint.RelativeCylinder(150));
        var serverCheckoutQuery = InterestQuery.Query(Constraint.RelativeCylinder(500));
        var gameManagerQuery = InterestQuery.Query(Constraint.Component(GameStats.ComponentId));
        var worldManager = InterestQuery.Query(Constraint.Component(Bountyhunt.WorldManager.ComponentId));
        var bountyTracerQuery = InterestQuery.Query(Constraint.Component(TracerComponent.ComponentId));
        var interestTemplate = InterestTemplate.Create().AddQueries<Position.Component>(serverCheckoutQuery).AddQueries<ClientMovement.Component>( checkoutQuery, gameManagerQuery, bountyTracerQuery, worldManager);
        var interestComponent = interestTemplate.ToSnapshot();

        // NEW STUFF

        var hunterComponent = new HunterComponent.Snapshot()
        {
            Name = playerName,
            Pubkey = loginData.Pubkey
        };
        var skillComponent = new PlayerSkillComponent.Snapshot()
        {
            Skills = new List<int>() { 0 },
        };
        var chatComponent = new PrivateChat.Snapshot();

        var effectSpawnerComponent = new EffectSpawnerComponent.Snapshot();

        var roomCompenent = new RoomPlayer.Snapshot()
        {
            Pubkey = loginData.Pubkey,
            ActiveRoom = new RoomBaseInfo(), 
        };
        /*
         * OLD STUFF
        var donnerinfocomponent = new Donner.DonnerInfo.Snapshot()
        { Bounty = 0, Pot = 0 };
        var donnerplayercomponent = new Donner.DonnerPlayer.Snapshot();
        var donnerteleportcomponent = new Donner.PaidTeleport.Snapshot();
        var bountyboardcomponent = new Donner.BountyBoard.Snapshot()
        {
            Board = new System.Collections.Generic.List<Donner.BountyBoardItem>()
        };
        var lightninginfo = new Donner.LightningNode.Snapshot();


        var playerStateComponent = new PlayerState.Snapshot
        {
            Name = playerName,
            Kills = 0,
            Deaths = 0,
        };

        var privateChat = new Chat.PrivateChat.Snapshot();
        */
        var template = new EntityTemplate();
        template.AddComponent(pos, WorkerUtils.UnityGameLogic);
        template.AddComponent(new Metadata.Snapshot { EntityType = "Player" }, WorkerUtils.UnityGameLogic);
        template.AddComponent(serverMovement, WorkerUtils.UnityGameLogic);
        template.AddComponent(clientMovement, client);
        template.AddComponent(clientRotation, client);
        template.AddComponent(shootingComponent, client);
        template.AddComponent(gunComponent, WorkerUtils.UnityGameLogic);
        template.AddComponent(gunStateComponent, client);
        template.AddComponent(healthComponent, WorkerUtils.UnityGameLogic);
        template.AddComponent(healthRegenComponent, WorkerUtils.UnityGameLogic);
        template.AddComponent(interestComponent, WorkerUtils.UnityGameLogic);

        // NEW STUFF
        template.AddComponent(hunterComponent, WorkerUtils.UnityGameLogic);
        template.AddComponent(skillComponent, WorkerUtils.UnityGameLogic);
        template.AddComponent(chatComponent, client);
        template.AddComponent(roomCompenent, WorkerUtils.UnityGameLogic);
        template.AddComponent(effectSpawnerComponent, client);
        /*
         * OLD STUFF
        template.AddComponent(playerStateComponent, WorkerUtils.UnityGameLogic);
        template.AddComponent(donnerinfocomponent, WorkerUtils.UnityGameLogic);
        template.AddComponent(donnerplayercomponent, client);
        template.AddComponent(donnerteleportcomponent, WorkerUtils.UnityGameLogic);
        template.AddComponent(bountyboardcomponent, WorkerUtils.UnityGameLogic);
        template.AddComponent(lightninginfo, WorkerUtils.UnityGameLogic);
        template.AddComponent(privateChat, client);
        */

        PlayerLifecycleHelper.AddPlayerLifecycleComponents(template, clientWorkerId, WorkerUtils.UnityGameLogic);

        template.SetReadAccess(WorkerUtils.UnityClient, WorkerUtils.UnityGameLogic, WorkerUtils.MobileClient);
        template.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.UnityGameLogic);

        switch (loginData.RequestedWeapon)
        {
            case (0):
                PrometheusManager.TotalSoldiersChosen.Inc();
                break;
            case (1):
                PrometheusManager.TotalSnipersChosen.Inc();
                break;
            case (2):
                PrometheusManager.TotalScoutsChosen.Inc();
                break;
            default:
                break;
        }
        return template;
    }

    public static EntityTemplate BountyPickup(Vector3 position, long satValue)
    {
        var bountyPickUpComponent = new Bountyhunt.BountyPickup.Snapshot(true, satValue);

        var entityTemplate = new EntityTemplate();
        entityTemplate.AddComponent(new Position.Snapshot(Coordinates.FromUnityVector(position)), WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(new Metadata.Snapshot("BountyPickup"), WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(new Persistence.Snapshot(), WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(bountyPickUpComponent, WorkerUtils.UnityGameLogic);


        entityTemplate.SetReadAccess(WorkerUtils.UnityGameLogic, WorkerUtils.UnityClient);
        entityTemplate.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.UnityGameLogic);

        //TransformSynchronizationHelper.AddTransformSynchronizationComponents(entityTemplate, WorkerUtils.UnityGameLogic);

        return entityTemplate;
    }

    public static EntityTemplate RoomManager(Vector3 position, Room room)
    {
        var roomManagerComponent = new RoomManager.Snapshot();
        roomManagerComponent.RoomInfo = room;
        roomManagerComponent.RoomState = RoomState.CREATED;
        var roomStatsComponent = new RoomStatsManager.Snapshot();
        var roomGameModeComponent = new RoomGameModeManager.Snapshot();
        var roomAdComponent = new RoomAdvertingManager.Snapshot();
        if (room.FinanceInfo.FixedAdvertisers.HasValue)
        {
            roomAdComponent.CurrentAdvertisers = room.FinanceInfo.FixedAdvertisers.Value.Advertisers;
        } else
        {
            roomAdComponent.CurrentAdvertisers = new List<AdvertiserSource>();
        }

        var entityTemplate = new EntityTemplate();
        entityTemplate.AddComponent(new Position.Snapshot(Coordinates.FromUnityVector(position)), WorkerUtils.UnityGameLogic);
        
        entityTemplate.AddComponent(new Metadata.Snapshot("RoomManager"), WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(new Persistence.Snapshot(), WorkerUtils.UnityGameLogic);

        entityTemplate.AddComponent(roomManagerComponent, WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(roomStatsComponent, WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(roomGameModeComponent, WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(roomAdComponent, WorkerUtils.UnityGameLogic);

        entityTemplate.SetReadAccess(WorkerUtils.UnityGameLogic, WorkerUtils.UnityClient);
        entityTemplate.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.UnityGameLogic);

        return entityTemplate;
    }

    public static EntityTemplate WorldManager(Vector3 position)
    {
        var worldManagerComponent = new WorldManager.Snapshot();
        worldManagerComponent.ActiveRooms = new Dictionary<string, Room>();
        worldManagerComponent.ActivePlayers = new Dictionary<string, Bountyhunt.PlayerInfo>();

        var entityTemplate = new EntityTemplate(); entityTemplate.AddComponent(new Position.Snapshot(Coordinates.FromUnityVector(position)), WorkerUtils.UnityGameLogic);

        entityTemplate.AddComponent(new Metadata.Snapshot("WorldManager"), WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(new Persistence.Snapshot(), WorkerUtils.UnityGameLogic);

        entityTemplate.AddComponent(worldManagerComponent, WorkerUtils.UnityGameLogic);

        entityTemplate.SetReadAccess(WorkerUtils.UnityGameLogic, WorkerUtils.UnityClient);
        entityTemplate.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.UnityGameLogic);

        return entityTemplate;
    }
    public static EntityTemplate GameManager(Vector3 position)
    {
        var boutySpawnerComponent = new BountySpawner.Snapshot();
        var gameStatsComponent = new GameStats.Snapshot();
        gameStatsComponent.PlayerMap = new Dictionary<EntityId, PlayerItem>();

        gameStatsComponent.LastRoundScores = new Dictionary<EntityId, PlayerItem>();
        var chatComponent = new Chat.ChatComponent.Snapshot();

        var advertisingComponent = new AdvertisingComponent.Snapshot();
        advertisingComponent.CurrentAdvertisers = new List<AdvertiserSource>();

        var entityTemplate = new EntityTemplate(); entityTemplate.AddComponent(new Position.Snapshot(Coordinates.FromUnityVector(position)), WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(new Metadata.Snapshot("GameManager"), WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(new Persistence.Snapshot(), WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(boutySpawnerComponent, WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(gameStatsComponent, WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(chatComponent, WorkerUtils.UnityGameLogic);



        entityTemplate.AddComponent(advertisingComponent, WorkerUtils.UnityGameLogic);

        entityTemplate.SetReadAccess(WorkerUtils.UnityGameLogic, WorkerUtils.UnityClient);
        entityTemplate.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.UnityGameLogic);

        return entityTemplate;
    }

    public static EntityTemplate BountyTracer(Vector3 position, long attachedHunter)
    {
        var tracerComponent = new TracerComponent.Snapshot() { AttachedHunter = attachedHunter };

        var entityTemplate = new EntityTemplate();
        entityTemplate.AddComponent(new Position.Snapshot(Coordinates.FromUnityVector(position)), WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(new Metadata.Snapshot("BountyTracer"), WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(new Persistence.Snapshot(), WorkerUtils.UnityGameLogic);

        entityTemplate.AddComponent(tracerComponent, WorkerUtils.UnityGameLogic);

        entityTemplate.SetReadAccess(WorkerUtils.UnityGameLogic, WorkerUtils.UnityClient);
        entityTemplate.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.UnityGameLogic);

        return entityTemplate;
    }

    public static EntityTemplate LevelCube(Vector3 position, Vector3 scale, UnityEngine.Quaternion rotation, EntityId roomId)
    {
        var transform = new TransformSync.Snapshot()
        {
            Transform = new Bountyhunt.TransformData(Utility.Vector3ToVector3Float(scale), Utility.QuatToBhQuat(rotation))
        };
        var roombound = new RoomBoundObject.Snapshot()
        {
            RoomEntityId = roomId
        };
        var entityTemplate = new EntityTemplate();
        entityTemplate.AddComponent(new Position.Snapshot(Coordinates.FromUnityVector(position)), WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(new Metadata.Snapshot("LevelCube"), WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(new Persistence.Snapshot(), WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(transform, WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(roombound, WorkerUtils.UnityGameLogic);

        entityTemplate.SetReadAccess(WorkerUtils.UnityGameLogic, WorkerUtils.UnityClient);
        entityTemplate.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.UnityGameLogic);

        return entityTemplate;
    }

    public static EntityTemplate MovingPlatform(Vector3 position, Vector3 scale, UnityEngine.Quaternion rotation, EntityId roomId)
    {
        var transform = new TransformSync.Snapshot()
        {
            Transform = new Bountyhunt.TransformData(Utility.Vector3ToVector3Float(scale), Utility.QuatToBhQuat(rotation))
        };
        var roombound = new RoomBoundObject.Snapshot()
        {
            RoomEntityId = roomId
        };
        var animator = new AnimatorSync.Snapshot();
        var entityTemplate = new EntityTemplate();
        entityTemplate.AddComponent(new Position.Snapshot(Coordinates.FromUnityVector(position)), WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(new Metadata.Snapshot("MovingPlatform"), WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(new Persistence.Snapshot(), WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(transform, WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(roombound, WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(animator, WorkerUtils.UnityGameLogic);

        entityTemplate.SetReadAccess(WorkerUtils.UnityGameLogic, WorkerUtils.UnityClient);
        entityTemplate.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.UnityGameLogic);

        return entityTemplate;
    }
}
