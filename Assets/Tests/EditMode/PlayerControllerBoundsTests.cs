using NUnit.Framework;
using UnityEngine;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class PlayerControllerBoundsTests
    {
        [Test]
        public void ClampToPlayableBounds_AllowsShipToReachScreenEdgeInset()
        {
            GameObject playerObject = new GameObject("Player");
            PlayerController controller = playerObject.AddComponent<PlayerController>();
            controller.Initialize(null, null, null, -3f, 3f, -9f, 9f);

            Vector2 left = controller.ClampToPlayableBoundsForTests(new Vector2(-99f, 0f));
            Vector2 right = controller.ClampToPlayableBoundsForTests(new Vector2(99f, 0f));

            Assert.That(left.x, Is.EqualTo(-2.5f));
            Assert.That(right.x, Is.EqualTo(2.5f));

            Object.DestroyImmediate(playerObject);
        }
    }
}
