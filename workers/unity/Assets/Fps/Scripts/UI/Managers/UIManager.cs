using UnityEngine;
using UnityEngine.Events;

namespace Fps.UI
{
    public class UIManager : MonoBehaviour
    {
        public ScreenManager ScreenManager;
        public static UIManager instance;
        public static UnityEvent onShowGameView = new UnityEvent();
        public static UnityEvent onShowFrontEnd = new UnityEvent();
        public static UnityEvent onToggleEscapeMenu = new UnityEvent();

        public static bool inEscapeMenu = false;

        private void Awake()
        {
            instance = this;
        }

        public void ShowGameView()
        {
            onShowGameView.Invoke();
        }

        public void ShowFrontEnd()
        {
            onShowFrontEnd.Invoke();
        }

        public void ToggleEscapeMenu()
        {
            onToggleEscapeMenu.Invoke();
        }

    }
}
