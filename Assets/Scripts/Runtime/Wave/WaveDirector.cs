using System;
using UnityEngine;

namespace Wanwan.Runtime
{
    public enum WavePhase
    {
        Calm,
        Pressure,
        Burst,
        Reward
    }

    public class WaveDirector
    {
        public const float CalmDurationSeconds = 5f;
        public const float PressureDurationSeconds = 6f;
        public const float BurstDurationSeconds = 5f;
        public const float RewardDurationSeconds = 3f;

        private WavePhase currentPhase = WavePhase.Calm;
        private float phaseElapsedSeconds;

        public event Action<WavePhase> PhaseChanged;

        public void UpdatePhase(float deltaTime)
        {
            if (deltaTime <= 0f)
            {
                return;
            }

            phaseElapsedSeconds += deltaTime;
            float duration = GetDuration(currentPhase);
            if (phaseElapsedSeconds < duration)
            {
                return;
            }

            phaseElapsedSeconds = Mathf.Max(0f, phaseElapsedSeconds - duration);
            OnPhaseChanged(GetNextPhase(currentPhase));
        }

        public WavePhase GetCurrentPhase()
        {
            return currentPhase;
        }

        public float GetPhaseProgress()
        {
            return Mathf.Clamp01(phaseElapsedSeconds / GetDuration(currentPhase));
        }

        public void OnPhaseChanged(WavePhase newPhase)
        {
            currentPhase = newPhase;
            phaseElapsedSeconds = 0f;
            PhaseChanged?.Invoke(currentPhase);
        }

        public static float GetDuration(WavePhase phase)
        {
            switch (phase)
            {
                case WavePhase.Pressure:
                    return PressureDurationSeconds;
                case WavePhase.Burst:
                    return BurstDurationSeconds;
                case WavePhase.Reward:
                    return RewardDurationSeconds;
                default:
                    return CalmDurationSeconds;
            }
        }

        private static WavePhase GetNextPhase(WavePhase phase)
        {
            switch (phase)
            {
                case WavePhase.Calm:
                    return WavePhase.Pressure;
                case WavePhase.Pressure:
                    return WavePhase.Burst;
                case WavePhase.Burst:
                    return WavePhase.Reward;
                default:
                    return WavePhase.Calm;
            }
        }
    }
}
