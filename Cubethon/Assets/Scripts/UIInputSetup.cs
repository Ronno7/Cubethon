using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace Cubethon
{
    // Both project templates can use the same scenes and menu buttons.
    [RequireComponent(typeof(EventSystem))]
    public sealed class UIInputSetup : MonoBehaviour
    {
        private void Awake()
        {
#if ENABLE_INPUT_SYSTEM
            var module = GetComponent<InputSystemUIInputModule>();
            if (module == null) module = gameObject.AddComponent<InputSystemUIInputModule>();
            module.AssignDefaultActions();
#elif ENABLE_LEGACY_INPUT_MANAGER
            if (GetComponent<StandaloneInputModule>() == null)
                gameObject.AddComponent<StandaloneInputModule>();
#else
            Debug.LogError("Cubethon needs an enabled input backend. Check Active Input Handling.");
#endif
        }
    }
}
