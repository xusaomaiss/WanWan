using NUnit.Framework;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class ArcadeFontProviderTests
    {
        [Test]
        public void PreferredFontNames_PrioritizesIosChineseFonts()
        {
            Assert.That(ArcadeFontProvider.PreferredFontNames[0], Is.EqualTo("PingFangSC-Regular"));
            Assert.That(ArcadeFontProvider.PreferredFontNames, Does.Contain("PingFang SC"));
            Assert.That(ArcadeFontProvider.PreferredFontNames, Does.Contain("Heiti SC"));
        }

        [Test]
        public void CreateText_UsesSharedArcadeFontProvider()
        {
            GameObject parent = new GameObject("TextParent");
            try
            {
                Text text = UiFactory.CreateText(parent.transform, "最高分", 24, TextAnchor.MiddleCenter, Color.white, Vector2.zero, Vector2.one, Vector2.zero);

                Assert.That(text.font, Is.SameAs(ArcadeFontProvider.GetUiFont()));
            }
            finally
            {
                Object.DestroyImmediate(parent);
            }
        }

        [Test]
        public void GetUiFont_PrefersBundledChineseFont()
        {
            Font bundled = Resources.Load<Font>(ArcadeFontProvider.BundledChineseFontResourceName);

            Assert.That(bundled, Is.Not.Null);
            Assert.That(ArcadeFontProvider.GetUiFont(), Is.SameAs(bundled));
        }

        [Test]
        public void CreateText_DisablesAutomaticWrappingForChineseLabels()
        {
            GameObject parent = new GameObject("TextParent");
            try
            {
                Text text = UiFactory.CreateText(parent.transform, "继续下一关", 24, TextAnchor.MiddleCenter, Color.white, Vector2.zero, Vector2.one, Vector2.zero);

                Assert.That(text.text, Is.EqualTo("继续下一关"));
                Assert.That(text.horizontalOverflow, Is.EqualTo(HorizontalWrapMode.Overflow));
                Assert.That(text.verticalOverflow, Is.EqualTo(VerticalWrapMode.Overflow));
                Assert.That(text.resizeTextForBestFit, Is.False);
            }
            finally
            {
                Object.DestroyImmediate(parent);
            }
        }

        [Test]
        public void CreateButton_DisablesBestFitToAvoidIosDynamicFontGlyphDropout()
        {
            GameObject parent = new GameObject("ButtonParent");
            try
            {
                Button button = UiFactory.CreatePixelButton(parent.transform, "继续下一关", ArcadeTheme.ElectricBlue, new Vector2(390f, 118f), Vector2.zero);
                Text text = button.GetComponentInChildren<Text>();

                Assert.That(text.text, Is.EqualTo("继续下一关"));
                Assert.That(text.resizeTextForBestFit, Is.False);
            }
            finally
            {
                Object.DestroyImmediate(parent);
            }
        }

        [Test]
        public void CreateText_PreservesExplicitLineBreaks()
        {
            GameObject parent = new GameObject("TextParent");
            try
            {
                Text text = UiFactory.CreateText(parent.transform, "装甲 10/10\n炸弹 ◆◆◇", 24, TextAnchor.MiddleCenter, Color.white, Vector2.zero, Vector2.one, Vector2.zero);

                Assert.That(text.text, Is.EqualTo("装甲 10/10\n炸弹 ◆◆◇"));
                Assert.That(text.text, Does.Contain("\n"));
            }
            finally
            {
                Object.DestroyImmediate(parent);
            }
        }

        [Test]
        public void ApplyToTextMesh_UsesSharedArcadeFontProvider()
        {
            GameObject label = new GameObject("TextMeshLabel");
            try
            {
                TextMesh textMesh = label.AddComponent<TextMesh>();

                ArcadeFontProvider.ApplyTo(textMesh);

                Assert.That(textMesh.font, Is.SameAs(ArcadeFontProvider.GetUiFont()));
            }
            finally
            {
                Object.DestroyImmediate(label);
            }
        }

        [Test]
        public void UiFont_SupportsCommonChineseHudCharacters()
        {
            Font font = ArcadeFontProvider.GetUiFont();
            string sample = "空投爆破街机空战指挥部选择战机主武器副武器速度火力选择难度本地排行榜名次名号分数关卡设置得分金币最高分连击倍率装甲炸弹升级充能警报敌方旗舰暂停菜单任务失败击落点击跳过能量胶囊医疗补给";

            foreach (char character in sample)
            {
                Assert.That(font.HasCharacter(character), Is.True, $"Font {font.name} is missing Chinese character '{character}'.");
            }
        }

        [Test]
        public void RuntimeScripts_DoNotBypassUiTextFactoryOrFontProvider()
        {
            string scriptsRoot = Path.Combine(Application.dataPath, "Scripts");
            string[] offenders = Directory.GetFiles(scriptsRoot, "*.cs", SearchOption.AllDirectories)
                .Where(path => Path.GetFileName(path) != "UiFactory.cs" && Path.GetFileName(path) != "ArcadeFontProvider.cs")
                .Where(path =>
                {
                    string source = File.ReadAllText(path);
                    return source.Contains("AddComponent<Text>()") ||
                           source.Contains("typeof(Text)") ||
                           source.Contains("Resources.GetBuiltinResource<Font>(\"Arial.ttf\")");
                })
                .Select(path => path.Substring(Application.dataPath.Length + 1))
                .ToArray();

            Assert.That(offenders, Is.Empty, "Runtime UI text must go through UiFactory or ArcadeFontProvider so iOS Chinese glyphs and overflow rules stay consistent.");
        }
    }
}
