using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Cubethon
{
    // Supports a fresh project's New, Old, or Both Active Input Handling setting.
    public static class GameInput
    {
        public static float Horizontal
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                Keyboard keyboard = Keyboard.current;
                if (keyboard == null) return 0f;
                bool left = keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed;
                bool right = keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed;
                return (right ? 1f : 0f) - (left ? 1f : 0f);
#elif ENABLE_LEGACY_INPUT_MANAGER
                bool left = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
                bool right = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
                return (right ? 1f : 0f) - (left ? 1f : 0f);
#else
                return 0f;
#endif
            }
        }

        public static bool RestartPressed
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame;
#elif ENABLE_LEGACY_INPUT_MANAGER
                return Input.GetKeyDown(KeyCode.R);
#else
                return false;
#endif
            }
        }

        public static bool MenuPressed
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#elif ENABLE_LEGACY_INPUT_MANAGER
                return Input.GetKeyDown(KeyCode.Escape);
#else
                return false;
#endif
            }
        }
    }
}
