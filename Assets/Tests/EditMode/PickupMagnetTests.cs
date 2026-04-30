using NUnit.Framework;
using UnityEngine;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class PickupMagnetTests
    {
        [Test]
        public void PickupMagnetRadius_IsSmallAroundPlayer()
        {
            Assert.That(PickupPresentation.PickupMagnetRadiusWorld, Is.GreaterThan(PickupPresentation.AmmoPackColliderRadius));
            Assert.That(PickupPresentation.PickupMagnetRadiusWorld, Is.LessThanOrEqualTo(1.55f));
        }

        [Test]
        public void PickupMagnet_MovesOnlyWhenInsideSmallRadius()
        {
            Vector3 player = Vector3.zero;
            Vector3 near = new Vector3(1.1f, 0f, 0f);
            Vector3 far = new Vector3(2.2f, 0f, 0f);

            Assert.That(PickupMagnet.TryMoveTowardPlayer(near, player, 0.1f, out Vector3 movedNear), Is.True);
            Assert.That(movedNear.x, Is.LessThan(near.x));

            Assert.That(PickupMagnet.TryMoveTowardPlayer(far, player, 0.1f, out Vector3 movedFar), Is.False);
            Assert.That(movedFar, Is.EqualTo(far));
        }
    }
}
