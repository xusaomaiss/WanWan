namespace Wanwan.Runtime
{
    public class PowerMeterState
    {
        public const int SlotCount = 4;

        private static readonly PowerMeterUpgrade[] Slots =
        {
            PowerMeterUpgrade.SpeedUp,
            PowerMeterUpgrade.Missile,
            PowerMeterUpgrade.Double,
            PowerMeterUpgrade.Laser
        };

        private static readonly string[] Labels =
        {
            "SPEED",
            "MISSILE",
            "DOUBLE",
            "LASER"
        };

        public int CollectedCapsules { get; private set; }
        public int HighlightedIndex { get; private set; } = -1;
        public PowerMeterUpgrade HighlightedUpgrade => HighlightedIndex < 0 ? PowerMeterUpgrade.None : Slots[HighlightedIndex];
        public bool IsFull => CollectedCapsules >= SlotCount;
        public bool CanActivate => IsFull;

        public void CollectCapsule()
        {
            if (CollectedCapsules < SlotCount)
            {
                CollectedCapsules++;
                HighlightedIndex = CollectedCapsules - 1;
            }
        }

        public PowerMeterUpgrade ActivateHighlightedUpgrade()
        {
            if (!CanActivate)
            {
                return PowerMeterUpgrade.None;
            }

            PowerMeterUpgrade upgrade = HighlightedUpgrade;
            CollectedCapsules = 0;
            HighlightedIndex = -1;
            return upgrade;
        }

        public string BuildHudText()
        {
            string text = string.Empty;
            for (int i = 0; i < Labels.Length; i++)
            {
                if (i > 0)
                {
                    text += " ";
                }

                if (i < CollectedCapsules)
                {
                    text += i == HighlightedIndex ? $"[>{Labels[i]}<]" : $"[{Labels[i]}]";
                }
                else
                {
                    text += Labels[i];
                }
            }

            return text;
        }
    }
}
