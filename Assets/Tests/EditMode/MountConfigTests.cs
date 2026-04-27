using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class MountConfigTests
    {
        [TestCase(MountType.MissilePod)]
        [TestCase(MountType.DefenseDrone)]
        [TestCase(MountType.ShieldEmitter)]
        public void Get_ReturnsPlayableMountConfig(MountType type)
        {
            MountConfig config = MountConfig.Get(type);

            Assert.That(config.Type, Is.EqualTo(type));
            Assert.That(config.Cost, Is.GreaterThan(0));
            Assert.That(config.DisplayName, Is.Not.Empty);
            Assert.That(config.Description, Is.Not.Empty);
            Assert.That(config.PurchaseUnits, Is.GreaterThan(0));
            Assert.That(config.UnitLabel, Is.Not.Empty);
        }

        [Test]
        public void GetPlayableMounts_ReturnsThreeChoices()
        {
            MountType[] mounts = MountConfig.GetPlayableMounts();

            Assert.That(mounts, Is.EqualTo(new[] { MountType.MissilePod, MountType.DefenseDrone, MountType.ShieldEmitter }));
        }
    }
}
