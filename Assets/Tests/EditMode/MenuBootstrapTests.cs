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

        [Test]
        public void TitleMenu_UsesIndependentHeroFighterLayer()
        {
            Assert.That(MenuBootstrap.TitleHeroFighterObjectName, Is.EqualTo("TitleHeroFighter"));
            Assert.That(RuntimeSpriteFactory.RaidenFighterJetResourcePath, Is.EqualTo("RaidenArt/Ships/fighter_jet_128"));
            Assert.That(RuntimeSpriteFactory.MenuStormTitleResourcePath, Is.EqualTo("RaidenArt/Cinematics/menu_storm_title_ai"));
        }
    }
}
