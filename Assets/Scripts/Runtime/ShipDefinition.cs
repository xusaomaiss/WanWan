using UnityEngine;

namespace Wanwan.Runtime
{
    public readonly struct ShipDefinition
    {
        public ShipDefinition(PlayerShipType type, string displayName, string mainWeapon, string subWeapon, int speedStars, int powerStars, Color accentColor, PlayerSkill skill = PlayerSkill.None)
            : this(type, displayName, mainWeapon, subWeapon, speedStars, powerStars, powerStars, 3, accentColor, skill)
        {
        }

        public ShipDefinition(PlayerShipType type, string displayName, string mainWeapon, string subWeapon, int speedStars, int powerStars, int attackStars, int defenseStars, Color accentColor, PlayerSkill skill = PlayerSkill.None)
        {
            Type = type;
            DisplayName = displayName;
            MainWeapon = mainWeapon;
            SubWeapon = subWeapon;
            SpeedStars = speedStars;
            PowerStars = powerStars;
            AttackStars = attackStars;
            DefenseStars = defenseStars;
            AccentColor = accentColor;
            Skill = skill;
        }

        public PlayerShipType Type { get; }
        public string DisplayName { get; }
        public string MainWeapon { get; }
        public string SubWeapon { get; }
        public int SpeedStars { get; }
        public int PowerStars { get; }
        public int AttackStars { get; }
        public int DefenseStars { get; }
        public Color AccentColor { get; }
        public PlayerSkill Skill { get; }

        public static ShipDefinition Get(PlayerShipType type)
        {
            switch (type)
            {
                case PlayerShipType.Blue:
                    return new ShipDefinition(type, "BLUE A1", "FIRE ATTACK", "追踪导弹", 3, 4, 4, 5, ArcadeTheme.ElectricBlue, PlayerSkill.BetterGraze);
                case PlayerShipType.Yellow:
                    return new ShipDefinition(type, "YELLOW", "FIRE ATTACK", "聚能火箭", 4, 3, 4, 3, ArcadeTheme.EnergyYellow, PlayerSkill.Level2Weapon);
                case PlayerShipType.Purple:
                    return new ShipDefinition(type, "PURPLE", "FIRE ATTACK", "脉冲护航", 3, 2, 4, 5, new Color(0.72f, 0.24f, 1f), PlayerSkill.BetterGraze);
                case PlayerShipType.Azure:
                    return new ShipDefinition(type, "BLUE", "FIRE ATTACK", "均衡挂载", 3, 1, 4, 5, new Color(0.12f, 0.5f, 1f), PlayerSkill.None);
                case PlayerShipType.Green:
                default:
                    return new ShipDefinition(PlayerShipType.Green, "RED JET", "FIRE ATTACK", "追踪导弹", 3, 5, 4, 5, ArcadeTheme.WarningRed, PlayerSkill.ExtraShield);
            }
        }
    }
}
