using System.Collections;
using UnityEngine;

namespace Wanwan.Runtime
{
    public class CarrierLaunchIntroController : MonoBehaviour
    {
        private GameManager manager;
        private Transform playerTransform;
        private PlayerController playerController;
        private Vector3 gameplayPosition;
        private CarrierLaunchIntroConfig config;
        private ScrollingBackgroundLayer[] backgroundLayers;
        private SpriteRenderer introBackdrop;
        private SpriteRenderer whiteFlash;
        private SpriteRenderer[] speedLines;
        private Camera sceneCamera;
        private ParticleSystem exhaustParticles;
        private Coroutine playRoutine;
        private bool completed;
        private float skipInputEnabledAt;

        public void Initialize(
            GameManager gameManager,
            Transform player,
            Vector3 playerGameplayPosition,
            CarrierLaunchIntroConfig introConfig,
            ScrollingBackgroundLayer[] scrollingLayers)
        {
            Initialize(gameManager, player, playerGameplayPosition, introConfig, scrollingLayers, null, null);
        }

        public void Initialize(
            GameManager gameManager,
            Transform player,
            Vector3 playerGameplayPosition,
            CarrierLaunchIntroConfig introConfig,
            ScrollingBackgroundLayer[] scrollingLayers,
            SpriteRenderer cinematicBackdrop,
            Camera cameraComponent)
        {
            manager = gameManager;
            playerTransform = player;
            playerController = player == null ? null : player.GetComponent<PlayerController>();
            gameplayPosition = playerGameplayPosition;
            config = introConfig;
            backgroundLayers = scrollingLayers ?? new ScrollingBackgroundLayer[0];
            introBackdrop = cinematicBackdrop;
            sceneCamera = cameraComponent;
            skipInputEnabledAt = Time.unscaledTime + config.SkipInputGraceSeconds;
            CreateExhaustParticles();
            CreateSpeedLines();
            CreateWhiteFlash();
        }

        public void Play()
        {
            if (playRoutine == null)
            {
                playRoutine = StartCoroutine(PlayIntro());
            }
        }

        public void SkipIntro()
        {
            CompleteIntro();
        }

        private void Update()
        {
            if (completed || manager == null || manager.CurrentState != GameFlowState.Intro)
            {
                return;
            }

            if (Time.unscaledTime < skipInputEnabledAt)
            {
                return;
            }

            if (Input.anyKeyDown || HasFreshTouch() || Input.GetMouseButtonDown(0))
            {
                CompleteIntro();
            }
        }

        private static bool HasFreshTouch()
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerator PlayIntro()
        {
            if (playerTransform == null)
            {
                CompleteIntro();
                yield break;
            }

            ApplyBackgroundBoost(0.35f);
            SetExhaustIntensity(0.16f);
            SetIntroBackdropAlpha(1f);
            SetWhiteFlashAlpha(0f);
            yield return new WaitForSeconds(config.HoldSeconds);

            Vector3 start = playerTransform.position;
            Vector3 end = new Vector3(start.x, gameplayPosition.y + config.TargetYInset - config.GameplayStartYInset, start.z);
            float elapsed = 0f;

            while (!completed && elapsed < config.LaunchDurationSeconds)
            {
                elapsed += Time.deltaTime;
                float normalized = Mathf.Clamp01(elapsed / Mathf.Max(0.01f, config.LaunchDurationSeconds));
                float eased = normalized * normalized * (3f - (2f * normalized));
                float speedBlend = Mathf.Lerp(config.InitialSpeed, config.MaxSpeed, eased);
                playerTransform.position = Vector3.LerpUnclamped(start, end, eased);
                ApplyBackgroundBoost(Mathf.Lerp(0.65f, config.BackgroundBoost, speedBlend));
                SetExhaustIntensity(Mathf.Lerp(0.45f, config.ExhaustIntensity, speedBlend));
                AnimateIntroBackdrop(normalized);
                AnimateSpeedLines(normalized);
                AnimateWhiteFlash(normalized);
                yield return null;
            }

            CompleteIntro();
        }

        private void CompleteIntro()
        {
            if (completed)
            {
                return;
            }

            completed = true;
            if (playRoutine != null)
            {
                StopCoroutine(playRoutine);
                playRoutine = null;
            }

            ApplyBackgroundBoost(1f);
            SetExhaustIntensity(0f);
            SetWhiteFlashAlpha(0f);
            CleanupIntroVisuals();
            if (playerController != null)
            {
                playerController.ResetForGameplayPosition(gameplayPosition);
            }
            else if (playerTransform != null)
            {
                playerTransform.position = gameplayPosition;
            }

            if (manager != null)
            {
                manager.CompleteIntro();
            }
        }

        private void ApplyBackgroundBoost(float multiplier)
        {
            for (int i = 0; i < backgroundLayers.Length; i++)
            {
                if (backgroundLayers[i] != null)
                {
                    backgroundLayers[i].SetSpeedMultiplier(multiplier);
                }
            }
        }

        private void CreateExhaustParticles()
        {
            if (playerTransform == null)
            {
                return;
            }

            GameObject exhaustObject = new GameObject("LaunchExhaust");
            exhaustObject.transform.SetParent(playerTransform, false);
            exhaustObject.transform.localPosition = new Vector3(0f, -0.42f, -0.08f);
            exhaustObject.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);

            exhaustParticles = exhaustObject.AddComponent<ParticleSystem>();
            ParticleSystem.MainModule main = exhaustParticles.main;
            main.loop = true;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.18f, 0.36f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(1.2f, 2.8f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.09f, 0.22f);
            main.startColor = new ParticleSystem.MinMaxGradient(
                new Color(0.44f, 0.88f, 1f, 0.95f),
                new Color(1f, 0.42f, 0.08f, 0.82f));
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            ParticleSystem.EmissionModule emission = exhaustParticles.emission;
            emission.rateOverTime = 0f;

