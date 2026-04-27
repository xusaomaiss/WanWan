using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    [TestFixture]
    public class MenuTransitionControllerTests
    {
        [Test]
        public void TransitionController_CanBeReferenced()
        {
            var type = typeof(MenuTransitionController);
            Assert.IsNotNull(type);
        }

        [Test]
        public void TransitionController_HasFadeGroupMethod()
        {
            var type = typeof(MenuTransitionController);
            var method = type.GetMethod("FadeGroup");
            Assert.IsNotNull(method);
        }
    }
}
