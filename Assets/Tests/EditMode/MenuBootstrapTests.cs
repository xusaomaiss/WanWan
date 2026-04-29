using NUnit.Framework;
using System.Reflection;
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
            Assert.That(MenuBootstrap.TitlePrimaryLabels, Is.EqualTo(new[] { "START", "MAP", "EXIT" }));
            Assert.That(MenuBootstrap.RightSideMenuLabels, Is.EqualTo(new[] { "CONTROL", "ACHIEVEMENT", "SHOP", "HELP" }));
            Assert.That(MenuBootstrap.ModernTitleButtonSize, Is.EqualTo(new Vector2(486f, 96f)));
            Assert.That(MenuBootstrap.TitleStarfieldResourcePath, Is.EqualTo("MainMenu/Backgrounds/bg_space_far"));
            Assert.That(MenuBootstrap.TitleSparkleOverlayResourcePath, Is.EqualTo("MainMenu/Backgrounds/bg_space_front_stars"));
            Assert.That(MenuBootstrap.TitleLogoBackplateResourcePath, Is.EqualTo("MainMenu/Titles/title_raiden"));
            Assert.That(RuntimeSpriteFactory.RaidenFighterJetResourcePath, Is.EqualTo("RaidenArt/Ships/fighter_jet_128"));
            Assert.That(RuntimeSpriteFactory.MenuStormTitleResourcePath, Is.EqualTo("RaidenArt/Cinematics/menu_storm_title_ai"));
        }

        [Test]
        public void TitleMenu_ResourcesExistForSpaceShooterSkin()
        {
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.TitleStarfieldResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.TitleSparkleOverlayResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.TitleLogoBackplateResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.TitleMidNebulaResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.TitleWideButtonResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.TitleIconFrameResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.IconControlResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.IconTrophyResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.IconShopResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.IconHelpResourcePath), Is.Not.Null);
        }

        [Test]
        public void TitleMenu_PrefabAssetsExistForReusableUiParts()
        {
            string[] prefabNames =
            {
                "MainMenuRoot",
                "TitleSpaceShooter",
                "TitleRaiden",
                "ButtonMain",
                "ButtonSquare",
                "IconInfo",
                "IconSettings",
                "IconControl",
                "IconTrophy",
                "IconShop",
                "IconHelp"
            };

            foreach (string prefabName in prefabNames)
            {
                Assert.That(System.IO.File.Exists($"Assets/Prefabs/UI/MainMenu/{prefabName}.prefab"), Is.True, prefabName);
            }
        }

        [Test]
        public void MainMenuScripts_ExposeCommercialMenuBehaviors()
        {
            Assert.That(typeof(StarfieldParallax), Is.Not.Null);
            Assert.That(typeof(MenuButtonAnimator), Is.Not.Null);
            Assert.That(typeof(MainMenuController), Is.Not.Null);
        }

        [Test]
        public void CreateSpaceMenuButton_UsesSpriteAndKeepsDecorationsOutOfRaycasts()
        {
            GameObject parent = new GameObject("SpaceButtonParent");
            try
            {
                Button button = UiFactory.CreateSpaceMenuButton(parent.transform, "START", MenuBootstrap.TitleWideButtonResourcePath, ArcadeTheme.ElectricBlue, new Vector2(300f, 76f), Vector2.zero);

                Assert.That(button.interactable, Is.True);
                Assert.That(button.GetComponent<Image>().sprite, Is.Not.Null);
                Assert.That(button.GetComponent<MenuButtonAnimator>(), Is.Not.Null);
                Assert.That(button.GetComponentInChildren<Text>().text, Is.EqualTo("START"));

                foreach (Image image in button.GetComponentsInChildren<Image>())
                {
                    if (image.gameObject != button.gameObject)
                    {
                        Assert.That(image.raycastTarget, Is.False, image.gameObject.name);
                    }
                }
            }
            finally
            {
                Object.DestroyImmediate(parent);
            }
        }

        [Test]
        public void TitleMenu_CreatesPrimaryButtonsAndNonBlockingDecorations()
        {
            GameObject host = new GameObject("MenuBootstrapHost");
            try
            {
                MenuBootstrap menu = host.AddComponent<MenuBootstrap>();
                typeof(MenuBootstrap).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(menu, null);

                foreach (string label in MenuBootstrap.TitlePrimaryLabels)
                {
                    Button button = FindButtonByLabel(label);
                    Assert.That(button, Is.Not.Null, label);
                    Assert.That(button.interactable, Is.True, label);
                    Assert.That(button.targetGraphic, Is.Not.Null, label);
                }

                Assert.That(GameObject.Find("TitleBackgroundShade").GetComponent<Image>().raycastTarget, Is.False);
                Assert.That(GameObject.Find("MainMenuRoot").GetComponent<MainMenuController>(), Is.Not.Null);
                Assert.That(GameObject.Find("TitleContentGroup").GetComponent<MenuScaleInAnimator>(), Is.Not.Null);
                Assert.That(GameObject.Find("CONTROLButton"), Is.Not.Null);
                Assert.That(GameObject.Find("ACHIEVEMENTButton"), Is.Not.Null);
                Assert.That(GameObject.Find("SHOPButton"), Is.Not.Null);
                Assert.That(GameObject.Find("HELPButton"), Is.Not.Null);
                Assert.That(GameObject.Find("FarStarsA").GetComponent<StarfieldParallax>(), Is.Not.Null);
                Assert.That(GameObject.Find("MidNebulaA").GetComponent<StarfieldParallax>(), Is.Not.Null);
                Assert.That(GameObject.Find("FrontStarsA").GetComponent<StarfieldParallax>(), Is.Not.Null);
            }
            finally
            {
                foreach (Canvas canvas in Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
                {
                    Object.DestroyImmediate(canvas.gameObject);
                }

                foreach (Camera camera in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
                {
                    Object.DestroyImmediate(camera.gameObject);
                }

                foreach (EventSystem eventSystem in Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None))
                {
                    Object.DestroyImmediate(eventSystem.gameObject);
                }

                Object.DestroyImmediate(host);
            }
        }

        private static Button FindButtonByLabel(string label)
        {
            foreach (Button button in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
            {
                Text text = button.GetComponentInChildren<Text>();
                if (text != null && text.text == label)
                {
                    return button;
                }
            }

            return null;
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
