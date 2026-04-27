using UnityEngine;

namespace Wanwan.Runtime
{
    public class GrazeState
    {
        public int CurrentGrazeStreak { get; private set; }
        public int TotalGrazeCount { get; private set; }
        public float FocusMeter { get; private set; }
        public bool IsFocusActive { get; private set; }
        public float FocusRemainingSeconds { get; private set; }
        public float FocusNormalized => FocusMeter / FocusMeterMax;
        public const float FocusDurationSeconds = 5f;

        public float GrazeRadius { get; set; } = 1.5f;

        private const float FocusMeterMax = 100f;
        private const float FocusMeterDecayRate = 5f;
        private const float FocusThreshold = 80f;
        private float focusStartMeter;

        public bool RegisterGraze()
        {
            CurrentGrazeStreak++;
            TotalGrazeCount++;
            FocusMeter = Mathf.Min(FocusMeterMax, FocusMeter + 8f);

            if (FocusMeter >= FocusThreshold && !IsFocusActive)
            {
                IsFocusActive = true;
                FocusRemainingSeconds = FocusDurationSeconds;
                focusStartMeter = FocusMeter;
                return true;
            }
            return false;
        }

        public void BreakStreak()
        {
            CurrentGrazeStreak = 0;
        }

        public void Tick(float delta)
        {
            if (!IsFocusActive)
            {
                FocusMeter = Mathf.Max(0f, FocusMeter - FocusMeterDecayRate * delta);
            }
            else
            {
                FocusRemainingSeconds = Mathf.Max(0f, FocusRemainingSeconds - delta);
                FocusMeter = Mathf.Lerp(0f, focusStartMeter, FocusRemainingSeconds / FocusDurationSeconds);
                if (FocusRemainingSeconds <= 0f)
                {
                    IsFocusActive = false;
                    FocusMeter = 0f;
                }
            }
        }
    }
}
