using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class MenuBootstrapTests
    {
        [Test]
        public void MenuStartup_SkipsLogoInterlude()
        {
            Assert.That(MenuBootstrap.ShowStartupLogo, Is.False);
        }

        [Test]
        public void TitleHighScore_UsesSaveButtonFontSize()
        {
            Assert.That(MenuBootstrap.TitleHighScoreFontSize, Is.EqualTo(30));
            Assert.That(MenuBootstrap.TitleHighScoreFontSize, Is.EqualTo(MenuBootstrap.TitleCircleButtonFontSize));
        }
    }
}
