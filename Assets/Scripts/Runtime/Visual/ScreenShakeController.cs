using System.Collections;
using UnityEngine;

namespace Wanwan.Runtime
{
    public class ScreenShakeController : MonoBehaviour
    {
        private Camera targetCamera;
        private Coroutine activeShake;

        public Camera TargetCamera => targetCamera;

        public void Initialize(Camera camera)
        {
            targetCamera = camera;
        }

        public void Play(float duration, float magnitude)
        {
            if (targetCamera == null || duration <= 0f || magnitude <= 0f)
            {
                return;
            }

            if (activeShake != null)
            {
                StopCoroutine(activeShake);
            }

            activeShake = StartCoroutine(Shake(duration, magnitude));
        }

        private IEnumerator Shake(float duration, float magnitude)
        {
            Vector3 originalPosition = targetCamera.transform.position;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                Vector2 offset = Random.insideUnitCircle * magnitude;
                targetCamera.transform.position = originalPosition + new Vector3(offset.x, offset.y, 0f);
                yield return null;
            }

            targetCamera.transform.position = originalPosition;
            activeShake = null;
        }
    }
}
