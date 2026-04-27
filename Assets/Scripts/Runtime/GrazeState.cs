using UnityEngine;

namespace Wanwan.Runtime
{
    public class GrazeState
    {
        public int CurrentGrazeStreak { get; private set; }
        public int TotalGrazeCount { get; private set; }
        public float FocusMeter { get; private set; }
        public bool IsFocusActive { get; private set; }

        public float GrazeRadius { get; set; } = 1.5f;

        private const float FocusMeterMax = 100f;
        private const float FocusMeterDecayRate = 5f;
        private const float FocusThreshold = 80f;

        public bool RegisterGraze()
        {
            CurrentGrazeStreak++;
            TotalGrazeCount++;
            FocusMeter = Mathf.Min(FocusMeterMax, FocusMeter + 8f);

            if (FocusMeter >= FocusThreshold && !IsFocusActive)
            {
                IsFocusActive = true;
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
                FocusMeter = Mathf.Max(0f, FocusMeter - FocusMeterDecayRate * 2f * delta);
                if (FocusMeter <= 0f)
                {
                    IsFocusActive = false;
                }
            }
        }
    }
}
