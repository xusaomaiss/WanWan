namespace Wanwan.Runtime
{
    public static class StageClearTarget
    {
        public static int GetRequiredKills(GameDifficulty difficulty)
        {
            return GetRequiredKills(difficulty, 0, 0);
        }

        public static int GetRequiredKills(GameDifficulty difficulty, int stageIndex, int loopIndex)
        {
            int baseTarget;
            switch (difficulty)
            {
                case GameDifficulty.High:
                    baseTarget = 40;
                    break;
                case GameDifficulty.Medium:
                    baseTarget = 30;
                    break;
                default:
                    baseTarget = 20;
                    break;
            }

            int stageBonus = stageIndex * 2;
            int loopBonus = loopIndex * 8;
            return baseTarget + stageBonus + loopBonus;
        }
    }
}
