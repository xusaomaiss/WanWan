using UnityEngine;

namespace Wanwan.Runtime
{
    public static class StageCatalog
    {
        public const int StageCount = 8;

        private static readonly StageDefinition[] Stages =
        {
            new StageDefinition(1, "城市上空", "城市防卫旗舰", new Color(0.01f, 0.02f, 0.08f), new Color(0.34f, 0.72f, 1f), 1f, 1f, StageCombatStyle.Balanced, BossPatternStyle.Standard, "城市空域恢复安全，地面防线重新建立。"),
            new StageDefinition(2, "海岸线", "海峡拦截舰", new Color(0.01f, 0.035f, 0.09f), new Color(0.2f, 0.92f, 1f), 1.08f, 1.12f, StageCombatStyle.Flanking, BossPatternStyle.Crossfire, "海岸补给航线已打通，舰队获得前推进力。"),
            new StageDefinition(3, "森林地带", "林区突击母舰", new Color(0.015f, 0.04f, 0.055f), new Color(0.36f, 1f, 0.46f), 1.16f, 0.92f, StageCombatStyle.Swarm, BossPatternStyle.Barrage, "林区伏击网被摧毁，低空航道重新变得清晰。"),
            new StageDefinition(4, "沙漠阵线", "沙暴炮击舰", new Color(0.055f, 0.035f, 0.02f), new Color(1f, 0.76f, 0.24f), 1.24f, 1.24f, StageCombatStyle.Sniper, BossPatternStyle.Needle, "沙漠炮击阵地沉默，前线雷达重新上线。"),
            new StageDefinition(5, "工业区", "钢铁压制舰", new Color(0.045f, 0.025f, 0.045f), new Color(1f, 0.38f, 0.58f), 1.34f, 1.04f, StageCombatStyle.Heavy, BossPatternStyle.Crusher, "工业区核心炉心稳定，敌方重装产线停摆。"),
            new StageDefinition(6, "高空云层", "高空截击舰", new Color(0.018f, 0.035f, 0.095f), new Color(0.76f, 0.9f, 1f), 1.44f, 1.36f, StageCombatStyle.Agile, BossPatternStyle.Drift, "高空拦截圈被撕开，云层之上已无敌机遮蔽。"),
            new StageDefinition(7, "外太空入口", "轨道封锁舰", new Color(0.025f, 0.015f, 0.07f), new Color(0.72f, 0.46f, 1f), 1.56f, 1.18f, StageCombatStyle.Spiral, BossPatternStyle.Orbit, "轨道封锁解除，深空航线开始展开。"),
            new StageDefinition(8, "外星母舰", "Cranassian核心", new Color(0.045f, 0.01f, 0.06f), new Color(1f, 0.2f, 0.84f), 1.72f, 1.48f, StageCombatStyle.Finale, BossPatternStyle.Core, "Cranassian核心坍缩，整条战线迎来决定性胜利。")
        };

        public static StageDefinition GetStage(int stageIndex)
        {
            int safeIndex = Mathf.Clamp(stageIndex, 0, StageCount - 1);
            return Stages[safeIndex];
        }

        public static float GetLoopMultiplier(int loopIndex)
        {
            return 1f + (Mathf.Max(0, loopIndex) * 0.22f);
        }

        public static float GetDifficultyMultiplier(int stageIndex, int loopIndex)
        {
            return GetStage(stageIndex).DifficultyMultiplier * GetLoopMultiplier(loopIndex);
        }

        public static int GetNextStageIndex(int stageIndex)
        {
            return (stageIndex + 1) % StageCount;
        }

        public static bool IsLoopAdvance(int stageIndex)
        {
            return stageIndex >= StageCount - 1;
        }
    }
}
