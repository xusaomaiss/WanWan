using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class WeaponDisplayTests
    {
        [TestCase(WeaponType.Spread, "武器 扇形弹 1级")]
        [TestCase(WeaponType.Laser, "武器 激光弹 1级")]
        [TestCase(WeaponType.Burst, "武器 爆裂弹 1级")]
        [TestCase(WeaponType.Plasma, "武器 等离子弹 1级")]
        public void GetCurrentWeaponDisplayText_ReturnsLocalizedWeaponAndLevel(WeaponType type, string expected)
        {
            PlayerWeaponState state = new PlayerWeaponState();

            state.SetWeapon(type);

            Assert.That(state.GetCurrentWeaponDisplayText(), Is.EqualTo(expected));
        }
    }
}
