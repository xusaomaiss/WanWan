using NUnit.Framework;
using UnityEngine;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class PlayerControllerBoundsTests
    {
        [Test]
        public void ClampToPlayableBounds_AllowsShipToCoverEdgeEnemyLanes()
        {
            GameObject playerObject = new GameObject("Player");
            PlayerController controller = playerObject.AddComponent<PlayerController>();
            controller.Initialize(null, null, null, -3f, 3f, -9f, 9f);

            Vector2 left = controller.ClampToPlayableBoundsForTests(new Vector2(-99f, 0f));
            Vector2 right = controller.ClampToPlayableBoundsForTests(new Vector2(99f, 0f));

            Assert.That(left.x, Is.EqualTo(-2.88f));
            Assert.That(right.x, Is.EqualTo(2.88f));

            Object.DestroyImmediate(playerObject);
        }

        [Test]
        public void IsPointerOverUi_ReturnsFalseWithoutEventSystem()
        {
            Assert.That(PlayerController.IsPointerOverUi(), Is.False);
        }
    }
}
