using UnityEngine;

namespace Wanwan.Runtime
{
    public static class SessionState
    {
        private const string HighScoreKey = "wanwan.high_score";
        private const string AudioEnabledKey = "wanwan.audio_enabled";
        private const string MusicVolumeKey = "wanwan.music_volume";
        private const string SfxVolumeKey = "wanwan.sfx_volume";
        private const string SelectedShipKey = "wanwan.selected_ship";
        private const string SelectedDifficultyKey = "wanwan.selected_difficulty";
        private const string ControlSensitivityKey = "wanwan.control_sensitivity";
        private const string VibrationEnabledKey = "wanwan.vibration_enabled";
        private const string DamageNumbersEnabledKey = "wanwan.damage_numbers_enabled";
        private const string VirtualButtonOpacityKey = "wanwan.virtual_button_opacity";
        private const string VisualEffectsQualityKey = "wanwan.visual_effects_quality";
        private const string LeaderboardKey = "wanwan.leaderboard";
        private const string LeaderboardNameKey = "wanwan.leaderboard_name";
        private const string SpendableScoreKey = "wanwan.spendable_score";
        private const string PendingMountKey = "wanwan.pending_mount";

        public static int LastScore { get; private set; }
        public static int HighScore => PlayerPrefs.GetInt(HighScoreKey, 0);
        public static bool AudioEnabled => PlayerPrefs.GetInt(AudioEnabledKey, 1) == 1;
        public static float MusicVolume => PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        public static float SfxVolume => PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
        public static PlayerShipType SelectedShip => (PlayerShipType)PlayerPrefs.GetInt(SelectedShipKey, (int)PlayerShipType.Green);
        public static GameDifficulty SelectedDifficulty => (GameDifficulty)PlayerPrefs.GetInt(SelectedDifficultyKey, (int)GameDifficulty.Medium);
        public static ControlSensitivity ControlSensitivity => (ControlSensitivity)PlayerPrefs.GetInt(ControlSensitivityKey, (int)ControlSensitivity.Medium);
        public static bool VibrationEnabled => PlayerPrefs.GetInt(VibrationEnabledKey, 1) == 1;
        public static bool DamageNumbersEnabled => PlayerPrefs.GetInt(DamageNumbersEnabledKey, 1) == 1;
        public static float VirtualButtonOpacity => PlayerPrefs.GetFloat(VirtualButtonOpacityKey, 0.4f);
        public static VisualEffectsQuality VisualEffectsQuality => (VisualEffectsQuality)PlayerPrefs.GetInt(VisualEffectsQualityKey, (int)VisualEffectsQuality.Full);
        public static string LeaderboardName => SanitizeName(PlayerPrefs.GetString(LeaderboardNameKey, "AAA"));
        public static int SpendableScore => PlayerPrefs.GetInt(SpendableScoreKey, 0);
        public static MountType PendingMount => (MountType)PlayerPrefs.GetInt(PendingMountKey, (int)MountType.None);
        public static bool LastRunWasVictory { get; private set; }
        public static string LastRunRating { get; private set; } = "乙";
        public static string LastRunSummary { get; private set; } = string.Empty;
        public static int LastRunMaxCombo { get; private set; }
        public static int LastRunMaxMultiplier { get; private set; } = 1;
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
            LastRunRating = "乙";
            LastRunSummary = string.Empty;
            LastRunMaxCombo = 0;
            LastRunMaxMultiplier = 1;
        }

        public static void SelectDifficulty(GameDifficulty difficulty)
        {
            PlayerPrefs.SetInt(SelectedDifficultyKey, (int)difficulty);
            PlayerPrefs.Save();
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
            LastRunWasVictory = false;
            LastRunRating = "乙";
            LastRunSummary = string.Empty;
            LastRunDifficulty = GameDifficulty.Low;
            LastRunMaxCombo = 0;
            LastRunMaxMultiplier = 1;
            ResetCampaign();
            PlayerPrefs.DeleteKey(HighScoreKey);
            PlayerPrefs.DeleteKey(AudioEnabledKey);
            PlayerPrefs.DeleteKey(MusicVolumeKey);
            PlayerPrefs.DeleteKey(SfxVolumeKey);
            PlayerPrefs.DeleteKey(SelectedShipKey);
            PlayerPrefs.DeleteKey(SelectedDifficultyKey);
            PlayerPrefs.DeleteKey(ControlSensitivityKey);
            PlayerPrefs.DeleteKey(VibrationEnabledKey);
            PlayerPrefs.DeleteKey(DamageNumbersEnabledKey);
            PlayerPrefs.DeleteKey(VirtualButtonOpacityKey);
            PlayerPrefs.DeleteKey(VisualEffectsQualityKey);
            PlayerPrefs.DeleteKey(LeaderboardKey);
            PlayerPrefs.DeleteKey(LeaderboardNameKey);
            PlayerPrefs.DeleteKey(SpendableScoreKey);
            PlayerPrefs.DeleteKey(PendingMountKey);
            PlayerPrefs.Save();
        }

