namespace Wanwan.Runtime
{
    public class PowerMeterState
    {
        public const int SlotCount = 6;

        private static readonly PowerMeterUpgrade[] Slots =
        {
            PowerMeterUpgrade.SpeedUp,
            PowerMeterUpgrade.Missile,
            PowerMeterUpgrade.Double,
            PowerMeterUpgrade.Laser,
            PowerMeterUpgrade.Option,
            PowerMeterUpgrade.Shield
        };

        public static readonly string[] SlotLabels =
        {
            "SPEED",
            "MISSILE",
            "DOUBLE",
            "LASER",
            "OPTION",
            "SHIELD"
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
            for (int i = 0; i < SlotLabels.Length; i++)
            {
                if (i > 0)
                {
                    text += " ";
                }

                if (i < CollectedCapsules)
                {
                    text += i == HighlightedIndex ? $"[>{SlotLabels[i]}<]" : $"[{SlotLabels[i]}]";
                }
                else
                {
                    text += SlotLabels[i];
                }
            }

            return text;
        }
    }
}
