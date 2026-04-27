using UnityEngine;

namespace Wanwan.Runtime
{
    public static class StageCatalog
    {
        public const int StageCount = 8;

        private static readonly StageDefinition[] Stages =
        {
            new StageDefinition(1, "乡村", "乡村防卫旗舰", new Color(0.015f, 0.035f, 0.02f), new Color(0.36f, 1f, 0.46f), 1f, 1f, StageCombatStyle.Balanced, BossPatternStyle.Standard, "乡村低空航道恢复安全，地面防线重新建立。"),
            new StageDefinition(2, "城市", "城市防卫旗舰", new Color(0.01f, 0.02f, 0.08f), new Color(0.34f, 0.72f, 1f), 1.02f, 1.08f, StageCombatStyle.Flanking, BossPatternStyle.Crossfire, "城市空域恢复安全，地面防线重新建立。"),
            new StageDefinition(3, "海岸线", "海峡拦截舰", new Color(0.01f, 0.035f, 0.09f), new Color(0.2f, 0.92f, 1f), 1.16f, 0.92f, StageCombatStyle.Swarm, BossPatternStyle.Barrage, "海岸补给航线已打通，舰队获得前推进力。", StageWeather.Rain),
            new StageDefinition(4, "荒漠遗迹", "遗迹炮击舰", new Color(0.055f, 0.035f, 0.02f), new Color(1f, 0.76f, 0.24f), 1.24f, 1.24f, StageCombatStyle.Sniper, BossPatternStyle.Needle, "遗迹炮击阵地沉默，前线雷达重新上线。", StageWeather.Sand),
            new StageDefinition(5, "赤褐荒地", "峡谷压制舰", new Color(0.055f, 0.025f, 0.018f), new Color(1f, 0.38f, 0.28f), 1.34f, 1.04f, StageCombatStyle.Heavy, BossPatternStyle.Crusher, "荒地重装产线停摆，峡谷航线恢复通行。"),
            new StageDefinition(6, "浮空大陆", "浮岛截击舰", new Color(0.018f, 0.055f, 0.07f), new Color(0.58f, 1f, 0.86f), 1.44f, 1.36f, StageCombatStyle.Agile, BossPatternStyle.Drift, "浮空大陆拦截圈被撕开，瀑布航道已无敌机遮蔽。", StageWeather.Spores),
            new StageDefinition(7, "空间站", "轨道封锁舰", new Color(0.025f, 0.02f, 0.065f), new Color(0.72f, 0.46f, 1f), 1.56f, 1.18f, StageCombatStyle.Spiral, BossPatternStyle.Orbit, "轨道空间站封锁解除，深空航线开始展开。"),
            new StageDefinition(8, "外星基地", "Cranassian核心", new Color(0.045f, 0.01f, 0.06f), new Color(1f, 0.2f, 0.84f), 1.72f, 1.48f, StageCombatStyle.Finale, BossPatternStyle.Core, "Cranassian核心坍缩，整条战线迎来决定性胜利。", StageWeather.Spores)
        };

        private static readonly StageGameplayProfile[] GameplayProfiles =
        {
            new StageGameplayProfile(StageCombatStyle.Balanced, new[] { AmmoPowerupType.Scatter, AmmoPowerupType.RapidFire }, 6, 0.58f, 1f),
            new StageGameplayProfile(StageCombatStyle.Flanking, new[] { AmmoPowerupType.Wave, AmmoPowerupType.RapidFire }, 8, 0.62f, 1f),
            new StageGameplayProfile(StageCombatStyle.Swarm, new[] { AmmoPowerupType.Scatter, AmmoPowerupType.Homing }, 10, 0.66f, 0.85f),
            new StageGameplayProfile(StageCombatStyle.Sniper, new[] { AmmoPowerupType.Pierce, AmmoPowerupType.Laser }, 12, 0.58f, 1.32f),
            new StageGameplayProfile(StageCombatStyle.Heavy, new[] { AmmoPowerupType.Burst, AmmoPowerupType.Plasma }, 14, 0.6f, 1.45f),
            new StageGameplayProfile(StageCombatStyle.Agile, new[] { AmmoPowerupType.Wave, AmmoPowerupType.Homing }, 16, 0.64f, 0.95f),
            new StageGameplayProfile(StageCombatStyle.Spiral, new[] { AmmoPowerupType.Guard, AmmoPowerupType.Wave }, 18, 0.62f, 1.12f),
            new StageGameplayProfile(StageCombatStyle.Finale, new[] { AmmoPowerupType.Plasma, AmmoPowerupType.Laser, AmmoPowerupType.Guard }, 20, 0.72f, 1.5f)
        };

        public static StageDefinition GetStage(int stageIndex)
        {
            int safeIndex = Mathf.Clamp(stageIndex, 0, StageCount - 1);
            return Stages[safeIndex];
        }

        public static StageGameplayProfile GetGameplayProfile(int stageIndex)
        {
            int safeIndex = Mathf.Clamp(stageIndex, 0, StageCount - 1);
            return GameplayProfiles[safeIndex];
        }

        public static float GetLoopMultiplier(int loopIndex)
        {
            return 1f + (Mathf.Max(0, loopIndex) * 0.22f);
        }

        public static float GetDifficultyMultiplier(int stageIndex, int loopIndex)
        {
            return GetStage(stageIndex).DifficultyMultiplier * GetLoopMultiplier(loopIndex);
        }

        public static float GetStageDurationMultiplier(int stageIndex)
        {
            float[] multipliers = { 0.9f, 0.94f, 0.98f, 1.02f, 1.06f, 1.1f, 1.15f, 1.2f };
            return multipliers[Mathf.Clamp(stageIndex, 0, StageCount - 1)];
        }

        public static int GetNextStageIndex(int stageIndex)
        {
            return (stageIndex + 1) % StageCount;
        }

        public static bool IsLoopAdvance(int stageIndex)
        {
            return stageIndex >= StageCount - 1;
        }

        public static string BuildPreviewSummary(int stageIndex)
        {
            StageDefinition stage = GetStage(stageIndex);
            return $"第{stage.Number}关 {stage.Name}\n目标 {stage.BossName}\n战况 {BuildCombatStyleLabel(stage.CombatStyle)}";
        }

        public static string BuildCombatStyleLabel(StageCombatStyle combatStyle)
        {
            switch (combatStyle)
            {
                case StageCombatStyle.Flanking:
                    return "侧翼包抄";
                case StageCombatStyle.Swarm:
                    return "密集蜂群";
                case StageCombatStyle.Sniper:
                    return "远程狙击";
                case StageCombatStyle.Heavy:
                    return "重装压制";
                case StageCombatStyle.Agile:
                    return "高速截击";
                case StageCombatStyle.Spiral:
                    return "螺旋封锁";
                case StageCombatStyle.Finale:
                    return "最终核心";
                default:
                    return "均衡突袭";
            }
        }
    }
}
