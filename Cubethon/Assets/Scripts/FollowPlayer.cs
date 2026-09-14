using UnityEngine;

namespace Cubethon
{
    // Episode 4: the camera follows at a fixed offset.
    public sealed class FollowPlayer : MonoBehaviour
    {
        public Transform player;
        public Vector3 offset = new Vector3(0f, 3f, -7f);

        private void LateUpdate()
        {
            // LateUpdate follows the Rigidbody's interpolated visual position.
            if (player != null) transform.position = player.position + offset;
        }
    }
}
