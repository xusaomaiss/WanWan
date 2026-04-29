using System.Collections;
using UnityEngine;
using Wanwan.Runtime.Pools;

namespace Wanwan.Runtime
{
    public class EffectsController : MonoBehaviour
    {
        private Camera targetCamera;
        private AudioSource sfxSource;
        private AudioSource musicSource;
        private AudioClip playerShotClip;
        private AudioClip laserShotClip;
        private AudioClip hitClip;
        private AudioClip explosionSmallClip;
        private AudioClip explosionLargeClip;
        private AudioClip baseClip;
        private AudioClip powerupClip;
        private AudioClip bombClip;
        private AudioClip bossAlarmClip;
        private VisualEffectsQuality EffectsQuality => SessionState.VisualEffectsQuality;
        private PoolCollection pools;
        private ScreenShakeController screenShake;
        private ExplosionFlashController explosionFlash;

        public void Initialize(Camera mainCamera)
        {
            targetCamera = mainCamera;
            pools = FindObjectOfType<GameBootstrap>()?.Pools;
            screenShake = gameObject.AddComponent<ScreenShakeController>();
            screenShake.Initialize(mainCamera);
            explosionFlash = gameObject.AddComponent<ExplosionFlashController>();
            explosionFlash.Initialize(pools);
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;

            playerShotClip = BuildPulseTone("shoot_player", 760f, 0.055f, 0.28f);
            laserShotClip = BuildSweep("shoot_laser", 1180f, 620f, 0.09f, 0.24f);
            hitClip = BuildPulseTone("hit_tick", 860f, 0.045f, 0.22f);
            explosionSmallClip = BuildNoiseBurst("explosion_small", 0.22f, 0.32f, 135f);
            explosionLargeClip = BuildNoiseBurst("explosion_large", 0.55f, 0.4f, 82f);
            baseClip = BuildSweep("player_hit", 260f, 92f, 0.18f, 0.36f);
            powerupClip = BuildArpeggio("powerup", new[] { 880f, 1174f, 1568f }, 0.18f, 0.22f);
            bombClip = BuildBombClip();
            bossAlarmClip = BuildBossAlarmClip();

            musicSource.clip = BuildArcadeLoop();
            RefreshAudioSettings();
            musicSource.Play();
        }

        private void Update()
        {
            RefreshAudioSettings();
        }

        public void RefreshAudioSettings()
        {
            float enabledScale = SessionState.AudioEnabled ? 1f : 0f;
            if (sfxSource != null)
            {
                sfxSource.volume = 0.24f * SessionState.SfxVolume * enabledScale;
            }

            if (musicSource != null)
            {
                musicSource.volume = 0.075f * SessionState.MusicVolume * enabledScale;
            }
        }

        public void PlayPlayerShot(AmmoPowerupType type)
        {
            AudioClip clip = type == AmmoPowerupType.Laser || type == AmmoPowerupType.Pierce ? laserShotClip : playerShotClip;
            sfxSource.PlayOneShot(clip, type == AmmoPowerupType.RapidFire ? 0.52f : 0.72f);
        }

        public void PlayPlayerShot(WeaponType type)
        {
            PlayPlayerShot(PowerupCycle.ToAmmoPowerupType(type));
        }

        public void PlayHit(Vector3 position, Color color)
        {
            EmitParticles(position, color, 6, 0.16f, 0.35f);
            sfxSource.PlayOneShot(hitClip, 0.7f);
        }

        public void PlaySmallBurst(Vector3 position, Color color)
        {
            EmitExplosionImage(position);
            EmitParticles(position, color, 6, 0.12f, 0.2f);
            sfxSource.PlayOneShot(explosionSmallClip, 0.6f);
            StartShake(0.04f, 0.04f);
        }

        public void PlayBurst(Vector3 position, Color color)
        {
            EmitExplosionImage(position);
            EmitParticles(position, color, 16, 0.28f, 0.68f);
            EmitDebris(position, color, VisualEffectsBudget.GetParticleCount(EffectsQuality, 14));
            sfxSource.PlayOneShot(explosionSmallClip, 0.86f);
            StartShake(0.07f, 0.08f);
        }

