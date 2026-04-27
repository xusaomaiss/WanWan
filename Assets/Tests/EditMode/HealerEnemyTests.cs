using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    [TestFixture]
    public class HealerEnemyTests
    {
        [Test]
        public void EnemyType_HasHealerValue()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(EnemyType), EnemyType.Healer));
        }

        [Test]
        public void BlockController_HasHealMethod()
        {
            var method = typeof(BlockController).GetMethod("Heal",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            Assert.IsNotNull(method, "BlockController should have a public Heal method");
            var parameters = method.GetParameters();
            Assert.AreEqual(1, parameters.Length);
            Assert.AreEqual(typeof(int), parameters[0].ParameterType);
        }

        [Test]
        public void BlockController_HasHealTimerField()
        {
            var field = typeof(BlockController).GetField("healTimer",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.IsNotNull(field, "BlockController should have a healTimer field");
            Assert.AreEqual(typeof(float), field.FieldType);
        }

        [Test]
        public void BlockController_HasUpdateHealerBehavior()
        {
            var method = typeof(BlockController).GetMethod("UpdateHealerBehavior",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.IsNotNull(method, "BlockController should have UpdateHealerBehavior method");
        }
    }
}
