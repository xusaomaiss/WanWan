using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Wanwan.Runtime
{
    public class MenuTransitionController : MonoBehaviour
    {
        public IEnumerator FadeGroup(Transform parent, float duration = 0.28f)
        {
            var graphics = parent.GetComponentsInChildren<Graphic>();
            Color[] originalColors = new Color[graphics.Length];
            for (int i = 0; i < graphics.Length; i++)
            {
                originalColors[i] = graphics[i].color;
                graphics[i].color = WithScaledAlpha(originalColors[i], 0f);
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float eased = 1f - Mathf.Pow(1f - t, 3f);
                for (int i = 0; i < graphics.Length; i++)
                {
                    graphics[i].color = WithScaledAlpha(originalColors[i], eased);
                }
                yield return null;
            }

            for (int i = 0; i < graphics.Length; i++)
            {
                graphics[i].color = originalColors[i];
            }
        }

        private static Color WithScaledAlpha(Color color, float alphaScale)
        {
            return new Color(color.r, color.g, color.b, color.a * alphaScale);
        }
    }
}
