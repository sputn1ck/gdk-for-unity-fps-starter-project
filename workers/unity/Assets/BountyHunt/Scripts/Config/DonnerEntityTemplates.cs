using System.Text;
using UnityEngine;
using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Gdk.QueryBasedInterest;

using Improbable.Gdk.TransformSynchronization;
using Fps;
using Fps.Config;
using Fps.Respawning;
using Fps.Health;
using Fps.Guns;
using Fps.SchemaExtensions;
using Bountyhunt;

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

    public static EntityTemplate Player(string workerId, byte[] args)
    {
        var client = EntityTemplate.GetWorkerAccessAttribute(workerId);
        // checks login
        // todo reaadd
        var loginData = UnityEngine.JsonUtility.FromJson<LoginData>(Encoding.ASCII.GetString(args));
        var playerName = loginData.PlayerName;
        var (spawnPosition, spawnYaw, spawnPitch) = SpawnPoints.GetRandomSpawnPoint();

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
        var gunComponent = new GunComponent.Snapshot { GunId = PlayerGunSettings.DefaultGunIndex };
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
            RegenCooldownTimer = PlayerHealthSettings.RegenAfterDamageCooldown,
            RegenInterval = PlayerHealthSettings.RegenInterval,
            RegenPauseTime = 0,
        };

        var checkoutQuery = InterestQuery.Query(Constraint.RelativeCylinder(150));

        var interestTemplate = InterestTemplate.Create().AddQueries<ClientMovement.Component>( checkoutQuery);
        var interestComponent = interestTemplate.ToSnapshot();

        // NEW STUFF

        var hunterComponent = new HunterComponent.Snapshot()
        {
            Earnings = 0,
            Bounty = 0,
            Name = playerName
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

        PlayerLifecycleHelper.AddPlayerLifecycleComponents(template, workerId, WorkerUtils.UnityGameLogic);

        template.SetReadAccess(WorkerUtils.UnityClient, WorkerUtils.UnityGameLogic, WorkerUtils.MobileClient);
        template.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.UnityGameLogic);
        /*
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
        }*/
        return template;
    }
    /*
    public static EntityTemplate SpectatorTemplate(string workerId, byte[] args)
    {

        var (spawnPosition, spawnYaw, spawnPitch) = SpawnPoints.GetRandomSpawnPoint();
        var pos = new Position.Snapshot { Coords = Coordinates.FromUnityVector(spawnPosition) };
        var sessionQuery = InterestQuery.Query(Constraint.Component<Session.Component>());
        var checkoutQuery = InterestQuery.Query(Constraint.RelativeCylinder(150));
        var nodeInfoQuery = InterestQuery.Query(Constraint.Component(Donner.GamePotManager.ComponentId));
    }*/

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

    /*
     * OLD GAMEMANAGER
     * TODO: READD
    public static EntityTemplate NodeInfo(Vector3 position)
    {
        var nodeinfocomponent = new Donner.LightningNode.Snapshot();
        var gamepotcomponent = new Donner.GamePotManager.Snapshot();
        var chatComponent = new Chat.Chat.Snapshot();
        var entityTemplate = new EntityTemplate();
        entityTemplate.AddComponent(new Position.Snapshot(Coordinates.FromUnityVector(position)), WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(new Metadata.Snapshot("NodeInfo"), WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(new Persistence.Snapshot(), WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(nodeinfocomponent, WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(gamepotcomponent, WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(chatComponent, WorkerUtils.UnityGameLogic);

        entityTemplate.SetReadAccess(WorkerUtils.UnityGameLogic, WorkerUtils.UnityClient);
        entityTemplate.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.UnityGameLogic);

        return entityTemplate;
    }*/

    public static EntityTemplate GameManager(Vector3 position)
    {
        var boutySpawnerComponent = new BountySpawner.Snapshot();
        var gameStatsComponent = new GameStats.Snapshot();
        gameStatsComponent.Scoreboard = new Scoreboard(new System.Collections.Generic.List<ScoreboardItem>());
        gameStatsComponent.PlayerNames = new System.Collections.Generic.Dictionary<EntityId, string>();
        var entityTemplate = new EntityTemplate(); entityTemplate.AddComponent(new Position.Snapshot(Coordinates.FromUnityVector(position)), WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(new Metadata.Snapshot("GameManager"), WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(new Persistence.Snapshot(), WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(boutySpawnerComponent, WorkerUtils.UnityGameLogic);
        entityTemplate.AddComponent(gameStatsComponent, WorkerUtils.UnityGameLogic);

        entityTemplate.SetReadAccess(WorkerUtils.UnityGameLogic, WorkerUtils.UnityClient);
        entityTemplate.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.UnityGameLogic);

        return entityTemplate;
    }
}