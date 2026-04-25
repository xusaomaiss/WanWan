using System.Collections;
using UnityEngine;

namespace Wanwan.Runtime
{
    public class EffectsController : MonoBehaviour
    {
        private Camera targetCamera;
        private AudioSource audioSource;
        private AudioClip hitClip;
        private AudioClip burstClip;
        private AudioClip baseClip;
        private AudioClip powerupClip;

        public void Initialize(Camera mainCamera)
        {
            targetCamera = mainCamera;
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = 0.18f;
            hitClip = BuildTone(740f, 0.045f);
            burstClip = BuildTone(510f, 0.09f);
            baseClip = BuildTone(220f, 0.14f);
            powerupClip = BuildTone(920f, 0.12f);
        }

        public void PlayHit(Vector3 position, Color color)
        {
            EmitParticles(position, color, 6, 0.16f, 0.35f);
            audioSource.PlayOneShot(hitClip);
        }

        public void PlayBurst(Vector3 position, Color color)
        {
            EmitExplosionImage(position);
            EmitParticles(position, color, 12, 0.24f, 0.55f);
            EmitDebris(position, color, 10);
            audioSource.PlayOneShot(burstClip);
            StartCoroutine(Shake(0.07f, 0.08f));
        }

        public void PlayScorePopup(Vector3 position, int scoreValue)
        {
            StartCoroutine(AnimateScoreLabel(position, scoreValue));
        }

        public void PlayBombDetonation(Vector3 center)
        {
            audioSource.PlayOneShot(baseClip);
            StartCoroutine(Shake(0.18f, 0.2f));
            StartCoroutine(AnimateBombFlash(center));
            StartCoroutine(AnimateBombShockwave(center));
        }

        public void PlayBaseHit()
        {
            audioSource.PlayOneShot(baseClip);
            StartCoroutine(Shake(0.12f, 0.15f));
        }

        public void PlayPowerupPickup(Vector3 position, Color color, string label)
        {
            EmitParticles(position, color, 15, 0.28f, 0.62f);
            audioSource.PlayOneShot(powerupClip);
            StartCoroutine(Shake(0.08f, 0.12f));
            StartCoroutine(AnimatePowerupLabel(position, color, label));
        }

        public void PlayPowerupSpawn(Vector3 position, Color color)
        {
            EmitParticles(position, color, 8, 0.2f, 0.36f);
        }

        public void PlayPlayerPierced(Vector3 position, Color color)
        {
            EmitParticles(position, color, 20, 0.34f, 0.72f);
            audioSource.PlayOneShot(baseClip);
            StartCoroutine(Shake(0.18f, 0.22f));
        }

        public void PlayBossArrival(Vector3 position, Color color)
        {
            EmitParticles(position, color, 28, 0.42f, 1.15f);
            audioSource.PlayOneShot(powerupClip);
            StartCoroutine(Shake(0.22f, 0.18f));
        }

