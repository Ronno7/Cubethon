using UnityEngine;
using UnityEngine.SceneManagement;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Cubethon
{
    public class GameManager : MonoBehaviour
    {
        public PlayerMovement player;
        public Score score;
        public GameObject completeLevelUI;
        public GameObject gameOverUI;
        public string nextSceneName;
        public string menuSceneName = "Menu";

        // Preserves the existing serialized field.
        [HideInInspector] public float restartDelay = 1f;

        public bool HasEnded { get; private set; }
        public bool HasWon { get; private set; }

        private bool recordedWin;
        private GUIStyle messageStyle;

        private void Awake()
        {
            if (player == null)
                player = FindFirstObjectByType<PlayerMovement>();

            if (score == null)
                score = FindFirstObjectByType<Score>();

            // The result bar below replaces the automatic end screens.
            // Keeping the completion animation inactive also prevents
            // its LoadNextLevel event from advancing automatically.
            if (completeLevelUI != null)
                completeLevelUI.SetActive(false);

            if (gameOverUI != null)
                gameOverUI.SetActive(false);
        }

        private void Update()
        {
            bool replay = false;
            bool retry = false;
            bool next = false;
            bool menu = false;

#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;

            if (keyboard != null)
            {
                replay = keyboard.vKey.wasPressedThisFrame;
                retry = keyboard.rKey.wasPressedThisFrame;
                menu = keyboard.escapeKey.wasPressedThisFrame;
                next = keyboard.enterKey.wasPressedThisFrame
                    || keyboard.numpadEnterKey.wasPressedThisFrame;
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            replay = Input.GetKeyDown(KeyCode.V);
            retry = Input.GetKeyDown(KeyCode.R);
            menu = Input.GetKeyDown(KeyCode.Escape);
            next = Input.GetKeyDown(KeyCode.Return)
                || Input.GetKeyDown(KeyCode.KeypadEnter);
#endif

            if (menu)
                ReturnToMenu();
            else if (retry)
                Restart();
            else if (replay)
                Replay();
            else if (next)
                NextLevel();
        }

        public void EndGame()
        {
            EndAttempt(false);
        }

        public void CompleteLevel()
        {
            EndAttempt(true);
        }

        private void EndAttempt(bool won)
        {
            if (player == null || HasEnded || player.IsReplaying)
                return;

            recordedWin = won;
            ShowResult(won);
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
            if (player == null || !HasEnded || !player.HasRecording)
                return;

            HasEnded = false;
            HasWon = false;
            player.BeginReplay();

            if (score != null)
            {
                score.enabled = true;
                score.Refresh();
            }
        }

        public void ReplayFinished()
        {
            if (player != null && player.IsReplaying)
                ShowResult(recordedWin);
        }

        public void Restart()
        {
            SceneLoader.Load(SceneManager.GetActiveScene().name);
        }

        public void ReturnToMenu()
        {
            SceneLoader.Load(menuSceneName);
        }

        public void NextLevel()
        {
            if (HasEnded && HasWon)
                SceneLoader.Load(nextSceneName);
        }

        private void OnGUI()
        {
            if (player == null || (!HasEnded && !player.IsReplaying))
                return;

            if (messageStyle == null)
            {
                messageStyle = new GUIStyle(GUI.skin.box)
                {
                    fontSize = 18,
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true
                };
            }

            string title = player.IsReplaying
                ? "REPLAY"
                : HasWon ? "LEVEL COMPLETE" : "GAME OVER";

            string controls = "R: Retry | Esc: Menu";

            if (HasEnded && player.HasRecording)
                controls = "V: Replay | " + controls;

            if (HasEnded && HasWon)
                controls += " | Enter: Next level";

            float width = Mathf.Min(760f, Screen.width - 24f);

            GUI.Box(
                new Rect(
                    (Screen.width - width) / 2f,
                    (Screen.height - 100f) / 2f,
                    width,
                    100f),
                title + "\n\n" + controls,
                messageStyle);
        }
    }
}