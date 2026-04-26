using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class ScoreRewardConfigTests
    {
        [Test]
        public void GetStageClearBonus_IncludesBaseAndResourceBonuses()
        {
            int bonus = ScoreRewardConfig.GetStageClearBonus(2, 1, 3);

            Assert.That(bonus, Is.EqualTo(
                ScoreRewardConfig.StageClearBaseBonus
                + (2 * ScoreRewardConfig.RemainingBombBonus)
                + ScoreRewardConfig.ShieldChargeBonus
                + (3 * ScoreRewardConfig.ComboMultiplierBonus)));
        }

        [Test]
        public void GetStageClearBonus_ClampsNegativeInputs()
        {
            int bonus = ScoreRewardConfig.GetStageClearBonus(-2, -1, -4);

            Assert.That(bonus, Is.EqualTo(ScoreRewardConfig.StageClearBaseBonus + ScoreRewardConfig.ComboMultiplierBonus));
        }
    }
}
