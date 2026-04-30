using UnityEngine;

namespace Wanwan.Runtime
{
    public static class PickupMagnet
    {
        public static bool TryMoveTowardPlayer(Vector3 currentPosition, Vector3 playerPosition, float deltaSeconds, out Vector3 nextPosition)
        {
            float distance = Vector2.Distance(currentPosition, playerPosition);
            if (distance > PickupPresentation.PickupMagnetRadiusWorld)
            {
                nextPosition = currentPosition;
                return false;
            }

            float dt = Mathf.Max(0f, deltaSeconds);
            float strength = Mathf.InverseLerp(PickupPresentation.PickupMagnetRadiusWorld, PickupPresentation.PickupCollectRadiusWorld, distance);
            float speed = PickupPresentation.PickupMagnetSpeedWorldPerSecond * Mathf.Lerp(0.55f, 1.35f, strength);
            nextPosition = Vector3.MoveTowards(currentPosition, playerPosition, speed * dt);
            return true;
        }

        public static bool IsInCollectRange(Vector3 currentPosition, Vector3 playerPosition)
        {
            return Vector2.Distance(currentPosition, playerPosition) <= PickupPresentation.PickupCollectRadiusWorld;
        }
    }
}
