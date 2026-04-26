using UnityEngine;

namespace Wanwan.Runtime
{
    public static class ScoreRewardConfig
    {
        public const int BossDefeatBaseScore = 10000;
        public const int StageClearBaseBonus = 100000;
        public const int RemainingBombBonus = 5000;
        public const int ShieldChargeBonus = 8000;
        public const int ComboMultiplierBonus = 2500;

        public static int GetStageClearBonus(int remainingBombs, int shieldCharges, int maxComboMultiplier)
        {
            return StageClearBaseBonus
                + (Mathf.Max(0, remainingBombs) * RemainingBombBonus)
                + (Mathf.Max(0, shieldCharges) * ShieldChargeBonus)
                + (Mathf.Max(1, maxComboMultiplier) * ComboMultiplierBonus);
        }
    }
}
