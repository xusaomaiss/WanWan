using System.Collections;
using UnityEngine;
using Wanwan.Runtime.Pools;

namespace Wanwan.Runtime
{
    public class ExplosionFlashController : MonoBehaviour
    {
        private PoolCollection pools;

        public void Initialize(PoolCollection poolCollection)
        {
            pools = poolCollection;
        }

        public void PlayWorldFlash(Vector3 position, float scale, Color color)
        {
            StartCoroutine(AnimateWorldFlash(position, Mathf.Max(0.1f, scale), color));
        }

        private IEnumerator AnimateWorldFlash(Vector3 position, float scale, Color color)
        {
            GameObject flash = pools != null ? pools.RentParticle() : new GameObject("ExplosionFlash");
            SpriteRenderer renderer = flash.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                renderer = flash.AddComponent<SpriteRenderer>();
            }

            renderer.sprite = RuntimeSpriteFactory.GetSciFiFlashSprite();
            renderer.color = color;
            renderer.sortingOrder = 41;
            flash.transform.position = new Vector3(position.x, position.y, -0.55f);
            flash.transform.localScale = Vector3.one * scale;

            const float duration = 0.18f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                flash.transform.localScale = Vector3.one * Mathf.Lerp(scale, scale * 2.8f, t);
                Color fade = renderer.color;
                fade.a = Mathf.Lerp(color.a, 0f, t);
                renderer.color = fade;
                yield return null;
            }

            Destroy(flash);
        }
    }
}
