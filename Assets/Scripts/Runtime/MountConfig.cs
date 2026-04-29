using UnityEngine;

namespace Wanwan.Runtime
{
    public readonly struct MountConfig
    {
        private static readonly MountType[] PlayableMounts =
        {
            MountType.MissilePod,
            MountType.DefenseDrone,
            MountType.ShieldEmitter
        };

        public MountConfig(MountType type, int cost, string displayName, string description, Color accentColor, float fireInterval, int purchaseUnits, string unitLabel)
        {
            Type = type;
            Cost = Mathf.Max(0, cost);
            DisplayName = displayName;
            Description = description;
            AccentColor = accentColor;
            FireInterval = Mathf.Max(0.1f, fireInterval);
            PurchaseUnits = Mathf.Max(0, purchaseUnits);
            UnitLabel = unitLabel;
        }

        public MountType Type { get; }
        public int Cost { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public Color AccentColor { get; }
        public float FireInterval { get; }
        public int PurchaseUnits { get; }
        public string UnitLabel { get; }

        public static MountConfig Get(MountType type)
        {
            switch (type)
            {
                case MountType.MissilePod:
                    return new MountConfig(type, 20000, "导弹舱", "追踪爆破导弹", new Color(1f, 0.62f, 0.24f), 0.68f, 16, "发");
                case MountType.DefenseDrone:
                    return new MountConfig(type, 28000, "防卫机", "双侧辅助火力", new Color(0.38f, 0.9f, 1f), 0.34f, 40, "发");
                case MountType.ShieldEmitter:
                    return new MountConfig(type, 32000, "护盾发生器", "每层抵消一次受击", new Color(0.42f, 1f, 0.62f), 1f, 2, "层");
                default:
                    return new MountConfig(MountType.None, 0, "无挂载", "不装备临时挂载", Color.gray, 1f, 0, string.Empty);
            }
        }

        public static MountType[] GetPlayableMounts()
        {
            MountType[] result = new MountType[PlayableMounts.Length];
            System.Array.Copy(PlayableMounts, result, PlayableMounts.Length);
            return result;
        }
    }
}
