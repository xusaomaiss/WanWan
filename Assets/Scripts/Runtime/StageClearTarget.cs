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
                    baseTarget = 90;
                    break;
                case GameDifficulty.Medium:
                    baseTarget = 82;
                    break;
                default:
                    baseTarget = 75;
                    break;
            }

            int stageBonus = stageIndex * 2;
            int loopBonus = loopIndex * 12;
            return baseTarget + stageBonus + loopBonus;
        }
    }
}
