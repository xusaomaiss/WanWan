using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class BombStateTests
    {
        [Test]
        public void Constructor_SetsInitialCount()
        {
            BombState state = new BombState(3);

            Assert.That(state.Count, Is.EqualTo(3));
            Assert.That(state.HasBomb, Is.True);
        }

        [Test]
        public void TryConsume_DecrementsCount()
        {
            BombState state = new BombState(2);

            bool consumed = state.TryConsume();

            Assert.That(consumed, Is.True);
            Assert.That(state.Count, Is.EqualTo(1));
        }

        [Test]
        public void TryConsume_WhenEmpty_ReturnsFalseAndDoesNotGoNegative()
        {
            BombState state = new BombState(0);

            bool consumed = state.TryConsume();

            Assert.That(consumed, Is.False);
            Assert.That(state.Count, Is.EqualTo(0));
        }
    }
}
