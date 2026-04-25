namespace Wanwan.Runtime
{
    public static class StageClearTarget
    {
        public static int GetRequiredKills(GameDifficulty difficulty)
        {
            switch (difficulty)
            {
                case GameDifficulty.High:
                    return 40;
                case GameDifficulty.Medium:
                    return 30;
                default:
                    return 20;
            }
        }
    }
}
