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

        public MountConfig(MountType type, int cost, string displayName, string description, Color accentColor, float fireInterval)
        {
            Type = type;
            Cost = Mathf.Max(0, cost);
            DisplayName = displayName;
            Description = description;
            AccentColor = accentColor;
            FireInterval = Mathf.Max(0.1f, fireInterval);
        }

        public MountType Type { get; }
        public int Cost { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public Color AccentColor { get; }
        public float FireInterval { get; }

        public static MountConfig Get(MountType type)
        {
            switch (type)
            {
                case MountType.MissilePod:
                    return new MountConfig(type, 30000, "导弹舱", "低频追踪导弹", new Color(1f, 0.62f, 0.24f), 0.82f);
                case MountType.DefenseDrone:
                    return new MountConfig(type, 50000, "防卫机", "双侧辅助火力", new Color(0.38f, 0.9f, 1f), 0.46f);
                case MountType.ShieldEmitter:
                    return new MountConfig(type, 70000, "护盾发生器", "下一关额外护盾", new Color(0.42f, 1f, 0.62f), 1f);
                default:
                    return new MountConfig(MountType.None, 0, "无挂载", "不装备临时挂载", Color.gray, 1f);
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
