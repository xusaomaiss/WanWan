using UnityEngine;
using UnityEngine.UI;

namespace Wanwan.Runtime
{
    public static class ArcadeFontProvider
    {
        public const string FallbackFontResourceName = "Arial.ttf";
        public const string BundledChineseFontResourceName = "Fonts/NotoSansCJKsc-Regular";

        public static readonly string[] PreferredFontNames =
        {
            "PingFangSC-Regular",
            "PingFangSC-Semibold",
            "PingFangSC-Medium",
            "PingFang SC",
            "Heiti SC",
            "STHeitiSC-Light",
            "STHeitiSC-Medium",
            "Hiragino Sans GB",
            "STHeiti",
            "Arial Unicode MS",
            "Helvetica Neue",
            "Arial"
        };

        private const int DynamicFontSize = 96;
        private static Font cachedFont;

        public static Font GetUiFont()
        {
            if (cachedFont != null)
            {
                return cachedFont;
            }

            cachedFont = Resources.Load<Font>(BundledChineseFontResourceName);
            if (cachedFont != null)
            {
                return cachedFont;
            }

            cachedFont = Font.CreateDynamicFontFromOSFont(PreferredFontNames, DynamicFontSize);
            if (cachedFont == null)
            {
                cachedFont = Resources.GetBuiltinResource<Font>(FallbackFontResourceName);
            }

            return cachedFont;
        }

        public static void ApplyTo(Text text)
        {
            if (text == null)
            {
                return;
            }

            text.font = GetUiFont();
        }

        public static void ApplyTo(TextMesh textMesh)
        {
            if (textMesh == null)
            {
                return;
            }

            Font font = GetUiFont();
            textMesh.font = font;
            MeshRenderer renderer = textMesh.GetComponent<MeshRenderer>();
            if (renderer != null && font != null)
            {
                renderer.sharedMaterial = font.material;
            }
        }
    }
}
