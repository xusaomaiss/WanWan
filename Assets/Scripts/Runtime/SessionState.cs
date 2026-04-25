using UnityEngine;

namespace Wanwan.Runtime
{
    public static class SessionState
    {
        private const string HighScoreKey = "wanwan.high_score";

        public static int LastScore { get; private set; }
        public static int HighScore => PlayerPrefs.GetInt(HighScoreKey, 0);
        public static GameDifficulty SelectedDifficulty { get; private set; } = GameDifficulty.Low;
        public static bool LastRunWasVictory { get; private set; }
        public static string LastRunRating { get; private set; } = "B";
        public static string LastRunSummary { get; private set; } = string.Empty;
        public static GameDifficulty LastRunDifficulty { get; private set; } = GameDifficulty.Low;
        public static int CurrentStageIndex { get; private set; }
        public static int CurrentLoopIndex { get; private set; }
        public static StageDefinition CurrentStage => StageCatalog.GetStage(CurrentStageIndex);
        public static int CurrentStageNumber => CurrentStage.Number;
        public static int CurrentLoopNumber => CurrentLoopIndex + 1;
        public static float CurrentStageDifficultyMultiplier => StageCatalog.GetDifficultyMultiplier(CurrentStageIndex, CurrentLoopIndex);

        public static void ResetRun()
        {
            LastScore = 0;
            LastRunWasVictory = false;
            LastRunRating = "B";
            LastRunSummary = string.Empty;
        }

        public static void SelectDifficulty(GameDifficulty difficulty)
        {
            SelectedDifficulty = difficulty;
            ResetCampaign();
        }

        public static void ResetCampaign()
        {
            CurrentStageIndex = 0;
            CurrentLoopIndex = 0;
        }

        public static void AdvanceStage()
        {
            if (StageCatalog.IsLoopAdvance(CurrentStageIndex))
            {
                CurrentLoopIndex++;
            }

            CurrentStageIndex = StageCatalog.GetNextStageIndex(CurrentStageIndex);
        }

        public static void ResetProgress()
        {
            LastScore = 0;
            SelectedDifficulty = GameDifficulty.Low;
            LastRunWasVictory = false;
            LastRunRating = "B";
            LastRunSummary = string.Empty;
            LastRunDifficulty = GameDifficulty.Low;
            ResetCampaign();
            PlayerPrefs.DeleteKey(HighScoreKey);
            PlayerPrefs.Save();
        }

        public static void CommitRunScore(int score, bool victory = false, string rating = "B", string summary = "")
        {
            LastScore = score;
            LastRunWasVictory = victory;
            LastRunRating = rating;
            LastRunSummary = summary;
            LastRunDifficulty = SelectedDifficulty;
            if (score > HighScore)
            {
                PlayerPrefs.SetInt(HighScoreKey, score);
                PlayerPrefs.Save();
            }
        }
    }
}
