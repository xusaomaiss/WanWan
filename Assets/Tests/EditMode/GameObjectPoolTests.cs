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

        [Test]
        public void Rent_AfterReturn_RemovesRuntimeComponentsAndChildren()
        {
            var pool = new GameObjectPool("Test", 0);
            var obj = pool.Rent();
            obj.AddComponent<SpriteRenderer>();
            obj.AddComponent<BoxCollider2D>();
            new GameObject("RuntimeChild").transform.SetParent(obj.transform, false);

            pool.Return(obj);
            var reused = pool.Rent();

            Assert.AreSame(obj, reused);
            Assert.That(reused.GetComponents<Component>().Length, Is.EqualTo(1));
            Assert.That(reused.transform.childCount, Is.EqualTo(0));
        }

        [Test]
        public void Rent_AfterReturn_ResetsTransformState()
        {
            var pool = new GameObjectPool("Test", 0);
            var obj = pool.Rent();
            obj.transform.position = new Vector3(4f, -3f, 2f);
            obj.transform.rotation = Quaternion.Euler(0f, 0f, 35f);
            obj.transform.localScale = new Vector3(2f, 3f, 1f);

            pool.Return(obj);
            var reused = pool.Rent();

            Assert.AreSame(obj, reused);
            Assert.That(reused.transform.position, Is.EqualTo(Vector3.zero));
            Assert.That(reused.transform.rotation, Is.EqualTo(Quaternion.identity));
            Assert.That(reused.transform.localScale, Is.EqualTo(Vector3.one));
        }
    }
}
