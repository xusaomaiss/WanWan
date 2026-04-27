using NUnit.Framework;
using UnityEngine;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class SessionStateTests
    {
        [SetUp]
        public void SetUp()
        {
            SessionState.ResetProgress();
        }

        [Test]
        public void CommitRunScore_StoresLastScore()
        {
            SessionState.CommitRunScore(1250);

            Assert.That(SessionState.LastScore, Is.EqualTo(1250));
            Assert.That(SessionState.SpendableScore, Is.EqualTo(0));
        }

        [Test]
        public void CommitRunScore_AddsVictoryScoreToSpendableScoreOnly()
        {
            SessionState.CommitRunScore(1200, true);
            SessionState.CommitRunScore(900, false);

            Assert.That(SessionState.SpendableScore, Is.EqualTo(1200));
        }

        [Test]
        public void CommitRunScore_KeepsHighestScore()
        {
            SessionState.CommitRunScore(800);
            SessionState.CommitRunScore(600);
            SessionState.CommitRunScore(1200);

            Assert.That(SessionState.HighScore, Is.EqualTo(1200));
        }

        [Test]
        public void HighScore_PersistsAcrossRunReset()
        {
            SessionState.CommitRunScore(900);

            SessionState.ResetRun();

            Assert.That(SessionState.LastScore, Is.EqualTo(0));
            Assert.That(SessionState.HighScore, Is.EqualTo(900));
            Assert.That(PlayerPrefs.GetInt("wanwan.high_score", 0), Is.EqualTo(900));
        }

        [Test]
        public void SelectDifficulty_StoresCurrentDifficulty()
        {
            SessionState.SelectDifficulty(GameDifficulty.High);

            Assert.That(SessionState.SelectedDifficulty, Is.EqualTo(GameDifficulty.High));
        }

        [Test]
        public void DefaultDifficulty_IsMediumForOneTapStart()
        {
            Assert.That(SessionState.SelectedDifficulty, Is.EqualTo(GameDifficulty.Medium));
        }

        [Test]
        public void SelectDifficulty_PersistsAsDefaultStartPreference()
        {
            SessionState.SelectDifficulty(GameDifficulty.High);
            SessionState.ResetRun();

            Assert.That(SessionState.SelectedDifficulty, Is.EqualTo(GameDifficulty.High));
            Assert.That(PlayerPrefs.GetInt("wanwan.selected_difficulty", -1), Is.EqualTo((int)GameDifficulty.High));
        }

        [Test]
        public void AdvanceStage_MovesThroughEightStagesAndLoops()
        {
            Assert.That(SessionState.CurrentStageNumber, Is.EqualTo(1));
            Assert.That(SessionState.CurrentLoopNumber, Is.EqualTo(1));

            for (int i = 0; i < 7; i++)
            {
                SessionState.AdvanceStage();
            }

            Assert.That(SessionState.CurrentStageNumber, Is.EqualTo(8));
            Assert.That(SessionState.CurrentLoopNumber, Is.EqualTo(1));

            SessionState.AdvanceStage();

            Assert.That(SessionState.CurrentStageNumber, Is.EqualTo(1));
            Assert.That(SessionState.CurrentLoopNumber, Is.EqualTo(2));
        }

        [Test]
        public void AudioSettings_PersistAndClampVolumes()
        {
            SessionState.SetAudioEnabled(false);
            SessionState.SetMusicVolume(1.4f);
            SessionState.SetSfxVolume(-0.2f);

            Assert.That(SessionState.AudioEnabled, Is.False);
            Assert.That(SessionState.MusicVolume, Is.EqualTo(1f));
            Assert.That(SessionState.SfxVolume, Is.EqualTo(0f));
            Assert.That(PlayerPrefs.GetInt("wanwan.audio_enabled", 1), Is.EqualTo(0));
            Assert.That(PlayerPrefs.GetFloat("wanwan.music_volume", 0f), Is.EqualTo(1f));
            Assert.That(PlayerPrefs.GetFloat("wanwan.sfx_volume", 1f), Is.EqualTo(0f));
        }

        [Test]
        public void ShipAndExtendedSettings_PersistAndResetToDefaults()
        {
            SessionState.SelectShip(PlayerShipType.Blue);
            SessionState.SetControlSensitivity(ControlSensitivity.High);
            SessionState.SetVibrationEnabled(false);
            SessionState.SetDamageNumbersEnabled(false);
            SessionState.SetVirtualButtonOpacity(1.4f);
            SessionState.SetVisualEffectsQuality(VisualEffectsQuality.BatterySaver);
            SessionState.SetLeaderboardName("sky");

            Assert.That(SessionState.SelectedShip, Is.EqualTo(PlayerShipType.Blue));
            Assert.That(SessionState.ControlSensitivity, Is.EqualTo(ControlSensitivity.High));
            Assert.That(SessionState.VibrationEnabled, Is.False);
            Assert.That(SessionState.DamageNumbersEnabled, Is.False);
            Assert.That(SessionState.VirtualButtonOpacity, Is.EqualTo(1f));
            Assert.That(SessionState.VisualEffectsQuality, Is.EqualTo(VisualEffectsQuality.BatterySaver));

            SessionState.ResetSettings();

            Assert.That(SessionState.SelectedShip, Is.EqualTo(PlayerShipType.Green));
            Assert.That(SessionState.SelectedDifficulty, Is.EqualTo(GameDifficulty.Medium));
            Assert.That(SessionState.ControlSensitivity, Is.EqualTo(ControlSensitivity.Medium));
            Assert.That(SessionState.VibrationEnabled, Is.True);
            Assert.That(SessionState.DamageNumbersEnabled, Is.True);
            Assert.That(SessionState.VirtualButtonOpacity, Is.EqualTo(0.4f).Within(0.001f));
            Assert.That(SessionState.VisualEffectsQuality, Is.EqualTo(VisualEffectsQuality.Full));
            Assert.That(SessionState.LeaderboardName, Is.EqualTo("AAA"));
            Assert.That(SessionState.SpendableScore, Is.EqualTo(0));
            Assert.That(SessionState.PendingMount, Is.EqualTo(MountType.None));
            Assert.That(SessionState.PendingMountUnits, Is.EqualTo(0));
        }

        [Test]
        public void TryPurchaseMount_DeductsScoreAndStoresPendingMountUnits()
        {
            SessionState.AddSpendableScore(MountConfig.Get(MountType.MissilePod).Cost + 100);

            bool purchased = SessionState.TryPurchaseMount(MountType.MissilePod);

            Assert.That(purchased, Is.True);
            Assert.That(SessionState.PendingMount, Is.EqualTo(MountType.MissilePod));
            Assert.That(SessionState.PendingMountUnits, Is.EqualTo(MountConfig.Get(MountType.MissilePod).PurchaseUnits));
            Assert.That(SessionState.SpendableScore, Is.EqualTo(100));
        }

        [Test]
        public void TryPurchaseMount_AllowsStackingSameConsumableAndRejectsMixedMount()
        {
            MountConfig missile = MountConfig.Get(MountType.MissilePod);
            MountConfig drone = MountConfig.Get(MountType.DefenseDrone);
            SessionState.AddSpendableScore((missile.Cost * 2) + drone.Cost);

            Assert.That(SessionState.TryPurchaseMount(MountType.MissilePod), Is.True);
            Assert.That(SessionState.TryPurchaseMount(MountType.MissilePod), Is.True);
            Assert.That(SessionState.TryPurchaseMount(MountType.DefenseDrone), Is.False);

            Assert.That(SessionState.PendingMount, Is.EqualTo(MountType.MissilePod));
            Assert.That(SessionState.PendingMountUnits, Is.EqualTo(missile.PurchaseUnits * 2));
        }

        [Test]
        public void TryPurchaseMount_RejectsInsufficientScoreAndNone()
        {
            SessionState.AddSpendableScore(MountConfig.Get(MountType.MissilePod).Cost - 1);

            Assert.That(SessionState.TryPurchaseMount(MountType.MissilePod), Is.False);
            Assert.That(SessionState.TryPurchaseMount(MountType.None), Is.False);
            Assert.That(SessionState.PendingMount, Is.EqualTo(MountType.None));
            Assert.That(SessionState.PendingMountUnits, Is.EqualTo(0));
        }

        [Test]
        public void ConsumePendingMountForRun_ReturnsAndClearsPurchasedMountUnits()
        {
            SessionState.AddSpendableScore(MountConfig.Get(MountType.DefenseDrone).Cost);
            SessionState.TryPurchaseMount(MountType.DefenseDrone);

            MountType consumed = SessionState.ConsumePendingMountForRun(out int units);

            Assert.That(consumed, Is.EqualTo(MountType.DefenseDrone));
            Assert.That(units, Is.EqualTo(MountConfig.Get(MountType.DefenseDrone).PurchaseUnits));
            Assert.That(SessionState.PendingMount, Is.EqualTo(MountType.None));
            Assert.That(SessionState.PendingMountUnits, Is.EqualTo(0));
        }

        [Test]
        public void VisualEffectsQuality_PersistsAndDefaultsToFull()
        {
            Assert.That(SessionState.VisualEffectsQuality, Is.EqualTo(VisualEffectsQuality.Full));

            SessionState.SetVisualEffectsQuality(VisualEffectsQuality.BatterySaver);

            Assert.That(SessionState.VisualEffectsQuality, Is.EqualTo(VisualEffectsQuality.BatterySaver));
            Assert.That(PlayerPrefs.GetInt("wanwan.visual_effects_quality", -1), Is.EqualTo((int)VisualEffectsQuality.BatterySaver));
        }

        [Test]
        public void Leaderboard_KeepsTopTenSortedByScore()
        {
            for (int i = 0; i < 12; i++)
            {
                SessionState.RecordLeaderboardScore("P" + i, i * 100, GameDifficulty.Low, i % 8, i / 8);
            }

            LeaderboardEntry[] entries = SessionState.GetLeaderboardEntries();

            Assert.That(entries.Length, Is.EqualTo(10));
            Assert.That(entries[0].Name, Is.EqualTo("P11"));
            Assert.That(entries[0].Score, Is.EqualTo(1100));
            Assert.That(entries[9].Name, Is.EqualTo("P2"));
            Assert.That(entries[9].Score, Is.EqualTo(200));
        }

        [Test]
        public void Leaderboard_FiltersByDifficulty()
        {
            SessionState.RecordLeaderboardScore("LOW", 100, GameDifficulty.Low, 0, 0);
            SessionState.RecordLeaderboardScore("HIG", 300, GameDifficulty.High, 1, 0);

            LeaderboardEntry[] highEntries = SessionState.GetLeaderboardEntries(GameDifficulty.High);

            Assert.That(highEntries.Length, Is.EqualTo(1));
            Assert.That(highEntries[0].Name, Is.EqualTo("HIG"));
            Assert.That(highEntries[0].Difficulty, Is.EqualTo(GameDifficulty.High));
        }

        [Test]
        public void CommitRunScore_UsesConfiguredLeaderboardName()
        {
            SessionState.SetLeaderboardName("ace");

            SessionState.CommitRunScore(700);

            LeaderboardEntry[] entries = SessionState.GetLeaderboardEntries();
            Assert.That(entries[0].Name, Is.EqualTo("ACE"));
        }

        [Test]
        public void UpdateLastLeaderboardName_RenamesCommittedRun()
        {
            SessionState.CommitRunScore(900);

            SessionState.UpdateLastLeaderboardName("sky");

            LeaderboardEntry[] entries = SessionState.GetLeaderboardEntries();
            Assert.That(entries[0].Name, Is.EqualTo("SKY"));
            Assert.That(SessionState.LeaderboardName, Is.EqualTo("SKY"));
        }
    }
}