            ParticleSystem.ShapeModule shape = exhaustParticles.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 12f;
            shape.radius = 0.08f;

            ParticleSystemRenderer renderer = exhaustParticles.GetComponent<ParticleSystemRenderer>();
            renderer.sortingOrder = 11;
            renderer.material = new Material(Shader.Find("Sprites/Default"));

            exhaustParticles.Play();
        }

        private void SetExhaustIntensity(float intensity)
        {
            if (exhaustParticles == null)
            {
                return;
            }

            ParticleSystem.EmissionModule emission = exhaustParticles.emission;
            emission.rateOverTime = Mathf.Max(0f, intensity) * 70f;
        }

        private void AnimateIntroBackdrop(float normalized)
        {
            if (introBackdrop == null)
            {
                return;
            }

            float exitFade = Mathf.InverseLerp(0.76f, 1f, normalized);
            SetIntroBackdropAlpha(1f - exitFade);
            introBackdrop.transform.localScale *= 1f + (Time.deltaTime * Mathf.Lerp(0.015f, 0.045f, normalized));
            introBackdrop.transform.position = new Vector3(0f, Mathf.Lerp(0f, -0.8f, normalized), 7f);
        }

        private void SetIntroBackdropAlpha(float alpha)
        {
            if (introBackdrop == null)
            {
                return;
            }

            Color color = introBackdrop.color;
            color.a = Mathf.Clamp01(alpha);
            introBackdrop.color = color;
        }

        private void CreateSpeedLines()
        {
            if (sceneCamera == null)
            {
                return;
            }

            speedLines = new SpriteRenderer[14];
            float halfHeight = sceneCamera.orthographicSize + 1.5f;
            float halfWidth = sceneCamera.orthographicSize * sceneCamera.aspect;
            for (int i = 0; i < speedLines.Length; i++)
            {
                GameObject line = new GameObject("LaunchSpeedLine" + i);
                line.transform.position = new Vector3(Mathf.Lerp(-halfWidth, halfWidth, (i + 0.5f) / speedLines.Length), Random.Range(-halfHeight, halfHeight), -0.05f);
                line.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-3f, 3f));
                line.transform.localScale = new Vector3(Random.Range(0.025f, 0.055f), Random.Range(1.2f, 3.6f), 1f);

                SpriteRenderer renderer = line.AddComponent<SpriteRenderer>();
                renderer.sprite = RuntimeSpriteFactory.GetRoundedSquareSprite();
                renderer.sortingOrder = 18;
                renderer.color = new Color(0.65f, 0.95f, 1f, 0f);
                speedLines[i] = renderer;
            }
        }

        private void AnimateSpeedLines(float normalized)
        {
            if (speedLines == null || sceneCamera == null)
            {
                return;
            }

            float halfHeight = sceneCamera.orthographicSize + 1.8f;
            float alpha = Mathf.InverseLerp(0.16f, 0.46f, normalized) * (1f - Mathf.InverseLerp(0.86f, 1f, normalized));
            for (int i = 0; i < speedLines.Length; i++)
            {
                SpriteRenderer renderer = speedLines[i];
                if (renderer == null)
                {
                    continue;
                }

                Transform lineTransform = renderer.transform;
                float speed = Mathf.Lerp(7f, 19f, normalized) * Time.deltaTime;
                lineTransform.position += Vector3.down * speed;
                if (lineTransform.position.y < -halfHeight)
                {
                    lineTransform.position = new Vector3(lineTransform.position.x, halfHeight, lineTransform.position.z);
                }

                Color color = renderer.color;
                color.a = alpha * Random.Range(0.22f, 0.72f);
                renderer.color = color;
            }
        }

        private void CreateWhiteFlash()
        {
            if (sceneCamera == null)
            {
                return;
            }

            GameObject flash = new GameObject("LaunchWeatherFlash");
            flash.transform.position = new Vector3(0f, 0f, -0.1f);
            flash.transform.localScale = new Vector3(sceneCamera.orthographicSize * sceneCamera.aspect * 2.6f, sceneCamera.orthographicSize * 2.6f, 1f);

            whiteFlash = flash.AddComponent<SpriteRenderer>();
            whiteFlash.sprite = RuntimeSpriteFactory.GetRoundedSquareSprite();
            whiteFlash.sortingOrder = 40;
            whiteFlash.color = new Color(1f, 0.96f, 0.78f, 0f);
        }

        private void AnimateWhiteFlash(float normalized)
        {
            float alpha = Mathf.InverseLerp(0.72f, 0.9f, normalized) * (1f - Mathf.InverseLerp(0.92f, 1f, normalized));
            SetWhiteFlashAlpha(alpha * 0.55f);
        }

        private void SetWhiteFlashAlpha(float alpha)
        {
            if (whiteFlash == null)
            {
                return;
            }

            Color color = whiteFlash.color;
            color.a = Mathf.Clamp01(alpha);
            whiteFlash.color = color;
        }

        private void CleanupIntroVisuals()
        {
            if (introBackdrop != null)
            {
                Destroy(introBackdrop.gameObject);
                introBackdrop = null;
            }

            if (whiteFlash != null)
            {
                Destroy(whiteFlash.gameObject);
                whiteFlash = null;
            }

            if (speedLines != null)
            {
                for (int i = 0; i < speedLines.Length; i++)
                {
                    if (speedLines[i] != null)
                    {
                        Destroy(speedLines[i].gameObject);
                    }
                }

                speedLines = null;
            }
        }
    }
}
