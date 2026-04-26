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
            manager = gameManager;
            playerTransform = player;
            playerController = player == null ? null : player.GetComponent<PlayerController>();
            gameplayPosition = playerGameplayPosition;
            config = introConfig;
            backgroundLayers = scrollingLayers ?? new ScrollingBackgroundLayer[0];
            skipInputEnabledAt = Time.unscaledTime + config.SkipInputGraceSeconds;
            CreateExhaustParticles();
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
    }
}
