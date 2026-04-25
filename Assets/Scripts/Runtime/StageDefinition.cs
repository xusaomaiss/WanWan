using UnityEngine;

namespace Wanwan.Runtime
{
    public class StageDefinition
    {
        public StageDefinition(int number, string name, string bossName, Color backgroundColor, float difficultyMultiplier)
        {
            Number = number;
            Name = name;
            BossName = bossName;
            BackgroundColor = backgroundColor;
            DifficultyMultiplier = difficultyMultiplier;
        }

        public int Number { get; }
        public string Name { get; }
        public string BossName { get; }
        public Color BackgroundColor { get; }
        public float DifficultyMultiplier { get; }
    }
}
