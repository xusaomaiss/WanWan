using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    [TestFixture]
    public class PlayerSkillTests
    {
        [Test]
        public void PlayerSkill_AllTypesDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(PlayerSkill), PlayerSkill.None));
            Assert.IsTrue(System.Enum.IsDefined(typeof(PlayerSkill), PlayerSkill.ExtraShield));
            Assert.IsTrue(System.Enum.IsDefined(typeof(PlayerSkill), PlayerSkill.BetterGraze));
            Assert.IsTrue(System.Enum.IsDefined(typeof(PlayerSkill), PlayerSkill.Level2Weapon));
        }

        [Test]
        public void GetDisplayName_ReturnsCorrectLabels()
        {
            Assert.AreEqual("初始护盾", PlayerSkill.ExtraShield.GetDisplayName());
            Assert.AreEqual("擦弹强化", PlayerSkill.BetterGraze.GetDisplayName());
            Assert.AreEqual("火力预热", PlayerSkill.Level2Weapon.GetDisplayName());
            Assert.AreEqual(string.Empty, PlayerSkill.None.GetDisplayName());
        }

        [Test]
        public void ShipDefinition_HasSkillProperty()
        {
            var prop = typeof(ShipDefinition).GetProperty("Skill");
            Assert.IsNotNull(prop);
            Assert.AreEqual(typeof(PlayerSkill), prop.PropertyType);
        }

        [Test]
        public void ShipDefinition_GreenHasExtraShieldSkill()
        {
            var definition = ShipDefinition.Get(PlayerShipType.Green);
            Assert.AreEqual(PlayerSkill.ExtraShield, definition.Skill);
        }

        [Test]
        public void ShipDefinition_BlueHasBetterGrazeSkill()
        {
            var definition = ShipDefinition.Get(PlayerShipType.Blue);
            Assert.AreEqual(PlayerSkill.BetterGraze, definition.Skill);
        }

        [Test]
        public void ShipDefinition_AllSelectableShipsHaveUiStats()
        {
            foreach (PlayerShipType shipType in MenuBootstrap.ShipSelectRoster)
            {
                var definition = ShipDefinition.Get(shipType);
                Assert.That(definition.DisplayName, Is.Not.Empty, shipType.ToString());
                Assert.That(definition.MainWeapon, Is.Not.Empty, shipType.ToString());
                Assert.That(definition.PowerStars, Is.InRange(1, 5), shipType.ToString());
                Assert.That(definition.AttackStars, Is.InRange(1, 5), shipType.ToString());
                Assert.That(definition.DefenseStars, Is.InRange(1, 5), shipType.ToString());
                Assert.That(definition.SpeedStars, Is.InRange(1, 5), shipType.ToString());
            }
        }

        [Test]
        public void ShipDefinition_UsesChineseDisplayNames()
        {
            Assert.That(ShipDefinition.Get(PlayerShipType.Green).DisplayName, Is.EqualTo("赤焰战机"));
            Assert.That(ShipDefinition.Get(PlayerShipType.Blue).DisplayName, Is.EqualTo("蓝翼A1"));
            Assert.That(ShipDefinition.Get(PlayerShipType.Yellow).DisplayName, Is.EqualTo("黄蜂战机"));
            Assert.That(ShipDefinition.Get(PlayerShipType.Purple).DisplayName, Is.EqualTo("紫电战机"));
            Assert.That(ShipDefinition.Get(PlayerShipType.Azure).DisplayName, Is.EqualTo("苍蓝战机"));
        }

        [Test]
        public void GameManager_HasApplyShipSkillMethod()
        {
            var method = typeof(GameManager).GetMethod("ApplyShipSkill",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.IsNotNull(method, "GameManager should have ApplyShipSkill method");
        }
    }
}
