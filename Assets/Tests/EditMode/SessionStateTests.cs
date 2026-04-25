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
    }
}
