namespace Wanwan.Runtime.Achievement
{
    public enum AchievementType
    {
        FirstClear,
        PerfectStage,
        MaxCombo100,
        Collect500Coins,
        FirstBossDefeat,
        HardClear,
        MaxCombo50,
        Collect200Coins,
        Loop3Clear,
        NoBombClear
    }

    public static class AchievementExtensions
    {
        public static string GetDisplayName(this AchievementType type)
        {
            switch (type)
            {
                case AchievementType.FirstClear:
                    return "初次通关";
                case AchievementType.PerfectStage:
                    return "无伤通关";
                case AchievementType.MaxCombo100:
                    return "百连击";
                case AchievementType.Collect500Coins:
                    return "金币大亨";
                case AchievementType.FirstBossDefeat:
                    return "首杀旗舰";
                case AchievementType.HardClear:
                    return "困难通关";
                case AchievementType.MaxCombo50:
                    return "五十连击";
                case AchievementType.Collect200Coins:
                    return "金币收集者";
                case AchievementType.Loop3Clear:
                    return "三轮通关";
                case AchievementType.NoBombClear:
                    return "无弹通关";
                default:
                    return string.Empty;
            }
        }
    }
}
