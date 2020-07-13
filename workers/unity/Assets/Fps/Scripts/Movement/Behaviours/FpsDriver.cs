using System;
using System.Collections;
using Fps.Animation;
using Fps.Guns;
using Fps.PlayerControls;
using Fps.SchemaExtensions;
using Fps.UI;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using UnityEngine;

namespace Fps.Movement
{
    public class FpsDriver : MonoBehaviour
    {
        [Serializable]
        private struct CameraSettings
        {
            public float PitchSpeed;
            public float YawSpeed;
            public float MinPitch;
            public float MaxPitch;
        }

        [Require] private ClientMovementWriter authority;
        [Require] private ServerMovementReader serverMovement;
        [Require] private GunStateComponentWriter gunState;
        [Require] private HealthComponentReader health;
        [Require] private HealthComponentCommandSender commandSender;
        [Require] private EntityId entityId;

        private ClientMovementDriver movement;
        private ClientShooting shooting;
        private ShotRayProvider shotRayProvider;
        private FpsAnimator fpsAnimator;
        private GunManager currentGun;
        private ClientPlayerSkillBehaviour skillBehaviour;
        private ViewChanger viewChanger;

        public bool editingInputfield;

        [SerializeField] private Transform pitchTransform;
        public new Camera camera;
        public GameObject ThirdPersonCameraSocket;


        [SerializeField] private CameraSettings cameraSettings = new CameraSettings
        {
            PitchSpeed = 1.0f,
            YawSpeed = 1.0f,
            MinPitch = -80.0f,
            MaxPitch = 60.0f
        };

        private bool isRequestingRespawn;
        private Coroutine requestingRespawnCoroutine;

        private IControlProvider controller;

        public static FpsDriver instance;
        private void Awake()
        {
            instance = this;
            movement = GetComponent<ClientMovementDriver>();
            shooting = GetComponent<ClientShooting>();
            shotRayProvider = GetComponent<ShotRayProvider>();
            fpsAnimator = GetComponent<FpsAnimator>();
            fpsAnimator.InitializeOwnAnimator();
            currentGun = GetComponent<GunManager>();
            controller = GetComponent<IControlProvider>();
            skillBehaviour = GetComponent<ClientPlayerSkillBehaviour>();
            viewChanger = GetComponent<ViewChanger>();

            var uiManager = GameObject.FindGameObjectWithTag("OnScreenUI")?.GetComponent<BBHUIManager>();
            if (uiManager == null)
            {
                throw new NullReferenceException("Was not able to find the OnScreenUI prefab in the scene.");
            }

        }

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            CursorUI.Instance.Hide();
            serverMovement.OnForcedRotationEvent += OnForcedRotation;
            health.OnRespawnEvent += OnRespawn;
            UnityEngine.EventSystems.EventSystem.current.sendNavigationEvents = false;

        }

        private void Update()
        {
            if (UnityEngine.EventSystems.EventSystem.current.IsEditingInpputfield()||ChatPanelUI.instance.chatting)
            {
                movement.ApplyMovement(Vector3.zero, transform.rotation, MovementSpeed.Run, false);
                Animations(false);
                return;
            }
            
            if (controller.ChatPressed)
            {
                ChatPanelUI.instance.StartChatInput();
                return;
            }

            if (controller.MenuPressed)
            {
                BBHUIManager.instance.inGame.ToggleEscapeScreen();
            }

            if (BBHUIManager.instance.inGame.EscapeScreen.activated)
            {
                movement.ApplyMovement(Vector3.zero, transform.rotation, MovementSpeed.Run, false);
                Animations(false);
                return;
            }

            if (isRequestingRespawn)
            {
                return;
            }

            if (health.Data.Health == 0)
            {
                if (controller.RespawnPressed)
                {
                    ((RespawnScreenUI)BBHUIManager.instance.inGame.RespawnScreen).Respawn();
                }
                return;
            }
            for (int i = 0; i < SkillDictionary.Count; i++)
            {
                if (Input.GetKeyDown(SkillDictionary.Get(i).key))
                {
                    skillBehaviour.CastSkill(i);
                }
            }
            // Movement
            var toMove = transform.rotation * controller.Movement;

            // Rotation
            var yawDelta = controller.YawDelta;
            var pitchDelta = controller.PitchDelta;

            // Modifiers
            var isAiming = controller.IsAiming;
            var isSprinting = controller.AreSprinting;

            var isJumpPressed = controller.JumpPressed;

            // Events
            var shootPressed = controller.ShootPressed;
            var shootHeld = controller.ShootHeld;


            // Update the pitch speed with that of the gun if aiming.
            var yawSpeed = cameraSettings.YawSpeed;
            var pitchSpeed = cameraSettings.PitchSpeed;
            if (isAiming)
            {
                yawSpeed = currentGun.CurrentGunSettings.AimYawSpeed;
                pitchSpeed = currentGun.CurrentGunSettings.AimPitchSpeed;
            }

            //setCamera
            if (InputKeyMapping.MappedKeyDown("ThirdPerson_Key"))
            {
                viewChanger.SetThirdPersonView(true);
            }
            else if (InputKeyMapping.MappedKeyUp("ThirdPerson_Key"))
            {
                viewChanger.SetThirdPersonView(false);
            }

            if (viewChanger.thirdPerson)
            {
                viewChanger.UpdateThirdPersonView(yawDelta,pitchDelta);
                yawDelta = 0;
                pitchDelta = 0;
            }

            //Mediator
            var movementSpeed = isAiming
                ? MovementSpeed.Walk
                : isSprinting
                    ? MovementSpeed.Sprint
                    : MovementSpeed.Run;
            var yawChange = yawDelta * yawSpeed;
            var pitchChange = pitchDelta * -pitchSpeed;
            var currentPitch = pitchTransform.transform.localEulerAngles.x;
            var newPitch = currentPitch + pitchChange;
            if (newPitch > 180)
            {
                newPitch -= 360;
            }

            newPitch = Mathf.Clamp(newPitch, -cameraSettings.MaxPitch, -cameraSettings.MinPitch);
            pitchTransform.localRotation = Quaternion.Euler(newPitch, 0, 0);
            var currentYaw = transform.eulerAngles.y;
            var newYaw = currentYaw + yawChange;
            var rotation = Quaternion.Euler(newPitch, newYaw, 0);

            //Check for sprint cooldown
            if (!movement.HasSprintedRecently && !viewChanger.thirdPerson)
            {
                HandleShooting(shootPressed, shootHeld);
            }

            Aiming(isAiming);

            var wasGroundedBeforeMovement = movement.IsGrounded;
            movement.ApplyMovement(toMove, rotation, movementSpeed, isJumpPressed);
            Animations(isJumpPressed && wasGroundedBeforeMovement);
        }

