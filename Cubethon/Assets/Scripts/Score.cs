using UnityEngine;
using UnityEngine.UI;

namespace Cubethon
{
    // Episode 7: distance along the Z axis is the score.
    public sealed class Score : MonoBehaviour
    {
        public Transform player;
        public Text scoreText;

        private void Update()
        {
            Refresh();
        }

        public void Refresh()
        {
            if (player != null && scoreText != null)
                scoreText.text = Mathf.Max(0f, player.position.z).ToString("0");
        }
    }
}
