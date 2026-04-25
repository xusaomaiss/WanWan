using UnityEngine;

namespace Wanwan.Runtime
{
    public static class StageCatalog
    {
        public const int StageCount = 8;

        private static readonly StageDefinition[] Stages =
        {
            new StageDefinition(1, "城市上空", "城市防卫旗舰", new Color(0.01f, 0.02f, 0.08f), 1f),
            new StageDefinition(2, "海岸线", "海峡拦截舰", new Color(0.01f, 0.035f, 0.09f), 1.08f),
            new StageDefinition(3, "森林地带", "林区突击母舰", new Color(0.015f, 0.04f, 0.055f), 1.16f),
            new StageDefinition(4, "沙漠阵线", "沙暴炮击舰", new Color(0.055f, 0.035f, 0.02f), 1.24f),
            new StageDefinition(5, "工业区", "钢铁压制舰", new Color(0.045f, 0.025f, 0.045f), 1.34f),
            new StageDefinition(6, "高空云层", "高空截击舰", new Color(0.018f, 0.035f, 0.095f), 1.44f),
            new StageDefinition(7, "外太空入口", "轨道封锁舰", new Color(0.025f, 0.015f, 0.07f), 1.56f),
            new StageDefinition(8, "外星母舰", "Cranassian核心", new Color(0.045f, 0.01f, 0.06f), 1.72f)
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
