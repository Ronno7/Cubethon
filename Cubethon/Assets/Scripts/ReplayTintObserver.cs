using UnityEngine;

namespace Cubethon
{
    [RequireComponent(typeof(GameManager))]
    public sealed class ReplayTintObserver : MonoBehaviour
    {
        public Color replayColor = new Color(0.1f, 0.7f, 1f, 1f);
        private GameManager manager;
        private Renderer playerRenderer;
        private MaterialPropertyBlock original;
        private MaterialPropertyBlock tinted;
        private bool applied;

        private void OnEnable()
        {
            manager = GetComponent<GameManager>();
            original = new MaterialPropertyBlock();
            tinted = new MaterialPropertyBlock();
            manager.ReplayStateChanged += SetReplayTint;
            if (manager.player != null && manager.player.IsReplaying) SetReplayTint(true);
        }

        private void OnDisable()
        {
            if (manager != null) manager.ReplayStateChanged -= SetReplayTint;
            RestoreColor();
        }

        private void SetReplayTint(bool replaying)
        {
            if (!replaying) { RestoreColor(); return; }
            if (manager.player == null) return;
            playerRenderer = manager.player.GetComponent<Renderer>();
            if (playerRenderer == null) return;
            if (!applied) playerRenderer.GetPropertyBlock(original);
            playerRenderer.GetPropertyBlock(tinted);
            tinted.SetColor("_Color", replayColor);     // Built-In Standard
            tinted.SetColor("_BaseColor", replayColor); // URP Lit
            playerRenderer.SetPropertyBlock(tinted);
            applied = true;
        }

        private void RestoreColor()
        {
            if (applied && playerRenderer != null) playerRenderer.SetPropertyBlock(original);
            applied = false;
        }
    }
}
