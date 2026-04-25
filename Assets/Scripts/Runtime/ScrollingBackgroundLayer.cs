using UnityEngine;

namespace Wanwan.Runtime
{
    public class ScrollingBackgroundLayer : MonoBehaviour
    {
        private float speed;
        private float wrapHeight;
        private Vector3 startPosition;

        public void Initialize(float scrollSpeed, float layerHeight)
        {
            speed = scrollSpeed;
            wrapHeight = layerHeight;
            startPosition = transform.position;
        }

        private void Update()
        {
            transform.position += Vector3.down * (speed * Time.deltaTime);
            if (transform.position.y <= startPosition.y - wrapHeight)
            {
                transform.position += Vector3.up * wrapHeight;
            }
        }
    }
}
