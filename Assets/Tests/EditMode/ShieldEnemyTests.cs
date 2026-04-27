using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    [TestFixture]
    public class ShieldEnemyTests
    {
        [Test]
        public void EnemyType_HasShieldValue()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(EnemyType), EnemyType.Shield));
        }

        [Test]
        public void BlockController_HasShieldAbsorbedField()
        {
            var field = typeof(BlockController).GetField("shieldAbsorbedFirstHit",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.IsNotNull(field, "BlockController should have shieldAbsorbedFirstHit field");
            Assert.AreEqual(typeof(bool), field.FieldType);
        }

        [Test]
        public void BlockController_HasRestoreEnemyColor()
        {
            var method = typeof(BlockController).GetMethod("RestoreEnemyColor",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.IsNotNull(method, "BlockController should have RestoreEnemyColor method for shield flash feedback");
        }

        [Test]
        public void EnemyType_Shield_HasCorrectOrdinal()
        {
            int value = (int)EnemyType.Shield;
            Assert.Greater(value, 0);
        }
    }
}
