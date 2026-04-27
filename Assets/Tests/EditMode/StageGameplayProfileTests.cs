using System.Linq;
using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class StageGameplayProfileTests
    {
        [Test]
        public void GetGameplayProfile_ReturnsDistinctProfileForEveryStage()
        {
            StageGameplayProfile[] profiles = Enumerable.Range(0, StageCatalog.StageCount)
                .Select(StageCatalog.GetGameplayProfile)
                .ToArray();

            Assert.That(profiles, Has.All.Not.Null);
            Assert.That(profiles.Select(profile => profile.SignaturePowerups[0]).Distinct().Count(), Is.GreaterThanOrEqualTo(5));
            Assert.That(profiles.Select(profile => profile.FormationBias).Distinct().Count(), Is.EqualTo(StageCatalog.StageCount));
        }

        [Test]
        public void GetGameplayProfile_ConfiguresPowerMeterCapsuleGuarantee()
        {
            for (int i = 0; i < StageCatalog.StageCount; i++)
            {
                StageGameplayProfile profile = StageCatalog.GetGameplayProfile(i);

                Assert.That(profile.PowerMeterCapsuleBudget, Is.EqualTo(6 + (i * 2)));
                Assert.That(profile.GuaranteesPowerMeterActivation, Is.True);
            }
        }

        [Test]
        public void GetGameplayProfile_GuaranteesAllPlayableAmmoPacksAndMedicalPickup()
        {
            AmmoPowerupType[] expected =
            {
                AmmoPowerupType.Scatter,
                AmmoPowerupType.RapidFire,
                AmmoPowerupType.Pierce,
                AmmoPowerupType.Laser,
                AmmoPowerupType.Plasma,
                AmmoPowerupType.Burst,
                AmmoPowerupType.Homing,
                AmmoPowerupType.Wave,
                AmmoPowerupType.Guard
            };

            for (int i = 0; i < StageCatalog.StageCount; i++)
            {
                StageGameplayProfile profile = StageCatalog.GetGameplayProfile(i);

                Assert.That(profile.GuaranteedAmmoPacks, Is.EqualTo(expected));
                Assert.That(profile.GuaranteedMedicalPacks, Is.EqualTo(1));
                Assert.That(profile.GuaranteedPickupCount, Is.EqualTo(10));
            }
        }

        [Test]
        public void PowerCapsuleUnlockProgress_DistributesBudgetAcrossStage()
        {
            StageGameplayProfile profile = StageCatalog.GetGameplayProfile(0);

            Assert.That(profile.GetPowerCapsuleUnlockProgress(0), Is.EqualTo(0.14f).Within(0.001f));
            Assert.That(profile.GetPowerCapsuleUnlockProgress(2), Is.InRange(0.4f, 0.43f));
            Assert.That(profile.GetPowerCapsuleUnlockProgress(profile.PowerMeterCapsuleBudget - 1), Is.EqualTo(0.84f).Within(0.001f));
        }

        [Test]
        public void GuaranteedPickupUnlockProgress_KeepsAmmoAvailableIntoLateStage()
        {
            StageGameplayProfile profile = StageCatalog.GetGameplayProfile(0);

            Assert.That(profile.GetGuaranteedPickupUnlockProgress(0), Is.EqualTo(0.08f).Within(0.001f));
            Assert.That(profile.GetGuaranteedPickupUnlockProgress(5), Is.InRange(0.53f, 0.54f));
            Assert.That(profile.GetGuaranteedPickupUnlockProgress(profile.GuaranteedPickupCount - 1), Is.EqualTo(0.9f).Within(0.001f));
        }

        [Test]
        public void GetStageDurationMultiplier_UsesCompactStagePacingTable()
        {
            float[] expected = { 0.9f, 0.94f, 0.98f, 1.02f, 1.06f, 1.1f, 1.15f, 1.2f };

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.That(StageCatalog.GetStageDurationMultiplier(i), Is.EqualTo(expected[i]).Within(0.001f));
            }
        }

        [Test]
        public void ChooseThemedPowerup_PrefersStageSignatureDuringReward()
        {
            StageGameplayProfile profile = StageCatalog.GetGameplayProfile(3);

            AmmoPowerupType chosen = profile.ChooseThemedPowerup(0, true);

            Assert.That(chosen, Is.EqualTo(profile.SignaturePowerups[0]));
        }

        [Test]
        public void GetBombPickupSpawnArea_UsesMiddleLowerScreen()
        {
            SpawnArea area = BlockSpawner.GetBombPickupSpawnArea(-5f, 5f, -3f, 7f);

            Assert.That(area.MinX, Is.EqualTo(-4.3f).Within(0.001f));
            Assert.That(area.MaxX, Is.EqualTo(4.3f).Within(0.001f));
            Assert.That(area.MinY, Is.EqualTo(-1f).Within(0.001f));
            Assert.That(area.MaxY, Is.EqualTo(1f).Within(0.001f));
        }

        [Test]
        public void EnemyScale_KeepsNormalSizeAndShrinksToughAndElite()
        {
            Assert.That(BlockSpawner.GetEnemyVisualScale(false, false), Is.EqualTo(new UnityEngine.Vector3(0.72f, 0.7f, 1f)));
            Assert.That(BlockSpawner.GetEnemyVisualScale(true, false), Is.EqualTo(new UnityEngine.Vector3(0.68f, 0.66f, 1f)));
            Assert.That(BlockSpawner.GetEnemyVisualScale(false, true), Is.EqualTo(new UnityEngine.Vector3(0.74f, 0.72f, 1f)));
            Assert.That(BlockSpawner.GetEnemyColliderSize(false, true).x, Is.LessThan(0.5f));
            Assert.That(BlockSpawner.GetEnemyColliderSize(true, false).x, Is.LessThan(0.44f));
        }
    }
}
