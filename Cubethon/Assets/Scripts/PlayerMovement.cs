using UnityEngine;

namespace Cubethon
{
    // Episodes 2, 3 and 8: forward force, steering, and falling off the road.
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PlayerMovement : MonoBehaviour
    {
        public Rigidbody rb;
        public GameManager gameManager;
        public float forwardForce = 2000f;
        public float sidewaysForce = 60f;
        public float fallHeight = -2f;

        private float steering;

        private void Awake()
        {
            if (rb == null) rb = GetComponent<Rigidbody>();
            if (gameManager == null) gameManager = FindFirstObjectByType<GameManager>();
        }

        private void Update()
        {
            // Read keys once per rendered frame; apply movement in the physics loop.
            steering = GameInput.Horizontal;
        }

        private void FixedUpdate()
        {
            if (gameManager != null && gameManager.HasEnded) return;

            // Keep the force convention used in the tutorial (50 Hz physics).
            rb.AddForce(0f, 0f, forwardForce * Time.fixedDeltaTime);
            rb.AddForce(steering * sidewaysForce * Time.fixedDeltaTime, 0f, 0f,
                ForceMode.VelocityChange);

            if (rb.position.y < fallHeight && gameManager != null)
                gameManager.EndGame();
        }

        public void StopAtFinish()
        {
            enabled = false;
            // Unity 6 calls this linearVelocity, replacing Rigidbody.velocity.
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }
}
