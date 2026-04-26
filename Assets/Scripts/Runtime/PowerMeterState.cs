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

        private static readonly string[] Labels =
        {
            "SPEED",
            "MISSILE",
            "DOUBLE",
            "LASER",
            "OPTION",
            "SHIELD"
        };

        public int HighlightedIndex { get; private set; } = -1;
        public PowerMeterUpgrade HighlightedUpgrade => HighlightedIndex < 0 ? PowerMeterUpgrade.None : Slots[HighlightedIndex];
        public bool CanActivate => HighlightedUpgrade != PowerMeterUpgrade.None;

        public void CollectCapsule()
        {
            if (HighlightedIndex < SlotCount - 1)
            {
                HighlightedIndex++;
            }
        }

        public PowerMeterUpgrade ActivateHighlightedUpgrade()
        {
            PowerMeterUpgrade upgrade = HighlightedUpgrade;
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

                text += i == HighlightedIndex ? $">{Labels[i]}<" : Labels[i];
            }

            return text;
        }
    }
}
