using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Wanwan.Runtime;

namespace Wanwan.Editor
{
    public static class MainMenuPrefabBuilder
    {
        private const string PrefabFolder = "Assets/Prefabs/UI/MainMenu";

        public static void GeneratePrefabs()
        {
            Directory.CreateDirectory(PrefabFolder);
            SaveRootPrefab();
            SaveTitlePrefab("TitleSpaceShooter", "SPACE SHOOTER", 42, new Vector2(620f, 110f));
            SaveTitlePrefab("TitleRaiden", "RAIDEN", 112, new Vector2(760f, 220f));
            SaveButtonPrefab("ButtonMain", MenuBootstrap.TitleWideButtonResourcePath, MenuBootstrap.ModernTitleButtonSize, false);
            SaveButtonPrefab("ButtonSquare", MenuBootstrap.TitleIconFrameResourcePath, MenuBootstrap.ModernTitleIconSize, true);
            SaveIconPrefab("IconInfo", MenuBootstrap.IconInfoResourcePath);
            SaveIconPrefab("IconSettings", MenuBootstrap.IconSettingsResourcePath);
            SaveIconPrefab("IconControl", MenuBootstrap.IconControlResourcePath);
            SaveIconPrefab("IconTrophy", MenuBootstrap.IconTrophyResourcePath);
            SaveIconPrefab("IconShop", MenuBootstrap.IconShopResourcePath);
            SaveIconPrefab("IconHelp", MenuBootstrap.IconHelpResourcePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void SaveRootPrefab()
        {
            GameObject root = new GameObject("MainMenuRoot", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(MainMenuController));
            RectTransform rect = root.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            Image image = root.GetComponent<Image>();
            image.color = Color.clear;
            image.raycastTarget = false;
            Save(root, "MainMenuRoot");
        }

        private static void SaveTitlePrefab(string name, string label, int fontSize, Vector2 size)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform));
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.sizeDelta = size;
            Text text = UiFactory.CreateArcadeLabel(obj.transform, label, fontSize, TextAnchor.MiddleCenter, new Color(0.86f, 0.98f, 1f), FontStyle.Bold, Vector2.zero, Vector2.one, Vector2.zero);
            text.gameObject.AddComponent<Outline>().effectColor = new Color(0.05f, 0.42f, 0.6f, 0.95f);
            text.raycastTarget = false;
            Save(obj, name);
        }

        private static void SaveButtonPrefab(string name, string resourcePath, Vector2 size, bool square)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(MenuButtonAnimator));
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.sizeDelta = size;
            Image image = obj.GetComponent<Image>();
            image.sprite = LoadSprite(resourcePath);
            image.type = Image.Type.Simple;
            image.color = Color.white;
            Button button = obj.GetComponent<Button>();
            button.targetGraphic = image;
            Image glow = UiFactory.CreatePanel(obj.transform, "Glow", new Color(0.14f, 0.85f, 1f, square ? 0.18f : 0.12f), new Vector2(0.07f, 0.12f), new Vector2(0.93f, 0.88f));
            glow.raycastTarget = false;
            obj.GetComponent<MenuButtonAnimator>().Configure(image, glow, ArcadeTheme.ElectricBlue, LoadSprite(resourcePath + "_pressed"));
            Save(obj, name);
        }

        private static void SaveIconPrefab(string name, string resourcePath)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(72f, 72f);
            Image image = obj.GetComponent<Image>();
            image.sprite = LoadSprite(resourcePath);
            image.preserveAspect = true;
            image.color = Color.white;
            image.raycastTarget = false;
            Save(obj, name);
        }

        private static Sprite LoadSprite(string resourcePath)
        {
            Texture2D texture = Resources.Load<Texture2D>(resourcePath);
            return texture == null ? RuntimeSpriteFactory.GetRoundedSquareSprite() : Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 128f);
        }

        private static void Save(GameObject obj, string name)
        {
            PrefabUtility.SaveAsPrefabAsset(obj, $"{PrefabFolder}/{name}.prefab");
            Object.DestroyImmediate(obj);
        }
    }
}
