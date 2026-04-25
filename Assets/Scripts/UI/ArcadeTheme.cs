using UnityEngine;

namespace Wanwan.Runtime
{
    public static class ArcadeTheme
    {
        public static readonly Color BackgroundBlack = FromHex(0x08, 0x08, 0x10);
        public static readonly Color InkBlack = FromHex(0x0A, 0x0A, 0x0A);
        public static readonly Color PanelBase = FromHex(0x1A, 0x1A, 0x2E);
        public static readonly Color MilitaryGreen = FromHex(0x00, 0xFF, 0x41);
        public static readonly Color ElectricBlue = FromHex(0x00, 0xD4, 0xFF);
        public static readonly Color WarningRed = FromHex(0xFF, 0x00, 0x40);
        public static readonly Color EnergyYellow = FromHex(0xFF, 0xD7, 0x00);
        public static readonly Color White = Color.white;
        public static readonly Color DimGray = FromHex(0x4A, 0x4A, 0x6A);

        public const int LogoSize = 64;
        public const int ScreenTitleSize = 48;
        public const int TitleSize = 32;
        public const int BodySize = 24;
        public const int SmallSize = 18;
        public const int ScoreSize = 30;

        public static Color FromHex(byte r, byte g, byte b)
        {
            return new Color32(r, g, b, 255);
        }
    }
}
