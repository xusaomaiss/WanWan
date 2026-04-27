using UnityEngine;

namespace Wanwan.Runtime
{
    public class ScoreCounter
    {
        public int CurrentScore { get; private set; }
        public bool IsComplete { get; private set; }

        private int targetScore;
        private float elapsed;
        private const float DurationSeconds = 1.5f;

        public void Start(int target)
        {
            targetScore = target;
            CurrentScore = 0;
            elapsed = 0f;
            IsComplete = false;
        }

        public void Tick(float delta)
        {
            if (IsComplete) return;

            elapsed += delta;
            float t = Mathf.Clamp01(elapsed / DurationSeconds);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            CurrentScore = Mathf.FloorToInt(targetScore * eased);

            if (t >= 1f)
            {
                CurrentScore = targetScore;
                IsComplete = true;
            }
        }
    }
}
