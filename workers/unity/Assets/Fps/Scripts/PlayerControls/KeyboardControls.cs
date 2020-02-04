using UnityEngine;

namespace Fps.PlayerControls
{
    public class KeyboardControls : MonoBehaviour, IControlProvider
    {
        public float sensitivity;

        public Vector3 Movement
        {
            get
            {
                var directionIndex = Forward & !Backward ? 1 : 0;
                directionIndex += Backward & !Forward ? 2 : 0;
                directionIndex += Right & !Left ? 4 : 0;
                directionIndex += Left & !Right ? 8 : 0;
                return cachedDirectionVectors[directionIndex];
            }
        }

        public float YawDelta => Input.GetAxis("Mouse X")*sensitivity;
        public float PitchDelta => Input.GetAxis("Mouse Y")*sensitivity;
        public bool IsAiming => Input.GetButton("Aim");
        public bool AreSprinting => Input.GetButton("Sprint") && Forward && !Backward;
        public bool JumpPressed => Input.GetButtonDown("Jump");
        public bool ShootPressed => Input.GetButtonDown("Shoot");
        public bool ShootHeld => Input.GetButton("Shoot");
        public bool MenuPressed => Input.GetButtonDown("Menu");
        public bool RespawnPressed => Input.GetButtonDown("Respawn");
        public bool ConnectPressed => Input.GetButtonDown("Respawn");
        public bool ChatPressed => Input.GetButtonDown("Chat");


        private static bool Forward => Input.GetButton("Forward");
        private static bool Backward => Input.GetButton("Backward");
        private static bool Left => Input.GetButton("Left");
        private static bool Right => Input.GetButton("Right");

        private readonly Vector3[] cachedDirectionVectors = new Vector3[16];

        public static KeyboardControls instance;

        private void Awake()
        {
            sensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1);
            instance = this;
            CreateDirectionCache();
        }

        // Cache the direction vectors to avoid having to normalize every time.
        private void CreateDirectionCache()
        {
            for (var i = 0; i < cachedDirectionVectors.Length; i++)
            {
                cachedDirectionVectors[i] = Vector3.zero;
            }

            var forwardRight = new Vector3(1, 0, 1).normalized;
            var forwardLeft = new Vector3(-1, 0, 1).normalized;
            var backwardRight = new Vector3(1, 0, -1).normalized;
            var backwardLeft = new Vector3(-1, 0, -1).normalized;

            cachedDirectionVectors[1] = Vector3.forward;
            cachedDirectionVectors[2] = Vector3.back;
            cachedDirectionVectors[4] = Vector3.right;
            cachedDirectionVectors[5] = forwardRight;
            cachedDirectionVectors[6] = backwardRight;
            cachedDirectionVectors[8] = Vector3.left;
            cachedDirectionVectors[9] = forwardLeft;
            cachedDirectionVectors[10] = backwardLeft;
        }

        

    }
}
