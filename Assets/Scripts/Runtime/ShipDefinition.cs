using UnityEngine;

namespace Wanwan.Runtime
{
    public readonly struct ShipDefinition
    {
        public ShipDefinition(PlayerShipType type, string displayName, string mainWeapon, string subWeapon, int speedStars, int powerStars, Color accentColor, PlayerSkill skill = PlayerSkill.None)
        {
            Type = type;
            DisplayName = displayName;
            MainWeapon = mainWeapon;
            SubWeapon = subWeapon;
            SpeedStars = speedStars;
            PowerStars = powerStars;
            AccentColor = accentColor;
            Skill = skill;
        }

        public PlayerShipType Type { get; }
        public string DisplayName { get; }
        public string MainWeapon { get; }
        public string SubWeapon { get; }
        public int SpeedStars { get; }
        public int PowerStars { get; }
        public Color AccentColor { get; }
        public PlayerSkill Skill { get; }

        public static ShipDefinition Get(PlayerShipType type)
        {
            if (type == PlayerShipType.Blue)
            {
                return new ShipDefinition(type, "蓝色战机", "激光弹", "追踪导弹", 4, 3, ArcadeTheme.ElectricBlue, PlayerSkill.BetterGraze);
            }

            return new ShipDefinition(PlayerShipType.Green, "绿色战机", "散射炮", "追踪导弹", 3, 4, ArcadeTheme.MilitaryGreen, PlayerSkill.ExtraShield);
        }
    }
}