        public void PlayEliteBurst(Vector3 position, Color color)
        {
            EmitExplosionImage(position);
            EmitExplosionImage(position + new Vector3(0.2f, -0.1f, 0f));
            EmitParticles(position, color, 24, 0.4f, 0.9f);
            EmitDebris(position, color, VisualEffectsBudget.GetParticleCount(EffectsQuality, 20));
            sfxSource.PlayOneShot(explosionSmallClip, 1f);
            StartShake(0.12f, 0.12f);
        }

        public void PlayGroundTargetDestroyed(Vector3 position, Color color)
        {
            EmitExplosionImage(position);
            EmitParticles(position, Color.Lerp(color, new Color(1f, 0.75f, 0.22f), 0.45f), 22, 0.36f, 0.82f);
            EmitDebris(position, color, VisualEffectsBudget.GetParticleCount(EffectsQuality, 22));
            sfxSource.PlayOneShot(explosionSmallClip, 1f);
            StartShake(0.1f, 0.1f);
        }

        public void PlayBossDefeat(Vector3 position, Color color)
        {
            sfxSource.PlayOneShot(explosionLargeClip, 1f);
            EmitExplosionImage(position);
            EmitExplosionImage(position + new Vector3(-0.34f, 0.22f, 0f));
            EmitExplosionImage(position + new Vector3(0.38f, -0.18f, 0f));
            EmitParticles(position, color, 44, 0.52f, 1.32f);
            EmitDebris(position, color, VisualEffectsBudget.GetParticleCount(EffectsQuality, 28));
            StartShake(0.28f, 0.24f);
            StartCoroutine(PlayBossDefeatCascade(position, color));
        }

        public void PlayScorePopup(Vector3 position, int scoreValue)
        {
            StartCoroutine(AnimateScoreLabel(position, scoreValue));
        }

        public void PlayBombDetonation(Vector3 center)
        {
            sfxSource.PlayOneShot(bombClip, 1f);
            StartShake(0.18f, 0.2f);
            StartCoroutine(AnimateBombFlash(center));
            StartCoroutine(AnimateBombShockwave(center));
            if (EffectsQuality == VisualEffectsQuality.Full)
            {
                EmitExplosionImage(center + new Vector3(-0.85f, 0.15f, 0f));
                EmitExplosionImage(center + new Vector3(0.82f, -0.12f, 0f));
            }
        }

        public void PlayBaseHit()
        {
            sfxSource.PlayOneShot(baseClip, 0.86f);
            StartShake(0.12f, 0.15f);
        }

        public void PlayPowerupPickup(Vector3 position, Color color, string label)
        {
            EmitParticles(position, color, 15, 0.28f, 0.62f);
            sfxSource.PlayOneShot(powerupClip, 0.92f);
            StartShake(0.08f, 0.12f);
            StartCoroutine(AnimatePowerupLabel(position, color, label));
        }

        public void PlayPowerupSpawn(Vector3 position, Color color)
        {
            EmitParticles(position, color, 8, 0.2f, 0.36f);
        }

        public void PlayPlayerPierced(Vector3 position, Color color)
        {
            EmitParticles(position, color, 20, 0.34f, 0.72f);
            sfxSource.PlayOneShot(baseClip, 1f);
            StartShake(0.18f, 0.22f);
        }

        public void PlayBossArrival(Vector3 position, Color color)
        {
            EmitParticles(position, color, 28, 0.42f, 1.15f);
            sfxSource.PlayOneShot(bossAlarmClip, 1f);
            StartShake(0.22f, 0.18f);
        }

        private void StartShake(float duration, float magnitude)
        {
            float budgetDuration = VisualEffectsBudget.GetShakeDuration(EffectsQuality, duration);
            float budgetMagnitude = VisualEffectsBudget.GetShakeMagnitude(EffectsQuality, magnitude);
            if (screenShake != null)
            {
                screenShake.Play(budgetDuration, budgetMagnitude);
                return;
            }

            StartCoroutine(Shake(budgetDuration, budgetMagnitude));
        }