        public static void ResetSettings()
        {
            PlayerPrefs.DeleteKey(AudioEnabledKey);
            PlayerPrefs.DeleteKey(MusicVolumeKey);
            PlayerPrefs.DeleteKey(SfxVolumeKey);
            PlayerPrefs.DeleteKey(SelectedShipKey);
            PlayerPrefs.DeleteKey(SelectedDifficultyKey);
            PlayerPrefs.DeleteKey(ControlSensitivityKey);
            PlayerPrefs.DeleteKey(VibrationEnabledKey);
            PlayerPrefs.DeleteKey(DamageNumbersEnabledKey);
            PlayerPrefs.DeleteKey(VirtualButtonOpacityKey);
            PlayerPrefs.DeleteKey(VisualEffectsQualityKey);
            PlayerPrefs.DeleteKey(LeaderboardNameKey);
            PlayerPrefs.Save();
        }

        public static void AddSpendableScore(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            PlayerPrefs.SetInt(SpendableScoreKey, SpendableScore + amount);
            PlayerPrefs.Save();
        }

        public static bool TryPurchaseMount(MountType type)
        {
            MountConfig config = MountConfig.Get(type);
            if (type == MountType.None || config.Cost <= 0 || PendingMount != MountType.None || SpendableScore < config.Cost)
            {
                return false;
            }

            PlayerPrefs.SetInt(SpendableScoreKey, SpendableScore - config.Cost);
            PlayerPrefs.SetInt(PendingMountKey, (int)type);
            PlayerPrefs.Save();
            return true;
        }

        public static MountType ConsumePendingMountForRun()
        {
            MountType mount = PendingMount;
            if (mount != MountType.None)
            {
                PlayerPrefs.SetInt(PendingMountKey, (int)MountType.None);
                PlayerPrefs.Save();
            }

            return mount;
        }

        public static void SetAudioEnabled(bool enabled)
        {
            PlayerPrefs.SetInt(AudioEnabledKey, enabled ? 1 : 0);
            PlayerPrefs.Save();
        }

        public static void SetMusicVolume(float volume)
        {
            PlayerPrefs.SetFloat(MusicVolumeKey, Mathf.Clamp01(volume));
            PlayerPrefs.Save();
        }

        public static void SetSfxVolume(float volume)
        {
            PlayerPrefs.SetFloat(SfxVolumeKey, Mathf.Clamp01(volume));
            PlayerPrefs.Save();
        }

        public static void SelectShip(PlayerShipType shipType)
        {
            PlayerPrefs.SetInt(SelectedShipKey, (int)shipType);
            PlayerPrefs.Save();
        }

        public static void SetControlSensitivity(ControlSensitivity sensitivity)
        {
            PlayerPrefs.SetInt(ControlSensitivityKey, (int)sensitivity);
            PlayerPrefs.Save();
        }

        public static void SetVibrationEnabled(bool enabled)
        {
            PlayerPrefs.SetInt(VibrationEnabledKey, enabled ? 1 : 0);
            PlayerPrefs.Save();
        }

        public static void SetDamageNumbersEnabled(bool enabled)
        {
            PlayerPrefs.SetInt(DamageNumbersEnabledKey, enabled ? 1 : 0);
            PlayerPrefs.Save();
        }

        public static void SetVirtualButtonOpacity(float opacity)
        {
            PlayerPrefs.SetFloat(VirtualButtonOpacityKey, Mathf.Clamp01(opacity));
            PlayerPrefs.Save();
        }

        public static void SetVisualEffectsQuality(VisualEffectsQuality quality)
        {
            PlayerPrefs.SetInt(VisualEffectsQualityKey, (int)quality);
            PlayerPrefs.Save();
        }

        public static void SetLeaderboardName(string name)
        {
            PlayerPrefs.SetString(LeaderboardNameKey, SanitizeName(name));
            PlayerPrefs.Save();
        }

        public static void CommitRunScore(int score, bool victory = false, string rating = "乙", string summary = "", int maxCombo = 0, int maxMultiplier = 1)
        {
            LastScore = score;
            LastRunWasVictory = victory;
            LastRunRating = rating;
            LastRunSummary = summary;
            LastRunMaxCombo = Mathf.Max(0, maxCombo);
            LastRunMaxMultiplier = Mathf.Max(1, maxMultiplier);
            LastRunDifficulty = SelectedDifficulty;
            RecordLeaderboardScore(LeaderboardName, score, SelectedDifficulty, CurrentStageIndex, CurrentLoopIndex);
            if (victory)
            {
                AddSpendableScore(score);
            }

            if (score > HighScore)
            {
                PlayerPrefs.SetInt(HighScoreKey, score);
                PlayerPrefs.Save();
            }
        }

