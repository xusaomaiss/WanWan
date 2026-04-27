using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    [TestFixture]
    public class SelfDestructEnemyTests
    {
        [Test]
        public void EnemyType_HasSelfDestructValue()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(EnemyType), EnemyType.SelfDestruct));
        }

        [Test]
        public void BlockController_HasEnemyTypeParameter()
        {
            var method = typeof(BlockController).GetMethod("Initialize");
            var parameters = method.GetParameters();
            bool hasEnemyType = false;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType == typeof(EnemyType))
                {
                    hasEnemyType = true;
                    break;
                }
            }
            Assert.IsTrue(hasEnemyType, "BlockController.Initialize should have an EnemyType parameter");
        }

        [Test]
        public void BlockController_HasUpdateSelfDestructBehavior()
        {
            var method = typeof(BlockController).GetMethod("UpdateSelfDestructBehavior",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.IsNotNull(method, "BlockController should have UpdateSelfDestructBehavior method");
        }

        [Test]
        public void BlockController_HasEnemyTypeField()
        {
            var field = typeof(BlockController).GetField("enemyType",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.IsNotNull(field, "BlockController should have an enemyType field");
            Assert.AreEqual(typeof(EnemyType), field.FieldType);
        }
    }
}
