using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    [TestFixture]
    public class ScoreCounterTests
    {
        [Test]
        public void Start_InitializesAtZero()
        {
            var counter = new ScoreCounter();
            counter.Start(10000);
            Assert.AreEqual(0, counter.CurrentScore);
            Assert.IsFalse(counter.IsComplete);
        }

        [Test]
        public void TicksUp_ToTargetScore()
        {
            var counter = new ScoreCounter();
            counter.Start(10000);

            while (!counter.IsComplete)
            {
                counter.Tick(0.016f);
            }
            Assert.AreEqual(10000, counter.CurrentScore);
        }

        [Test]
        public void LargeScore_CountsUpCorrectly()
        {
            var counter = new ScoreCounter();
            counter.Start(9999999);
            while (!counter.IsComplete)
            {
                counter.Tick(0.016f);
            }
            Assert.AreEqual(9999999, counter.CurrentScore);
        }

        [Test]
        public void ZeroTarget_CompletesImmediately()
        {
            var counter = new ScoreCounter();
            counter.Start(0);
            Assert.AreEqual(0, counter.CurrentScore);
        }

        [Test]
        public void Progress_IsMonotonic()
        {
            var counter = new ScoreCounter();
            counter.Start(50000);
            int previous = 0;
            while (!counter.IsComplete)
            {
                counter.Tick(0.016f);
                Assert.GreaterOrEqual(counter.CurrentScore, previous);
                previous = counter.CurrentScore;
            }
        }

        [Test]
        public void PartialTick_DoesNotReachTarget()
        {
            var counter = new ScoreCounter();
            counter.Start(10000);
            counter.Tick(0.5f);
            Assert.Greater(counter.CurrentScore, 0);
            Assert.Less(counter.CurrentScore, 10000);
            Assert.IsFalse(counter.IsComplete);
        }
    }
}
