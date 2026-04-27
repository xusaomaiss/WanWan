using NUnit.Framework;
using UnityEngine;
using Wanwan.Runtime.Pools;

namespace Wanwan.Tests.EditMode
{
    [TestFixture]
    public class GameObjectPoolTests
    {
        [Test]
        public void Rent_ReturnsNewObjectWhenPoolEmpty()
        {
            var pool = new GameObjectPool("Test", 0);
            var obj = pool.Rent();
            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.activeSelf);
        }

        [Test]
        public void Return_PutsObjectBackForReuse()
        {
            var pool = new GameObjectPool("Test", 2);
            var obj = pool.Rent();
            pool.Return(obj);
            var reused = pool.Rent();
            Assert.AreSame(obj, reused);
        }

        [Test]
        public void PreWarm_CreatesSpecifiedNumberOfObjects()
        {
            var pool = new GameObjectPool("Test", 5);
            pool.PreWarm();
            for (int i = 0; i < 5; i++) pool.Rent();
            var sixth = pool.Rent();
            Assert.IsNotNull(sixth);
        }

        [Test]
        public void Overflow_CreatesNewObjectWhenPoolExhausted()
        {
            var pool = new GameObjectPool("Test", 1);
            pool.PreWarm();
            var first = pool.Rent();
            var overflow = pool.Rent();
            Assert.IsNotNull(overflow);
            Assert.AreNotSame(first, overflow);
        }

        [Test]
        public void ReturnedObject_IsDeactivated()
        {
            var pool = new GameObjectPool("Test", 1);
            var obj = pool.Rent();
            pool.Return(obj);
            Assert.IsFalse(obj.activeSelf);
        }

        [Test]
        public void ReturnNull_DoesNotCrash()
        {
            var pool = new GameObjectPool("Test", 1);
            pool.Return(null);
            Assert.Pass();
        }
    }
}
