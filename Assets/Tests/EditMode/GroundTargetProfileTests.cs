using NUnit.Framework;
using UnityEngine;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class GroundTargetProfileTests
    {
        [Test]
        public void Get_TurretHasMoreHealthAndScoreThanTankAtSameDifficulty()
        {
            GroundTargetProfile tank = GroundTargetProfile.Get(GroundTargetType.Tank, 1f);
            GroundTargetProfile turret = GroundTargetProfile.Get(GroundTargetType.Turret, 1f);

            Assert.That(turret.HitPoints, Is.GreaterThan(tank.HitPoints));
            Assert.That(turret.ScoreValue, Is.GreaterThan(tank.ScoreValue));
            Assert.That(turret.FireInterval, Is.LessThan(tank.FireInterval));
        }

        [Test]
        public void Get_IncreasesHealthAndScoreWithDifficultyMultiplier()
        {
            GroundTargetProfile baseTank = GroundTargetProfile.Get(GroundTargetType.Tank, 1f);
            GroundTargetProfile hardTank = GroundTargetProfile.Get(GroundTargetType.Tank, 1.8f);

            Assert.That(hardTank.HitPoints, Is.GreaterThan(baseTank.HitPoints));
            Assert.That(hardTank.ScoreValue, Is.GreaterThan(baseTank.ScoreValue));
            Assert.That(hardTank.FireInterval, Is.LessThan(baseTank.FireInterval));
        }

        [Test]
        public void Get_ClampsDifficultyBelowOne()
        {
            GroundTargetProfile low = GroundTargetProfile.Get(GroundTargetType.Tank, 0.2f);
            GroundTargetProfile normal = GroundTargetProfile.Get(GroundTargetType.Tank, 1f);

            Assert.That(low.HitPoints, Is.EqualTo(normal.HitPoints));
            Assert.That(low.ScoreValue, Is.EqualTo(normal.ScoreValue));
            Assert.That(Mathf.Approximately(low.FireInterval, normal.FireInterval), Is.True);
        }
    }
}
