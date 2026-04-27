using NUnit.Framework;
using Wanwan.Runtime;
using Wanwan.Runtime.Weapons;

namespace Wanwan.Tests.EditMode
{
    [TestFixture]
    public class ModuleSynergyTests
    {
        [Test]
        public void Check_ReturnsNull_WhenLessThanTwoModules()
        {
            Assert.IsNull(ModuleSynergy.Check(new WeaponModuleType[0]));
            Assert.IsNull(ModuleSynergy.Check(new[] { WeaponModuleType.Pierce }));
            Assert.IsNull(ModuleSynergy.Check(null));
        }

        [Test]
        public void Check_FindsPierceWaveSynergy()
        {
            var equipped = new[] { WeaponModuleType.Pierce, WeaponModuleType.Wave };
            var result = ModuleSynergy.Check(equipped);
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual("贯穿弹附带横向扩散", result.Value.DisplayName);
        }

        [Test]
        public void Check_FindsHomingRapidFireSynergy()
        {
            var equipped = new[] { WeaponModuleType.RapidFire, WeaponModuleType.Homing };
            var result = ModuleSynergy.Check(equipped);
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual("追踪弹射速提升", result.Value.DisplayName);
        }

        [Test]
        public void Check_OrderIndependent()
        {
            var ab = ModuleSynergy.Check(new[] { WeaponModuleType.Pierce, WeaponModuleType.Wave });
            var ba = ModuleSynergy.Check(new[] { WeaponModuleType.Wave, WeaponModuleType.Pierce });
            Assert.IsTrue(ab.HasValue);
            Assert.IsTrue(ba.HasValue);
        }

        [Test]
        public void Check_ReturnsNull_WhenNoSynergyMatch()
        {
            var equipped = new[] { WeaponModuleType.Guard, WeaponModuleType.Pierce };
            Assert.IsNull(ModuleSynergy.Check(equipped));
        }

        [Test]
        public void SynergyDefinition_HasExpectedFields()
        {
            var type = typeof(SynergyDefinition);
            Assert.IsNotNull(type.GetField("ModuleA"));
            Assert.IsNotNull(type.GetField("ModuleB"));
            Assert.IsNotNull(type.GetField("DisplayName"));
        }
    }
}
