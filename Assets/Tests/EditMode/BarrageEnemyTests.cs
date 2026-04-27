using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    [TestFixture]
    public class BarrageEnemyTests
    {
        [Test]
        public void EnemyType_HasBarrageValue()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(EnemyType), EnemyType.Barrage));
        }

        [Test]
        public void BlockController_HasFireBarrageSpread()
        {
            var method = typeof(BlockController).GetMethod("FireBarrageSpread",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.IsNotNull(method, "BlockController should have FireBarrageSpread method");
        }

        [Test]
        public void BlockController_HasBarrageHoverTimer()
        {
            var field = typeof(BlockController).GetField("barrageHoverEndTime",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.IsNotNull(field, "BlockController should have a barrageHoverEndTime field");
            Assert.AreEqual(typeof(float), field.FieldType);
        }

        [Test]
        public void EnemyType_AllTypesDefined()
        {
            var values = System.Enum.GetValues(typeof(EnemyType));
            Assert.GreaterOrEqual(values.Length, 5);
            Assert.Contains(EnemyType.Normal, values);
            Assert.Contains(EnemyType.SelfDestruct, values);
            Assert.Contains(EnemyType.Shield, values);
            Assert.Contains(EnemyType.Healer, values);
            Assert.Contains(EnemyType.Barrage, values);
        }
    }
}
