using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cubethon
{
    // Composition only: attaches observers without coupling GameManager to them.
    public static class ObserverSetup
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {
            SceneManager.sceneLoaded -= Install;
            SceneManager.sceneLoaded += Install;
        }

        private static void Install(Scene scene, LoadSceneMode mode)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            foreach (GameManager manager in root.GetComponentsInChildren<GameManager>(false))
            {
                if (manager.GetComponent<CrashCounterObserver>() == null)
                    manager.gameObject.AddComponent<CrashCounterObserver>();
                if (manager.GetComponent<LevelTimerObserver>() == null)
                    manager.gameObject.AddComponent<LevelTimerObserver>();
                if (manager.GetComponent<ReplayTintObserver>() == null)
                    manager.gameObject.AddComponent<ReplayTintObserver>();
            }
        }
    }
}
