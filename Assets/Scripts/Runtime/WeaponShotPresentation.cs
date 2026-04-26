using UnityEngine;

namespace Wanwan.Runtime
{
    public static class WeaponShotPresentation
    {
        public static Vector3 GetPlayerScale(AmmoPowerupType type, bool canPierce)
        {
            return GetPlayerScale(type, canPierce, GetDefaultPlayerWidth(type));
        }

        public static Vector3 GetPlayerScale(WeaponType type, bool canPierce, float width)
        {
            return GetPlayerScale(PowerupCycle.ToAmmoPowerupType(type), canPierce, width);
        }

        public static Vector3 GetPlayerScale(AmmoPowerupType type, bool canPierce, float width)
        {
            float widthMultiplier = GetPlayerWidthMultiplier(type);
            float height = GetPlayerHeight(type, canPierce);
            return new Vector3(Mathf.Max(width * widthMultiplier, GetPlayerMinimumWidth(type)), height, 1f);
        }

        public static Vector2 GetPlayerColliderSize(AmmoPowerupType type, bool canPierce)
        {
            if (type == AmmoPowerupType.Laser)
            {
                return new Vector2(0.24f, 0.86f);
            }

            if (type == AmmoPowerupType.Burst || type == AmmoPowerupType.Plasma)
            {
                return new Vector2(0.28f, 0.62f);
            }

            return new Vector2(0.24f, canPierce ? 0.78f : 0.7f);
        }

        public static Vector2 GetPlayerColliderSize(WeaponType type, bool canPierce)
        {
            return GetPlayerColliderSize(PowerupCycle.ToAmmoPowerupType(type), canPierce);
        }

        public static Vector3 GetEnemyScale(bool fromBoss)
        {
            return fromBoss ? new Vector3(0.28f, 0.54f, 1f) : new Vector3(0.24f, 0.46f, 1f);
        }

        public static Vector2 GetEnemyColliderSize(bool fromBoss)
        {
            return fromBoss ? new Vector2(0.22f, 0.58f) : new Vector2(0.18f, 0.5f);
        }

        public static float GetLaserSideOffset()
        {
            return 0.36f;
        }

        public static float GetRapidSideOffset()
        {
            return 0.28f;
        }

        private static float GetDefaultPlayerWidth(AmmoPowerupType type)
        {
            switch (type)
            {
                case AmmoPowerupType.Laser:
                    return 0.14f;
                case AmmoPowerupType.RapidFire:
                    return 0.17f;
                case AmmoPowerupType.Scatter:
                    return 0.2f;
                case AmmoPowerupType.Plasma:
                    return 0.32f;
                case AmmoPowerupType.Burst:
                    return 0.36f;
                default:
                    return 0.2f;
            }
        }

        private static float GetPlayerWidthMultiplier(AmmoPowerupType type)
        {
            switch (type)
            {
                case AmmoPowerupType.Laser:
                    return 1.7f;
                case AmmoPowerupType.RapidFire:
                    return 1.65f;
                case AmmoPowerupType.Scatter:
                    return 1.5f;
                case AmmoPowerupType.Plasma:
                case AmmoPowerupType.Burst:
                    return 1.25f;
                default:
                    return 1.45f;
            }
        }

        private static float GetPlayerMinimumWidth(AmmoPowerupType type)
        {
            switch (type)
            {
                case AmmoPowerupType.Laser:
                    return 0.2f;
                case AmmoPowerupType.RapidFire:
                    return 0.24f;
                case AmmoPowerupType.Plasma:
                case AmmoPowerupType.Burst:
                    return 0.36f;
                default:
                    return 0.26f;
            }
        }

        private static float GetPlayerHeight(AmmoPowerupType type, bool canPierce)
        {
            switch (type)
            {
                case AmmoPowerupType.Laser:
                    return canPierce ? 0.86f : 0.74f;
                case AmmoPowerupType.RapidFire:
                    return 0.48f;
                case AmmoPowerupType.Plasma:
                case AmmoPowerupType.Burst:
                    return 0.5f;
                default:
                    return canPierce ? 0.68f : 0.48f;
            }
        }
    }
}
