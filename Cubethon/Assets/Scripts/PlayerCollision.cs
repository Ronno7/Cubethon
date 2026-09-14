using UnityEngine;

namespace Cubethon
{
    // Episodes 5 and 8: only obstacles end the run; the road does not.
    [RequireComponent(typeof(PlayerMovement))]
    public sealed class PlayerCollision : MonoBehaviour
    {
        public PlayerMovement movement;

        private void Awake()
        {
            if (movement == null) movement = GetComponent<PlayerMovement>();
        }

        private void OnCollisionEnter(Collision collisionInfo)
        {
            if (collisionInfo.collider.CompareTag("Obstacle") && movement.gameManager != null)
                movement.gameManager.EndGame();
        }
    }
}
