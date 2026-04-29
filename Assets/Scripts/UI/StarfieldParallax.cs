using UnityEngine;

namespace Wanwan.Runtime
{
    public class StarfieldParallax : MonoBehaviour
    {
        private RectTransform rectTransform;
        private Vector2 startPosition;
        private Vector2 drift;
        private float verticalSpeed;
        private float wrapHeight;

        public void Configure(float speed, float height, Vector2 driftPerSecond)
        {
            rectTransform = GetComponent<RectTransform>();
            startPosition = rectTransform != null ? rectTransform.anchoredPosition : Vector2.zero;
            verticalSpeed = speed;
            wrapHeight = Mathf.Max(1f, height);
            drift = driftPerSecond;
        }

        private void Update()
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
                startPosition = rectTransform != null ? rectTransform.anchoredPosition : Vector2.zero;
            }

            float yOffset = Mathf.Repeat(Time.unscaledTime * verticalSpeed, wrapHeight);
            Vector2 driftOffset = drift * Time.unscaledTime;
            rectTransform.anchoredPosition = startPosition + driftOffset + new Vector2(0f, -yOffset);
        }
    }
}
