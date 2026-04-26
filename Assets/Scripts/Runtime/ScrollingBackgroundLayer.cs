using UnityEngine;

namespace Wanwan.Runtime
{
    public class ScrollingBackgroundLayer : MonoBehaviour
    {
        private float speed;
        private float speedMultiplier = 1f;
        private float wrapHeight;
        private Vector3 startPosition;

        public void Initialize(float scrollSpeed, float layerHeight)
        {
            speed = scrollSpeed;
            wrapHeight = layerHeight;
            startPosition = transform.position;
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            speedMultiplier = Mathf.Max(0f, multiplier);
        }

        private void Update()
        {
            transform.position += Vector3.down * (speed * speedMultiplier * Time.deltaTime);
            if (transform.position.y <= startPosition.y - wrapHeight)
            {
                transform.position += Vector3.up * wrapHeight;
            }
        }
    }
}
