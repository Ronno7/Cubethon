using System;
using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Cubethon
{
    // Publisher: announces what happened without referencing the observers.
    public sealed class GameManager : MonoBehaviour
    {
        public event Action RunStarted;
        public event Action PlayerCrashed;
        public event Action<bool> RunEnded;
        public event Action<bool> ReplayStateChanged;

        public PlayerMovement player;
        public Score score;
        public GameObject completeLevelUI;
        public GameObject gameOverUI;
        public string nextSceneName = "Level02";
        public string menuSceneName = "Menu";
        [HideInInspector] public float restartDelay = 1f;

        public bool HasEnded { get; private set; }
        public bool HasWon { get; private set; }
        private bool recordedWin;
        private GUIStyle messageStyle;

        private void Awake()
        {
            if (player == null) player = FindFirstObjectByType<PlayerMovement>();
            if (score == null) score = FindFirstObjectByType<Score>();
            // The centered result bar replaces the automatic end screens.
            if (completeLevelUI != null) completeLevelUI.SetActive(false);
            if (gameOverUI != null) gameOverUI.SetActive(false);
        }

        private void Start()
        {
            // All observers subscribe before Start, including the first level.
            RunStarted?.Invoke();
        }

        private void Update()
        {
            bool replay = false, retry = false, next = false, menu = false;
#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                replay = keyboard.vKey.wasPressedThisFrame;
                retry = keyboard.rKey.wasPressedThisFrame;
                menu = keyboard.escapeKey.wasPressedThisFrame;
                next = keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame;
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            replay = Input.GetKeyDown(KeyCode.V);
            retry = Input.GetKeyDown(KeyCode.R);
            menu = Input.GetKeyDown(KeyCode.Escape);
            next = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
#endif
            if (menu) ReturnToMenu();
            else if (retry) Restart();
            else if (replay) Replay();
            else if (next) NextLevel();
        }

        // Falls are losses but do not count as obstacle collisions.
        public void EndGame() { EndAttempt(false); }
        public void CompleteLevel() { EndAttempt(true); }

        public void PlayerHitObstacle()
        {
            // Only an accepted live collision raises this event.
            if (EndAttempt(false)) PlayerCrashed?.Invoke();
        }

        private bool EndAttempt(bool won)
        {
            if (player == null || HasEnded || player.IsReplaying) return false;
            recordedWin = won;
            ShowResult(won);
            RunEnded?.Invoke(won);
            return true;
        }

        private void ShowResult(bool won)
        {
            HasEnded = true;
            HasWon = won;
            player.StopAtFinish();
            if (score != null)
            {
                score.Refresh();
                score.enabled = false;
            }
        }

        public void Replay()
        {
            if (player == null || !HasEnded || !player.HasRecording) return;
            HasEnded = false;
            HasWon = false;
            player.BeginReplay();
            if (score != null)
            {
                score.enabled = true;
                score.Refresh();
            }
            ReplayStateChanged?.Invoke(true);
        }

        public void ReplayFinished()
        {
            if (player == null || !player.IsReplaying) return;
            ShowResult(recordedWin);
            ReplayStateChanged?.Invoke(false);
        }

        public void Restart() { SceneLoader.Load(SceneManager.GetActiveScene().name); }
        public void ReturnToMenu() { SceneLoader.Load(menuSceneName); }
        public void NextLevel()
        {
            if (HasEnded && HasWon) SceneLoader.Load(nextSceneName);
        }

        private void OnGUI()
        {
            if (player == null || (!HasEnded && !player.IsReplaying)) return;
            if (messageStyle == null)
                messageStyle = new GUIStyle(GUI.skin.box)
                { fontSize = 18, alignment = TextAnchor.MiddleCenter, wordWrap = true };

            string title = player.IsReplaying ? "REPLAY" : HasWon ? "LEVEL COMPLETE" : "GAME OVER";
            string controls = "R: Retry | Esc: Menu";
            if (HasEnded && player.HasRecording) controls = "V: Replay | " + controls;
            if (HasEnded && HasWon) controls += " | Enter: Next level";
            float width = Mathf.Min(760f, Screen.width - 24f);

            messageStyle.normal.background = Texture2D.whiteTexture;
            messageStyle.hover.background = Texture2D.whiteTexture;
            messageStyle.normal.textColor = Color.black;
            messageStyle.hover.textColor = Color.black;


            Color previousColor = GUI.backgroundColor;
            GUI.backgroundColor = HasWon
                ? new Color(0.1f, 0.6f, 0.1f, 0.8f)  // Green: level completed
                : new Color(0.7f, 0.1f, 0.1f, 0.8f); // Red: otherwise

            GUI.Box(
                new Rect(
                    (Screen.width - width) / 2f,
                    (Screen.height - 100f) / 2f,
                    width,
                    100f),
                title + "\n\n" + controls,
                messageStyle);

            GUI.backgroundColor = previousColor;
        }
    }
}
