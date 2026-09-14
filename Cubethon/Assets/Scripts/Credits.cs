using UnityEngine;

namespace Cubethon
{
    // Episode 10: the final scene, reached after the last level.
    public sealed class Credits : MonoBehaviour
    {
        public void PlayAgain() { SceneLoader.Load("Level01"); }
        public void ReturnToMenu() { SceneLoader.Load("Menu"); }
        public void QuitGame() { QuitApplication.Quit(); }
    }
}
