using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class WaveDirectorTests
    {
        [Test]
        public void Constructor_StartsInCalm()
        {
            WaveDirector director = new WaveDirector();

            Assert.That(director.GetCurrentPhase(), Is.EqualTo(WavePhase.Calm));
            Assert.That(director.GetPhaseProgress(), Is.EqualTo(0f));
        }

        [Test]
        public void UpdatePhase_AdvancesThroughConfiguredOrder()
        {
            WaveDirector director = new WaveDirector();

            director.UpdatePhase(5f);
            Assert.That(director.GetCurrentPhase(), Is.EqualTo(WavePhase.Pressure));

            director.UpdatePhase(6f);
            Assert.That(director.GetCurrentPhase(), Is.EqualTo(WavePhase.Burst));

            director.UpdatePhase(5f);
            Assert.That(director.GetCurrentPhase(), Is.EqualTo(WavePhase.Reward));

            director.UpdatePhase(3f);
            Assert.That(director.GetCurrentPhase(), Is.EqualTo(WavePhase.Calm));
        }

        [Test]
        public void UpdatePhase_UsesExpectedPhaseDurations()
        {
            WaveDirector director = new WaveDirector();

            director.UpdatePhase(4.99f);
            Assert.That(director.GetCurrentPhase(), Is.EqualTo(WavePhase.Calm));

            director.UpdatePhase(0.01f);
            Assert.That(director.GetCurrentPhase(), Is.EqualTo(WavePhase.Pressure));

            director.UpdatePhase(5.99f);
            Assert.That(director.GetCurrentPhase(), Is.EqualTo(WavePhase.Pressure));

            director.UpdatePhase(0.01f);
            Assert.That(director.GetCurrentPhase(), Is.EqualTo(WavePhase.Burst));

            director.UpdatePhase(4.99f);
            Assert.That(director.GetCurrentPhase(), Is.EqualTo(WavePhase.Burst));

            director.UpdatePhase(0.01f);
            Assert.That(director.GetCurrentPhase(), Is.EqualTo(WavePhase.Reward));

            director.UpdatePhase(2.99f);
            Assert.That(director.GetCurrentPhase(), Is.EqualTo(WavePhase.Reward));

            director.UpdatePhase(0.01f);
            Assert.That(director.GetCurrentPhase(), Is.EqualTo(WavePhase.Calm));
        }

        [Test]
        public void UpdatePhase_FullCycleReturnsToCalm()
        {
            WaveDirector director = new WaveDirector();

            director.UpdatePhase(5f);
            director.UpdatePhase(6f);
            director.UpdatePhase(5f);
            director.UpdatePhase(3f);

            Assert.That(director.GetCurrentPhase(), Is.EqualTo(WavePhase.Calm));
            Assert.That(director.GetPhaseProgress(), Is.EqualTo(0f));
        }

        [Test]
        public void UpdatePhase_LargeDeltaAdvancesOnlyOnePhase()
        {
            WaveDirector director = new WaveDirector();

            director.UpdatePhase(60f);

            Assert.That(director.GetCurrentPhase(), Is.EqualTo(WavePhase.Pressure));
        }
    }
}
