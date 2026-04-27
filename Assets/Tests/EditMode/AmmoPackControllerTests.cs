using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class AmmoPackControllerTests
    {
        [Test]
        public void ApplyMotionStep_BouncesOffSideAndTopBounds()
        {
            GameObject packObject = new GameObject("AmmoPack");
            AmmoPackController pack = packObject.AddComponent<AmmoPackController>();
            packObject.transform.position = new Vector3(4.98f, 7.04f, 0f);
            pack.ConfigureMotionForTests(new Vector2(2f, 3f), -5f, 5f, 7f, -4f);

            pack.ApplyMotionStepForTests(0.1f);

            Assert.That(packObject.transform.position.x, Is.LessThanOrEqualTo(4.72f));
            Assert.That(GetVelocity(pack).x, Is.LessThan(0f));
            Assert.That(packObject.transform.position.y, Is.LessThanOrEqualTo(6.72f));
            Assert.That(GetVelocity(pack).y, Is.LessThan(0f));
            Object.DestroyImmediate(packObject);
        }

        [Test]
        public void ApplyMotionStep_MarksPackResolvedAfterBottomDespawn()
        {
            GameObject packObject = new GameObject("AmmoPack");
            AmmoPackController pack = packObject.AddComponent<AmmoPackController>();
            packObject.transform.position = new Vector3(0f, -4.2f, 0f);
            pack.ConfigureMotionForTests(Vector2.down, -5f, 5f, 7f, -4f);

            pack.ApplyMotionStepForTests(0.1f);

            Assert.That(pack.IsResolvedForTests, Is.True);
            Object.DestroyImmediate(packObject);
        }

        private static Vector2 GetVelocity(AmmoPackController pack)
        {
            FieldInfo field = typeof(AmmoPackController).GetField("velocity", BindingFlags.Instance | BindingFlags.NonPublic);
            return (Vector2)field.GetValue(pack);
        }
    }
}
