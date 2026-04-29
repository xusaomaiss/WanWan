using UnityEngine;

namespace Wanwan.Runtime
{
    public class CoinMagnetEffect : MonoBehaviour
    {
        public const string AttractTrailObjectName = "CoinMagnetTrail";
        public const float AttractingScaleMultiplier = 1.18f;

        private SpriteRenderer trailRenderer;

        public void Initialize()
        {
            if (trailRenderer != null)
            {
                return;
            }

            GameObject trail = new GameObject(AttractTrailObjectName);
            trail.transform.SetParent(transform, false);
            trail.transform.localPosition = Vector3.zero;
            trailRenderer = trail.AddComponent<SpriteRenderer>();
            trailRenderer.sprite = RuntimeSpriteFactory.GetSciFiFlashSprite();
            trailRenderer.color = new Color(0.18f, 0.88f, 1f, 0f);
            trailRenderer.sortingOrder = 17;
            trail.transform.localScale = Vector3.one * 0.65f;
        }

        public void SetAttracting(bool attracting)
        {
            if (trailRenderer == null)
            {
                Initialize();
            }

            Color color = trailRenderer.color;
            color.a = attracting ? 0.34f : 0f;
            trailRenderer.color = color;
        }
    }
}