        public static void RecordLeaderboardScore(string name, int score, GameDifficulty difficulty, int stageIndex, int loopIndex)
        {
            LeaderboardEntry[] current = GetLeaderboardEntries();
            LeaderboardEntry[] expanded = new LeaderboardEntry[current.Length + 1];
            for (int i = 0; i < current.Length; i++)
            {
                expanded[i] = current[i];
            }

            expanded[current.Length] = new LeaderboardEntry(SanitizeName(name), Mathf.Max(0, score), difficulty, Mathf.Max(0, stageIndex), Mathf.Max(0, loopIndex));
            System.Array.Sort(expanded, (left, right) => right.Score.CompareTo(left.Score));

            int count = Mathf.Min(10, expanded.Length);
            LeaderboardEntry[] trimmed = new LeaderboardEntry[count];
            for (int i = 0; i < count; i++)
            {
                trimmed[i] = expanded[i];
            }

            PlayerPrefs.SetString(LeaderboardKey, SerializeLeaderboard(trimmed));
            PlayerPrefs.Save();
        }

        public static LeaderboardEntry[] GetLeaderboardEntries()
        {
            return GetLeaderboardEntries(null);
        }

        public static LeaderboardEntry[] GetLeaderboardEntries(GameDifficulty? difficulty)
        {
            string raw = PlayerPrefs.GetString(LeaderboardKey, string.Empty);
            if (string.IsNullOrEmpty(raw))
            {
                return new LeaderboardEntry[0];
            }

            string[] rows = raw.Split('\n');
            System.Collections.Generic.List<LeaderboardEntry> entries = new System.Collections.Generic.List<LeaderboardEntry>();
            foreach (string row in rows)
            {
                string[] parts = row.Split('|');
                if (parts.Length != 5)
                {
                    continue;
                }

                if (!int.TryParse(parts[1], out int score) ||
                    !int.TryParse(parts[2], out int difficultyValue) ||
                    !int.TryParse(parts[3], out int stageIndex) ||
                    !int.TryParse(parts[4], out int loopIndex))
                {
                    continue;
                }

                GameDifficulty parsedDifficulty = (GameDifficulty)difficultyValue;
                if (difficulty.HasValue && parsedDifficulty != difficulty.Value)
                {
                    continue;
                }

                entries.Add(new LeaderboardEntry(parts[0], score, parsedDifficulty, stageIndex, loopIndex));
            }

            entries.Sort((left, right) => right.Score.CompareTo(left.Score));
            if (entries.Count > 10)
            {
                entries.RemoveRange(10, entries.Count - 10);
            }

            return entries.ToArray();
        }

        public static void UpdateLastLeaderboardName(string name)
        {
            string safeName = SanitizeName(name);
            SetLeaderboardName(safeName);

            LeaderboardEntry[] current = GetLeaderboardEntries();
            for (int i = 0; i < current.Length; i++)
            {
                LeaderboardEntry entry = current[i];
                if (entry.Score == LastScore &&
                    entry.Difficulty == LastRunDifficulty &&
                    entry.StageIndex == CurrentStageIndex &&
                    entry.LoopIndex == CurrentLoopIndex)
                {
                    current[i] = new LeaderboardEntry(safeName, entry.Score, entry.Difficulty, entry.StageIndex, entry.LoopIndex);
                    PlayerPrefs.SetString(LeaderboardKey, SerializeLeaderboard(current));
                    PlayerPrefs.Save();
                    return;
                }
            }
        }

        public static void ResetLeaderboard()
        {
            PlayerPrefs.DeleteKey(LeaderboardKey);
            PlayerPrefs.Save();
        }

        private static string SerializeLeaderboard(LeaderboardEntry[] entries)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            for (int i = 0; i < entries.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append('\n');
                }

                LeaderboardEntry entry = entries[i];
                builder.Append(SanitizeName(entry.Name));
                builder.Append('|');
                builder.Append(entry.Score);
                builder.Append('|');
                builder.Append((int)entry.Difficulty);
                builder.Append('|');
                builder.Append(entry.StageIndex);
                builder.Append('|');
                builder.Append(entry.LoopIndex);
            }

            return builder.ToString();
        }

        private static string SanitizeName(string name)
        {
            string safe = string.IsNullOrWhiteSpace(name) ? "AAA" : name.Trim().ToUpperInvariant();
            safe = safe.Replace("|", string.Empty).Replace("\n", string.Empty);
            if (string.IsNullOrEmpty(safe))
            {
                return "AAA";
            }

            return safe.Length > 3 ? safe.Substring(0, 3) : safe;
        }
    }
}
