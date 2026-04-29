using UnityEngine;

namespace Wanwan.Runtime
{
    public class MenuScaleInAnimator : MonoBehaviour
    {
        private RectTransform rectTransform;
        private Vector3 targetScale = Vector3.one;
        private float duration = 0.32f;
        private float elapsed;

        public void Configure(float startScale = 0.92f, float animationDuration = 0.32f)
        {
            rectTransform = GetComponent<RectTransform>();
            targetScale = Vector3.one;
            duration = Mathf.Max(0.01f, animationDuration);
            elapsed = 0f;
            transform.localScale = Vector3.one * startScale;
        }

        private void Update()
        {
            elapsed = Mathf.Min(duration, elapsed + Time.unscaledDeltaTime);
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, eased);
            if (elapsed >= duration)
            {
                transform.localScale = targetScale;
                enabled = false;
            }
        }
    }
}
