namespace Wanwan.Runtime
{
    public readonly struct LeaderboardEntry
    {
        public LeaderboardEntry(string name, int score, GameDifficulty difficulty, int stageIndex, int loopIndex)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "AAA" : name.ToUpperInvariant();
            Score = score;
            Difficulty = difficulty;
            StageIndex = stageIndex;
            LoopIndex = loopIndex;
        }

        public string Name { get; }
        public int Score { get; }
        public GameDifficulty Difficulty { get; }
        public int StageIndex { get; }
        public int LoopIndex { get; }
        public int StageNumber => StageIndex + 1;
        public int LoopNumber => LoopIndex + 1;
    }
}
