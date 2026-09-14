using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cubethon
{
    // Episodes 8 and 9: one result per run, automatic retry, and level completion.
    public sealed class GameManager : MonoBehaviour
    {
        public PlayerMovement player;
        public Score score;
        public GameObject completeLevelUI;
        public GameObject gameOverUI;
        public float restartDelay = 1f;
        public string nextSceneName = "Level02";
        public string menuSceneName = "Menu";

        public bool HasEnded { get; private set; }
        public bool HasWon { get; private set; }

        private void Update()
        {
            if (GameInput.MenuPressed) ReturnToMenu();
            else if (GameInput.RestartPressed) Restart();
        }

        public void EndGame()
        {
            if (HasEnded) return;
            HasEnded = true;
            if (player != null) player.enabled = false;
            FreezeScore();
            if (gameOverUI != null) gameOverUI.SetActive(true);
            StartCoroutine(RestartAfterDelay());
        }

        public void CompleteLevel()
        {
            if (HasEnded) return;
            HasEnded = true;
            HasWon = true;
            if (player != null) player.StopAtFinish();
            FreezeScore();
            if (completeLevelUI != null) completeLevelUI.SetActive(true);
        }

        private void FreezeScore()
        {
            if (score == null) return;
            score.Refresh();
            score.enabled = false;
        }

        private IEnumerator RestartAfterDelay()
        {
            yield return new WaitForSeconds(restartDelay);
            Restart();
        }

        public void Restart()
        {
            StopAllCoroutines();
            SceneLoader.Load(SceneManager.GetActiveScene().name);
        }

        public void ReturnToMenu()
        {
            StopAllCoroutines();
            SceneLoader.Load(menuSceneName);
        }
    }
}
