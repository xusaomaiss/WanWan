using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
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

        [Test]
        public void CreateCanvas_ProvidesStandaloneInputForButtons()
        {
            EventSystem existing = Object.FindFirstObjectByType<EventSystem>();
            if (existing != null)
            {
                Object.DestroyImmediate(existing.gameObject);
            }

            Canvas canvas = UiFactory.CreateCanvas("InputCanvas");
            try
            {
                EventSystem eventSystem = Object.FindFirstObjectByType<EventSystem>();

                Assert.That(eventSystem, Is.Not.Null);
                Assert.That(eventSystem.GetComponent<StandaloneInputModule>(), Is.Not.Null);
                Assert.That(eventSystem.GetComponent<TouchInputModule>(), Is.Null);
                Assert.That(canvas.GetComponent<GraphicRaycaster>(), Is.Not.Null);
            }
            finally
            {
                if (canvas != null)
                {
                    Object.DestroyImmediate(canvas.gameObject);
                }

                EventSystem eventSystem = Object.FindFirstObjectByType<EventSystem>();
                if (eventSystem != null)
                {
                    Object.DestroyImmediate(eventSystem.gameObject);
                }
            }
        }
    }
}