        private void EmitParticles(Vector3 position, Color color, int count, float duration, float radius)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject particle = new GameObject("CandyParticle");
                SpriteRenderer renderer = particle.AddComponent<SpriteRenderer>();
                renderer.sprite = RuntimeSpriteFactory.GetCircleSprite();
                renderer.color = color;
                renderer.sortingOrder = 20;
                particle.transform.position = position;
                float scale = Random.Range(0.08f, 0.18f);
                particle.transform.localScale = Vector3.one * scale;
                StartCoroutine(AnimateParticle(particle, renderer, duration, Random.insideUnitCircle * radius));
            }
        }

        private void EmitExplosionImage(Vector3 position)
        {
            GameObject explosion = new GameObject("ExplosionImage");
            SpriteRenderer renderer = explosion.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.GetExplosionSprite();
            renderer.color = Color.white;
            renderer.sortingOrder = 24;
            explosion.transform.position = position;
            explosion.transform.localScale = Vector3.one * Random.Range(0.62f, 0.86f);
            StartCoroutine(AnimateExplosionImage(explosion, renderer));
        }

        private void EmitDebris(Vector3 position, Color color, int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject debris = new GameObject("ExplosionDebris");
                SpriteRenderer renderer = debris.AddComponent<SpriteRenderer>();
                renderer.sprite = RuntimeSpriteFactory.GetRoundedSquareSprite();
                renderer.color = Color.Lerp(color, new Color(1f, 0.82f, 0.32f), 0.55f);
                renderer.sortingOrder = 23;
                debris.transform.position = position;
                debris.transform.localScale = new Vector3(Random.Range(0.035f, 0.075f), Random.Range(0.08f, 0.18f), 1f);
                debris.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
                Vector2 velocity = Random.insideUnitCircle.normalized * Random.Range(0.65f, 1.55f);
                velocity.y -= Random.Range(0f, 0.45f);
                StartCoroutine(AnimateDebris(debris, renderer, velocity));
            }
        }

        private IEnumerator AnimateExplosionImage(GameObject explosion, SpriteRenderer renderer)
        {
            float duration = 0.36f;
            float elapsed = 0f;
            Vector3 startScale = explosion.transform.localScale;
            Vector3 endScale = startScale * 1.85f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                explosion.transform.localScale = Vector3.Lerp(startScale, endScale, t);
                Color color = renderer.color;
                color.a = 1f - t;
                renderer.color = color;
                yield return null;
            }

            Destroy(explosion);
        }

        private IEnumerator AnimateParticle(GameObject particle, SpriteRenderer renderer, float duration, Vector2 offset)
        {
            Vector3 start = particle.transform.position;
            Vector3 end = start + new Vector3(offset.x, offset.y, 0f);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                particle.transform.position = Vector3.Lerp(start, end, t);
                particle.transform.localScale *= 0.99f;
                Color color = renderer.color;
                color.a = 1f - t;
                renderer.color = color;
                yield return null;
            }

            Destroy(particle);
        }

        private IEnumerator AnimateDebris(GameObject debris, SpriteRenderer renderer, Vector2 velocity)
        {
            float duration = 0.48f;
            float elapsed = 0f;
            Vector3 start = debris.transform.position;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                Vector2 gravity = Vector2.down * (1.2f * t * t);
                debris.transform.position = start + (Vector3)((velocity * elapsed) + gravity);
                debris.transform.Rotate(0f, 0f, 360f * Time.deltaTime);
                Color color = renderer.color;
                color.a = 1f - t;
                renderer.color = color;
                yield return null;
            }

            Destroy(debris);
        }

        private IEnumerator Shake(float duration, float magnitude)
        {
            if (targetCamera == null)
            {
                yield break;
            }

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
        }

        private IEnumerator AnimatePowerupLabel(Vector3 position, Color color, string label)
        {
            GameObject labelObject = new GameObject("PowerupLabel");
            TextMesh textMesh = labelObject.AddComponent<TextMesh>();
            textMesh.text = label + "!";
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.characterSize = 0.17f;
            textMesh.fontSize = 58;
            textMesh.color = color;
            labelObject.transform.position = position + Vector3.up * 0.1f;

            Vector3 start = labelObject.transform.position;
            Vector3 end = start + Vector3.up * 0.9f;
            float duration = 0.45f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                labelObject.transform.position = Vector3.Lerp(start, end, t);
                Color textColor = textMesh.color;
                textColor.a = 1f - t;
                textMesh.color = textColor;
                yield return null;
            }

            Destroy(labelObject);
        }

        private IEnumerator AnimateScoreLabel(Vector3 position, int scoreValue)
        {
            GameObject labelObject = new GameObject("ScorePopup");
            TextMesh textMesh = labelObject.AddComponent<TextMesh>();
            textMesh.text = "+" + scoreValue;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.characterSize = 0.15f;
            textMesh.fontSize = 54;
            textMesh.color = new Color(1f, 0.92f, 0.52f);
            labelObject.transform.position = position + Vector3.up * 0.22f;

            Vector3 start = labelObject.transform.position;
            Vector3 end = start + Vector3.up * 0.82f;
            float duration = 0.65f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                labelObject.transform.position = Vector3.Lerp(start, end, t);
                Color textColor = textMesh.color;
                textColor.a = 1f - t;
                textMesh.color = textColor;
                yield return null;
            }

            Destroy(labelObject);
        }

        private IEnumerator AnimateBombFlash(Vector3 center)
        {
            GameObject flash = new GameObject("BombFlash");
            SpriteRenderer renderer = flash.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.GetRoundedSquareSprite();
            renderer.color = new Color(0.72f, 0.95f, 1f, 0.58f);
            renderer.sortingOrder = 40;
            flash.transform.position = new Vector3(center.x, center.y, -0.5f);
            flash.transform.localScale = new Vector3(28f, 36f, 1f);

            float duration = 0.22f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                Color color = renderer.color;
                color.a = Mathf.Lerp(0.58f, 0f, t);
                renderer.color = color;
                yield return null;
            }

            Destroy(flash);
        }

        private IEnumerator AnimateBombShockwave(Vector3 center)
        {
            GameObject shockwave = new GameObject("BombShockwave");
            SpriteRenderer renderer = shockwave.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.GetCircleSprite();
            renderer.color = new Color(0.34f, 0.86f, 1f, 0.52f);
            renderer.sortingOrder = 39;
            shockwave.transform.position = new Vector3(center.x, center.y, -0.45f);
            shockwave.transform.localScale = Vector3.one * 0.4f;

            float duration = 0.42f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                shockwave.transform.localScale = Vector3.one * Mathf.Lerp(0.4f, 9.5f, t);
                Color color = renderer.color;
                color.a = Mathf.Lerp(0.52f, 0f, t);
                renderer.color = color;
                yield return null;
            }

            Destroy(shockwave);
        }

        private static AudioClip BuildTone(float frequency, float duration)
        {
            const int sampleRate = 44100;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float time = i / (float)sampleRate;
                float envelope = Mathf.Clamp01(1f - (time / duration));
                data[i] = Mathf.Sin(2f * Mathf.PI * frequency * time) * envelope * 0.35f;
            }

            AudioClip clip = AudioClip.Create("Tone" + frequency, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