        public void Respawn()
        {
            isRequestingRespawn = true;
            requestingRespawnCoroutine = StartCoroutine(RequestRespawn());
        }

        private IEnumerator RequestRespawn()
        {
            while (isRequestingRespawn)
            {
                commandSender?.SendRequestRespawnCommand(entityId, new Empty());
                yield return new WaitForSeconds(2);
            }
        }

        private void OnRespawn(Empty _)
        {
            StopCoroutine(requestingRespawnCoroutine);
            isRequestingRespawn = false;
        }

        private void HandleShooting(bool shootingPressed, bool shootingHeld)
        {
            if (shootingPressed)
            {
                shooting.BufferShot();
            }

            var isShooting = shooting.IsShooting(shootingHeld);
            if (isShooting)
            {
                FireShot(currentGun.CurrentGunSettings);
            }
        }

        private void FireShot(GunSettings gunSettings)
        {
            if (gunSettings.PelletsPerShot > 1)
            {
                var rays = shotRayProvider.GetShotgunRays(gunState.Data.IsAiming, camera, gunSettings.PelletsPerShot);
                for (int i = 0; i < rays.Length; i++)
                {
                    shooting.FireShot(gunSettings.ShotRange, rays[i]);
                }

                shooting.InitiateCooldown(gunSettings.ShotCooldown);

            }
            else
            {

                var ray = shotRayProvider.GetShotRay(gunState.Data.IsAiming, camera);
                shooting.FireShot(gunSettings.ShotRange, ray);
                shooting.InitiateCooldown(gunSettings.ShotCooldown);
            }
        }

        private void Aiming(bool shouldBeAiming)
        {
            if (shouldBeAiming != gunState.Data.IsAiming)
            {
                var update = new GunStateComponent.Update
                {
                    IsAiming = shouldBeAiming
                };
                gunState.SendUpdate(update);
            }
        }

        public void Animations(bool isJumping)
        {
            fpsAnimator.SetAiming(gunState.Data.IsAiming);
            fpsAnimator.SetGrounded(movement.IsGrounded);
            fpsAnimator.SetMovement(transform.position, Time.deltaTime);
            fpsAnimator.SetPitch(pitchTransform.transform.localEulerAngles.x);

            if (isJumping)
            {
                fpsAnimator.Jump();
            }
        }
        public void ChangeGunId(int newGunId)
        {
            gunState.SendUpdate(new GunStateComponent.Update() {  NewGunId= newGunId });
        }

        public int GetGunId()
        {
            return gunState.Data.NewGunId;
        }

        private void OnForcedRotation(RotationUpdate forcedRotation)
        {
            var newPitch = Mathf.Clamp(forcedRotation.Pitch.ToFloat1k(), -cameraSettings.MaxPitch,
                -cameraSettings.MinPitch);
            pitchTransform.localRotation = Quaternion.Euler(newPitch, 0, 0);
        }

        public EntityId getEntityID()
        {
            return entityId;
        }
    }
}
