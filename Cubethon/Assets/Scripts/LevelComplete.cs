using UnityEngine;

namespace Cubethon
{
    // Episode 9: the last frame of LevelComplete.anim calls LoadNextLevel.
    public sealed class LevelComplete : MonoBehaviour
    {
        public GameManager gameManager;
        private bool loading;

        public void LoadNextLevel()
        {
            if (loading || gameManager == null || !gameManager.HasWon) return;
            loading = true;
            SceneLoader.Load(gameManager.nextSceneName);
        }
    }
}
