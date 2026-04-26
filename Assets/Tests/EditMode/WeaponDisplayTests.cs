using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class WeaponDisplayTests
    {
        [TestCase(WeaponType.Spread, "Weapon: 扇形弹 Lv.1")]
        [TestCase(WeaponType.Laser, "Weapon: 激光弹 Lv.1")]
        [TestCase(WeaponType.Homing, "Weapon: 追踪弹 Lv.1")]
        [TestCase(WeaponType.Burst, "Weapon: 爆裂弹 Lv.1")]
        public void GetCurrentWeaponDisplayText_ReturnsLocalizedWeaponAndLevel(WeaponType type, string expected)
        {
            PlayerWeaponState state = new PlayerWeaponState();

            state.SetWeapon(type);

            Assert.That(state.GetCurrentWeaponDisplayText(), Is.EqualTo(expected));
        }
    }
}
