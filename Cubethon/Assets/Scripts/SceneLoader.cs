using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cubethon
{
    // Explicit names avoid relying on whichever scene order a fresh project had.
    public static class SceneLoader
    {
        public static void Load(string sceneName)
        {
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError("Cubethon cannot load '" + sceneName +
                    "'. Run Tools > Cubethon > Finish Import, then check the " +
                    "Scene List in File > Build Profiles.");
                return;
            }
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }
    }
}
