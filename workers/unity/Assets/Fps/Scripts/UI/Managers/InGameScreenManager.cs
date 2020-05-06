using UnityEngine;
using UnityEngine.UI;

namespace Fps.UI
{
    public class InGameScreenManager : MonoBehaviour
    {
        public GameObject RespawnScreen;
        public GameObject Reticle;
        public GameObject Hud;
        public GameObject EscapeScreen;

        public bool InEscapeMenu { private set; get; }

        private bool isPlayerAiming;

        private void OnValidate()
        {
            if (RespawnScreen == null)
            {
                throw new MissingReferenceException("Missing reference to the respawn screen.");
            }

            if (Reticle == null)
            {
                throw new MissingReferenceException("Missing reference to the reticle.");
            }

            if (Hud == null)
            {
                throw new MissingReferenceException("Missing reference to the hud.");
            }

            if (EscapeScreen == null)
            {
                throw new MissingReferenceException("Missing reference to the escape screen.");
            }

        }

        public void OnEnable()
        {
            Hud.SetActive(true);
            Reticle.SetActive(true);
            RespawnScreen.SetActive(false);

            SetEscapeScreen(false);
            SetPlayerAiming(false);
        }

        public void OnDisable()
        {
            InEscapeMenu = false;
            isPlayerAiming = false;
        }

        public void TryOpenSettingsMenu()
        {
            if (!gameObject.activeInHierarchy || RespawnScreen.activeInHierarchy)
            {
                return;
            }

            SetEscapeScreen(!EscapeScreen.activeInHierarchy);
        }

        public void SetEscapeScreen(bool inEscapeScreen)
        {
            EscapeScreen.SetActive(inEscapeScreen);
            Reticle.SetActive(!inEscapeScreen && !isPlayerAiming);


            Cursor.lockState = inEscapeScreen ? CursorLockMode.None : CursorLockMode.Locked;

            if (inEscapeScreen) CursorUI.Instance.Show();
            else CursorUI.Instance.Hide();

            InEscapeMenu = inEscapeScreen;
        }

        public void SetPlayerAiming(bool isAiming)
        {
            isPlayerAiming = isAiming;
            Reticle.SetActive(!isPlayerAiming);
        }
    }
}
