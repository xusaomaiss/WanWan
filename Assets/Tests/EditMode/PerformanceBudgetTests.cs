using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class PerformanceBudgetTests
    {
        [Test]
        public void AndroidLowEndBudget_TargetsStableSixtyFps()
        {
            Assert.That(PerformanceBudget.TargetFrameRate, Is.EqualTo(60));
            Assert.That(PerformanceBudget.FrameBudgetMilliseconds, Is.EqualTo(16.7f).Within(0.1f));
        }

        [Test]
        public void AndroidLowEndBudget_KeepsMemoryWarningConservative()
        {
            Assert.That(PerformanceBudget.MemoryWarningThresholdMb, Is.LessThanOrEqualTo(256));
            Assert.That(PerformanceBudget.TextureMemoryBudgetMb, Is.LessThanOrEqualTo(96));
        }
    }
}
