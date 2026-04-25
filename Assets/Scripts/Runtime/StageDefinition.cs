using UnityEngine;

namespace Wanwan.Runtime
{
    public class StageDefinition
    {
        public StageDefinition(
            int number,
            string name,
            string bossName,
            Color backgroundColor,
            Color accentColor,
            float difficultyMultiplier,
            float backgroundSpeedMultiplier,
            StageCombatStyle combatStyle,
            BossPatternStyle bossPattern,
            string victorySummary)
        {
            Number = number;
            Name = name;
            BossName = bossName;
            BackgroundColor = backgroundColor;
            AccentColor = accentColor;
            DifficultyMultiplier = difficultyMultiplier;
            BackgroundSpeedMultiplier = backgroundSpeedMultiplier;
            CombatStyle = combatStyle;
            BossPattern = bossPattern;
            VictorySummary = victorySummary;
        }

        public int Number { get; }
        public string Name { get; }
        public string BossName { get; }
        public Color BackgroundColor { get; }
        public Color AccentColor { get; }
        public float DifficultyMultiplier { get; }
        public float BackgroundSpeedMultiplier { get; }
        public StageCombatStyle CombatStyle { get; }
        public BossPatternStyle BossPattern { get; }
        public string VictorySummary { get; }
    }
}
