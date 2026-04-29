namespace Wanwan.Runtime
{
    public enum SciFiBackgroundLayerKind
    {
        Deep,
        Nebula,
        FarStars,
        NearStars,
        Dust
    }

    public enum SciFiHudSpriteKind
    {
        TopFrame,
        PausePanel,
        ResultPanel,
        IconBomb,
        IconCoin,
        IconFire
    }

    public enum SciFiButtonSpriteKind
    {
        WideBlue,
        WideCyan,
        IconFrame,
        Disabled
    }

    public static class SciFiResourcePaths
    {
        public const string BackgroundDeep = "SciFi/Backgrounds/space_deep_01";
        public const string BackgroundNebula = "SciFi/Backgrounds/space_nebula_01";
        public const string BackgroundFarStars = "SciFi/Backgrounds/space_stars_far";
        public const string BackgroundNearStars = "SciFi/Backgrounds/space_stars_near";
        public const string BackgroundDust = "SciFi/Backgrounds/space_dust";

        public const string HudTopFrame = "SciFi/UI/Panels/hud_top_frame";
        public const string HudPausePanel = "SciFi/UI/Panels/pause_panel";
        public const string HudResultPanel = "SciFi/UI/Panels/result_panel";
        public const string HudIconBomb = "SciFi/UI/Icons/icon_bomb";
        public const string HudIconCoin = "SciFi/UI/Icons/icon_coin";
        public const string HudIconFire = "SciFi/UI/Icons/icon_firepower";

        public const string ButtonWideBlue = "SciFi/UI/Buttons/btn_wide_blue";
        public const string ButtonWideCyan = "SciFi/UI/Buttons/btn_wide_cyan";
        public const string ButtonIconFrame = "SciFi/UI/Buttons/btn_icon_frame";
        public const string ButtonDisabled = "SciFi/UI/Buttons/btn_disabled";

        public const string ProjectilePlayerBlue = "SciFi/Projectiles/bullet_player_blue";
        public const string ProjectilePlayerLaser = "SciFi/Projectiles/bullet_player_laser";
        public const string ProjectileEnemyRed = "SciFi/Projectiles/bullet_enemy_red";
        public const string ProjectileBombWave = "SciFi/Projectiles/bomb_wave";

        public const string PickupCoin = "SciFi/Pickups/coin";
        public const string PickupBomb = "SciFi/Pickups/bomb_pickup";
        public const string PickupPower = "SciFi/Pickups/power_pickup";

        public const string EffectExplosionCore = "SciFi/Effects/explosion_core";
        public const string EffectExplosionRing = "SciFi/Effects/explosion_ring";
        public const string EffectFlashRadial = "SciFi/Effects/flash_radial";
        public const string EffectCoinTrail = "SciFi/Effects/coin_trail";
    }
}