        private void EmitParticles(Vector3 position, Color color, int count, float duration, float radius)
        {
            int budgetedCount = VisualEffectsBudget.GetParticleCount(EffectsQuality, count);
            for (int i = 0; i < budgetedCount; i++)
            {
                GameObject particle = pools != null ? pools.RentParticle() : new GameObject("CandyParticle");
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
            GameObject explosion = pools != null ? pools.RentParticle() : new GameObject("ExplosionImage");
            SpriteRenderer renderer = explosion.AddComponent<SpriteRenderer>();
            Sprite[] frames = RuntimeSpriteFactory.GetArcadeExplosionFrameSprites();
            renderer.sprite = RuntimeSpriteFactory.GetSciFiExplosionSprite();
            renderer.color = Color.white;
            renderer.sortingOrder = 24;
            explosion.transform.position = position;
            explosion.transform.localScale = Vector3.one * Random.Range(0.78f, 1.08f);
            if (explosionFlash != null)
            {
                explosionFlash.PlayWorldFlash(position, 0.75f, new Color(0.62f, 0.95f, 1f, 0.46f));
            }
            StartCoroutine(AnimateExplosionImage(explosion, renderer, frames));
        }

        private void EmitDebris(Vector3 position, Color color, int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject debris = pools != null ? pools.RentParticle() : new GameObject("ExplosionDebris");
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

        private IEnumerator AnimateExplosionImage(GameObject explosion, SpriteRenderer renderer, Sprite[] frames)
        {
            float duration = 0.42f;
            float elapsed = 0f;
            Vector3 startScale = explosion.transform.localScale;
            Vector3 endScale = startScale * 2.12f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                int frameIndex = Mathf.Clamp(Mathf.FloorToInt(t * frames.Length), 0, frames.Length - 1);
                renderer.sprite = frames[frameIndex];
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

        private IEnumerator PlayBossDefeatCascade(Vector3 position, Color color)
        {
            int cascadeCount = VisualEffectsBudget.GetExplosionCascadeCount(EffectsQuality);
            for (int i = 0; i < cascadeCount; i++)
            {
                yield return new WaitForSeconds(0.11f);
                float normalized = cascadeCount <= 1 ? 1f : i / (float)(cascadeCount - 1);
                Vector3 burstPosition = position + (Vector3)(Random.insideUnitCircle * Mathf.Lerp(0.55f, 1.35f, normalized));
                EmitExplosionImage(burstPosition);
                EmitParticles(burstPosition, Color.Lerp(color, new Color(1f, 0.78f, 0.28f), 0.42f), 18, 0.32f, 0.78f);
                EmitDebris(burstPosition, color, VisualEffectsBudget.GetParticleCount(EffectsQuality, 12));
                sfxSource.PlayOneShot(explosionSmallClip, 0.7f);
            }
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
            GameObject labelObject = pools != null ? pools.RentLabel() : new GameObject("PowerupLabel");
            TextMesh textMesh = labelObject.AddComponent<TextMesh>();
            ArcadeFontProvider.ApplyTo(textMesh);
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
            GameObject labelObject = pools != null ? pools.RentLabel() : new GameObject("ScorePopup");
            TextMesh textMesh = labelObject.AddComponent<TextMesh>();
            ArcadeFontProvider.ApplyTo(textMesh);
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
            GameObject flash = pools != null ? pools.RentParticle() : new GameObject("BombFlash");
            SpriteRenderer renderer = flash.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.GetSciFiFlashSprite();
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
            GameObject shockwave = pools != null ? pools.RentParticle() : new GameObject("BombShockwave");
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

        private static AudioClip BuildPulseTone(string name, float frequency, float duration, float gain)
        {
            const int sampleRate = 44100;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float time = i / (float)sampleRate;
                float envelope = Mathf.Clamp01(1f - (time / duration));
                float square = Mathf.Sign(Mathf.Sin(2f * Mathf.PI * frequency * time));
                float sine = Mathf.Sin(2f * Mathf.PI * frequency * 1.5f * time);
                data[i] = ((square * 0.72f) + (sine * 0.28f)) * envelope * gain;
            }

            AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private static AudioClip BuildSweep(string name, float startFrequency, float endFrequency, float duration, float gain)
        {
            const int sampleRate = 44100;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            float[] data = new float[samples];
            float phase = 0f;

            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)samples;
                float frequency = Mathf.Lerp(startFrequency, endFrequency, t);
                phase += 2f * Mathf.PI * frequency / sampleRate;
                float envelope = Mathf.Pow(1f - t, 1.8f);
                data[i] = Mathf.Sin(phase) * envelope * gain;
            }

            AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private static AudioClip BuildNoiseBurst(string name, float duration, float gain, float rumbleFrequency)
        {
            const int sampleRate = 44100;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            float[] data = new float[samples];
            uint seed = 2463534242u;

            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)samples;
                seed ^= seed << 13;
                seed ^= seed >> 17;
                seed ^= seed << 5;
                float noise = ((seed & 0xffff) / 32768f) - 1f;
                float rumble = Mathf.Sin(2f * Mathf.PI * rumbleFrequency * (i / (float)sampleRate));
                float envelope = Mathf.Pow(1f - t, 2.2f);
                data[i] = ((noise * 0.68f) + (rumble * 0.32f)) * envelope * gain;
            }

            AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private static AudioClip BuildArpeggio(string name, float[] frequencies, float duration, float gain)
        {
            const int sampleRate = 44100;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)samples;
                int note = Mathf.Min(frequencies.Length - 1, Mathf.FloorToInt(t * frequencies.Length));
                float local = (t * frequencies.Length) - note;
                float envelope = Mathf.Sin(local * Mathf.PI);
                float tone = Mathf.Sin(2f * Mathf.PI * frequencies[note] * (i / (float)sampleRate));
                data[i] = tone * envelope * gain;
            }

            AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private static AudioClip BuildBombClip()
        {
            const int sampleRate = 44100;
            float duration = 0.72f;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            float[] data = new float[samples];
            AudioClip charge = BuildSweep("bomb_charge_tmp", 180f, 980f, 0.28f, 0.36f);
            AudioClip blast = BuildNoiseBurst("bomb_blast_tmp", 0.44f, 0.46f, 70f);
            float[] chargeData = new float[Mathf.CeilToInt(sampleRate * 0.28f)];
            float[] blastData = new float[Mathf.CeilToInt(sampleRate * 0.44f)];
            charge.GetData(chargeData, 0);
            blast.GetData(blastData, 0);

            for (int i = 0; i < chargeData.Length && i < data.Length; i++)
            {
                data[i] += chargeData[i];
            }

            int offset = Mathf.CeilToInt(sampleRate * 0.22f);
            for (int i = 0; i < blastData.Length && i + offset < data.Length; i++)
            {
                data[i + offset] += blastData[i];
            }

            AudioClip clip = AudioClip.Create("bomb_release", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private static AudioClip BuildBossAlarmClip()
        {
            const int sampleRate = 44100;
            float duration = 1.25f;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float time = i / (float)sampleRate;
                float gate = Mathf.Repeat(time * 4.8f, 1f) < 0.55f ? 1f : 0f;
                float tone = Mathf.Sin(2f * Mathf.PI * 620f * time) + (Mathf.Sin(2f * Mathf.PI * 465f * time) * 0.55f);
                float envelope = Mathf.Clamp01(1f - (time / duration) * 0.25f);
                data[i] = tone * gate * envelope * 0.2f;
            }

            AudioClip clip = AudioClip.Create("boss_alarm", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private static AudioClip BuildArcadeLoop()
        {
            const int sampleRate = 22050;
            const float duration = 8f;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            float[] data = new float[samples];
            int[] melody = { 0, 3, 5, 7, 10, 7, 5, 3, 0, 5, 7, 12, 10, 7, 5, 3 };
            int[] bass = { 0, 0, 5, 5, 7, 7, 3, 3 };

            for (int i = 0; i < samples; i++)
            {
                float time = i / (float)sampleRate;
                float beat = time * 4f;
                int melodyIndex = Mathf.FloorToInt(beat * 2f) % melody.Length;
                int bassIndex = Mathf.FloorToInt(beat) % bass.Length;
                float melodyFreq = 440f * Mathf.Pow(2f, melody[melodyIndex] / 12f);
                float bassFreq = 110f * Mathf.Pow(2f, bass[bassIndex] / 12f);
                float melodyGate = Mathf.Repeat(beat * 2f, 1f) < 0.72f ? 1f : 0.18f;
                float bassGate = Mathf.Repeat(beat, 1f) < 0.58f ? 1f : 0.12f;
                float lead = Mathf.Sign(Mathf.Sin(2f * Mathf.PI * melodyFreq * time)) * 0.045f * melodyGate;
                float low = Mathf.Sign(Mathf.Sin(2f * Mathf.PI * bassFreq * time)) * 0.055f * bassGate;
                float pulse = Mathf.Sin(2f * Mathf.PI * 9.5f * time) * 0.01f;
                data[i] = lead + low + pulse;
            }

            AudioClip clip = AudioClip.Create("bgm_stage_loop", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
