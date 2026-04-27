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
            foreach (var g in graphics)
            {
                var color = g.color;
                g.color = new Color(color.r, color.g, color.b, 0f);
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float eased = 1f - Mathf.Pow(1f - t, 3f);
                foreach (var g in graphics)
                {
                    var color = g.color;
                    g.color = new Color(color.r, color.g, color.b, eased);
                }
                yield return null;
            }

            foreach (var g in graphics)
            {
                var color = g.color;
                g.color = new Color(color.r, color.g, color.b, 1f);
            }
        }
    }
}
