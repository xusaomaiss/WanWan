namespace Wanwan.Runtime.Pools
{
    public static class PoolConfig
    {
        public static int GetBulletPoolSize(VisualEffectsQuality quality) =>
            quality == VisualEffectsQuality.Full ? 128 : 64;

        public static int GetEnemyPoolSize(VisualEffectsQuality quality) =>
            quality == VisualEffectsQuality.Full ? 48 : 24;

        public static int GetParticlePoolSize(VisualEffectsQuality quality) =>
            quality == VisualEffectsQuality.Full ? 96 : 32;

        public static int GetPickupPoolSize(VisualEffectsQuality quality) =>
            quality == VisualEffectsQuality.Full ? 32 : 16;

        public static int GetFireballPoolSize(VisualEffectsQuality quality) =>
            quality == VisualEffectsQuality.Full ? 64 : 32;

        public static int GetLabelPoolSize(VisualEffectsQuality quality) =>
            quality == VisualEffectsQuality.Full ? 24 : 12;
    }
}
