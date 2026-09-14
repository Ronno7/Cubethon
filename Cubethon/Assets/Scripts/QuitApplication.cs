using UnityEngine;

namespace Cubethon
{
    public static class QuitApplication
    {
        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
            // Browsers cannot close their own tab; return to the title instead.
            SceneLoader.Load("Menu");
#else
            Application.Quit();
#endif
        }
    }
}
