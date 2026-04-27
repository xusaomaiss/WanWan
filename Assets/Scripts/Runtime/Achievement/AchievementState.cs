using System;
using System.Collections.Generic;
using UnityEngine;

namespace Wanwan.Runtime.Achievement
{
    public class AchievementState
    {
        private readonly HashSet<AchievementType> unlocked = new HashSet<AchievementType>();
        private readonly Dictionary<AchievementType, int> progressCounters = new Dictionary<AchievementType, int>();

        public void Load()
        {
            foreach (AchievementType type in Enum.GetValues(typeof(AchievementType)))
            {
                string key = $"wanwan.achievement.{type}";
                if (PlayerPrefs.HasKey(key))
                {
                    unlocked.Add(type);
                }
                string progressKey = $"wanwan.achievement.progress.{type}";
                if (PlayerPrefs.HasKey(progressKey))
                {
                    progressCounters[type] = PlayerPrefs.GetInt(progressKey);
                }
            }
        }

        public void Save()
        {
            PlayerPrefs.Save();
        }

        public void Unlock(AchievementType type)
        {
            if (unlocked.Contains(type))
                return;
            unlocked.Add(type);
            PlayerPrefs.SetInt($"wanwan.achievement.{type}", 1);
            PlayerPrefs.Save();
        }

        public bool IsUnlocked(AchievementType type) => unlocked.Contains(type);

        public void AddProgress(AchievementType type, int amount)
        {
            if (unlocked.Contains(type))
                return;
            progressCounters[type] = (progressCounters.ContainsKey(type) ? progressCounters[type] : 0) + amount;
            CheckProgress(type);
        }

        private void CheckProgress(AchievementType type)
        {
            int progress = progressCounters.ContainsKey(type) ? progressCounters[type] : 0;
            bool shouldUnlock = type switch
            {
                AchievementType.FirstClear => progress >= 1,
                AchievementType.MaxCombo100 => progress >= 100,
                AchievementType.MaxCombo50 => progress >= 50,
                AchievementType.Collect500Coins => progress >= 500,
                AchievementType.Collect200Coins => progress >= 200,
                _ => false
            };
            if (shouldUnlock)
            {
                Unlock(type);
            }
        }
    }
}
