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

            Assert.That(SessionState.SelectedShip, Is.EqualTo(PlayerShipType.Blue));
            Assert.That(SessionState.ControlSensitivity, Is.EqualTo(ControlSensitivity.High));
            Assert.That(SessionState.VibrationEnabled, Is.False);
            Assert.That(SessionState.DamageNumbersEnabled, Is.False);
            Assert.That(SessionState.VirtualButtonOpacity, Is.EqualTo(1f));

            SessionState.ResetSettings();

            Assert.That(SessionState.SelectedShip, Is.EqualTo(PlayerShipType.Green));
            Assert.That(SessionState.ControlSensitivity, Is.EqualTo(ControlSensitivity.Medium));
            Assert.That(SessionState.VibrationEnabled, Is.True);
            Assert.That(SessionState.DamageNumbersEnabled, Is.True);
            Assert.That(SessionState.VirtualButtonOpacity, Is.EqualTo(0.4f).Within(0.001f));
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
    }
}
