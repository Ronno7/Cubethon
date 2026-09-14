using UnityEngine;

namespace Cubethon
{
    // Episode 10: a title screen with working buttons.
    public sealed class Menu : MonoBehaviour
    {
        public void StartGame() { SceneLoader.Load("Level01"); }
        public void ShowCredits() { SceneLoader.Load("Credits"); }
        public void QuitGame() { QuitApplication.Quit(); }
    }
}
