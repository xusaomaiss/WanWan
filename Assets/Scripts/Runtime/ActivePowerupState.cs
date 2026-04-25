using UnityEngine;

namespace Wanwan.Runtime
{
    public class ActivePowerupState
    {
        private const int MaxLevel = 3;

        public AmmoPowerupType Type { get; private set; }
        public int Level { get; private set; }
        public float RemainingSeconds { get; private set; }
        public bool HasActivePowerup => Type != AmmoPowerupType.None && RemainingSeconds > 0f;

        public void Activate(AmmoPowerupType type, float durationSeconds)
        {
            if (type == AmmoPowerupType.None || type == AmmoPowerupType.Normal)
            {
                Clear();
                return;
            }

            Level = Type == type && HasActivePowerup ? Mathf.Min(MaxLevel, Level + 1) : 1;
            Type = type;
            RemainingSeconds = Mathf.Max(0f, durationSeconds);
        }

        public void Tick(float deltaSeconds)
        {
            if (!HasActivePowerup)
            {
                return;
            }

            RemainingSeconds = Mathf.Max(0f, RemainingSeconds - deltaSeconds);
            if (RemainingSeconds <= 0f)
            {
                Clear();
            }
        }

        public void Clear()
        {
            Type = AmmoPowerupType.None;
            Level = 0;
            RemainingSeconds = 0f;
        }
    }
}
