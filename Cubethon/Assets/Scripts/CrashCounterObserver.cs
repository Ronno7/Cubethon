using UnityEngine;

namespace Cubethon
{
    [RequireComponent(typeof(GameManager))]
    public sealed class CrashCounterObserver : MonoBehaviour
    {
        public static int SessionCrashes { get; private set; }
        private GameManager manager;
        private GUIStyle style;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSession() { SessionCrashes = 0; }

        private void OnEnable()
        {
            manager = GetComponent<GameManager>();
            manager.PlayerCrashed += CountCrash;
        }

        private void OnDisable()
        {
            if (manager != null) manager.PlayerCrashed -= CountCrash;
        }

        private void CountCrash() { SessionCrashes++; }

        private void OnGUI()
        {
            if (style == null) style = new GUIStyle(GUI.skin.box) { fontSize = 18 };
            GUI.Box(new Rect(Screen.width - 264f, 16f, 248f, 36f),
                "Session collisions: " + SessionCrashes, style);
        }
    }
}
