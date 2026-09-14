using UnityEngine;

namespace Cubethon
{
    // Episode 9: reaching the end wins the level.
    [RequireComponent(typeof(BoxCollider))]
    public sealed class EndTrigger : MonoBehaviour
    {
        public GameManager gameManager;

        private void Awake()
        {
            if (gameManager == null) gameManager = FindFirstObjectByType<GameManager>();
        }

        private void OnTriggerEnter(Collider other)
        {
            // Ignore obstacles and other objects that might enter the trigger.
            if (other.attachedRigidbody != null &&
                other.attachedRigidbody.GetComponent<PlayerMovement>() != null &&
                gameManager != null)
                gameManager.CompleteLevel();
        }
    }
}
