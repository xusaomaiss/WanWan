using UnityEngine;

namespace Wanwan.Runtime
{
    public class ComboState
    {
        public int CurrentCombo { get; private set; }
        public int MaxCombo { get; private set; }
        public int CurrentMultiplier => GetMultiplier(CurrentCombo);
        public int MaxMultiplier { get; private set; } = 1;

        public void RegisterKill(int comboGain = 1)
        {
            CurrentCombo += Mathf.Max(0, comboGain);
            MaxCombo = Mathf.Max(MaxCombo, CurrentCombo);
            MaxMultiplier = Mathf.Max(MaxMultiplier, CurrentMultiplier);
        }

        public void RegisterBossPhaseClear()
        {
            RegisterKill(5);
        }

        public void RegisterBossDefeated()
        {
            RegisterKill(10);
        }

        public void BreakCombo()
        {
            CurrentCombo = 0;
        }

        public string GetDisplayText()
        {
            return $"连击 {CurrentCombo}  倍率 {CurrentMultiplier}倍";
        }

        public static int GetMultiplier(int combo)
        {
            if (combo >= 100)
            {
                return 5;
            }

            if (combo >= 50)
            {
                return 4;
            }

            if (combo >= 25)
            {
                return 3;
            }

            return combo >= 10 ? 2 : 1;
        }
    }
}
