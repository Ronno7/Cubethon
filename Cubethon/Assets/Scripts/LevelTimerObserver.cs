using UnityEngine;

namespace Cubethon
{
    [RequireComponent(typeof(GameManager))]
    public sealed class LevelTimerObserver : MonoBehaviour
    {
        private GameManager manager;
        private float startedAt, elapsed;
        private bool running;
        private GUIStyle style;

        public float ElapsedSeconds
        {
            get { return running ? Mathf.Max(0f, Time.time - startedAt) : elapsed; }
        }

        private void OnEnable()
        {
            manager = GetComponent<GameManager>();
            manager.RunStarted += StartTimer;
            manager.RunEnded += StopTimer;
        }

        private void OnDisable()
        {
            if (manager == null) return;
            manager.RunStarted -= StartTimer;
            manager.RunEnded -= StopTimer;
        }

        private void StartTimer()
        {
            startedAt = Time.time;
            elapsed = 0f;
            running = true;
        }

        private void StopTimer(bool won)
        {
            elapsed = ElapsedSeconds;
            running = false;
        }

        private void OnGUI()
        {
            if (style == null) style = new GUIStyle(GUI.skin.box) { fontSize = 18 };
            GUI.Box(new Rect(Screen.width - 264f, 56f, 248f, 36f),
                "Attempt time: " + ElapsedSeconds.ToString("0.00") + " s", style);
        }
    }
}
