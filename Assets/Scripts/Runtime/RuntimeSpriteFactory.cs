using System.Collections.Generic;
using UnityEngine;

namespace Wanwan.Runtime
{
    public static class RuntimeSpriteFactory
    {
        public const string RaidenFighterJetResourcePath = "RaidenArt/Ships/fighter_jet_128";
        public const string RaidenEnemyJetResourcePath = "RaidenArt/Ships/enemy_jet_128";
        public const string ToughEnemyResourcePath = "RaidenArt/Ships/tough_enemy_ai";
        public const string EliteEnemyResourcePath = "RaidenArt/Ships/elite_enemy_ai";
        public const string BossFlagshipResourcePath = "RaidenArt/Ships/boss_flagship_ai";
        public const string CoinResourcePath = "RaidenArt/Pickups/coin_ai";

        private static readonly string[] RaidenStageBackgroundResourcePaths =
        {
            "RaidenArt/Backgrounds/stage_01_countryside",
            "RaidenArt/Backgrounds/stage_02_city",
            "RaidenArt/Backgrounds/stage_03_coastline",
            "RaidenArt/Backgrounds/stage_04_ruins",
            "RaidenArt/Backgrounds/stage_05_wasteland",
            "RaidenArt/Backgrounds/stage_06_floating_continent",
            "RaidenArt/Backgrounds/stage_07_space_station",
            "RaidenArt/Backgrounds/stage_08_alien_base"
        };

        private static readonly Dictionary<string, Sprite> SpriteCache = new Dictionary<string, Sprite>();

        public static Sprite GetRoundedSquareSprite()
        {
            return GetOrCreate("rounded-square", BuildRoundedSquareTexture);
        }

        public static Sprite GetCircleSprite()
        {
            return GetOrCreate("circle", BuildCircleTexture);
        }

        public static Sprite GetMissileSprite()
        {
            return GetOrCreate("missile", BuildMissileTexture);
        }

        public static Sprite GetBulletSprite(AmmoPowerupType type)
        {
            return GetOrCreate("bullet-" + type, () => BuildBulletTexture(type));
        }

        public static Sprite GetAmmoPackSprite(AmmoPowerupType type)
        {
            return GetOrCreate("ammo-pack-" + type, () => BuildAmmoPackTexture(type));
        }

        public static Sprite GetCapsuleSprite()
        {
            return GetOrCreate("capsule", BuildCapsuleTexture);
        }

        public static Sprite GetFighterJetSprite()
        {
            return GetOrCreate("fighter-jet", BuildHeroFighterTexture);
        }

        public static Sprite GetRaidenFighterJetSprite()
        {
            return GetResourceSpriteOrFallback("raiden-fighter-jet", RaidenFighterJetResourcePath, GetFighterJetSprite);
        }

        public static Sprite GetEnemyInterceptorSprite()
        {
            return GetResourceSpriteOrFallback("raiden-enemy-jet", RaidenEnemyJetResourcePath, GetGeneratedEnemyInterceptorSprite);
        }

        public static Sprite GetToughInterceptorSprite()
        {
            return GetResourceSpriteOrFallback("tough-enemy-ai", ToughEnemyResourcePath, GetGeneratedEnemyInterceptorSprite);
        }

        public static Sprite GetEliteInterceptorSprite()
        {
            return GetResourceSpriteOrFallback("elite-enemy-ai", EliteEnemyResourcePath, GetGeneratedEliteInterceptorSprite);
        }

        public static Sprite GetBossFlagshipSprite()
        {
            return GetResourceSpriteOrFallback("boss-flagship-ai", BossFlagshipResourcePath, GetGeneratedBossFlagshipSprite);
        }

        public static Sprite GetPlaneSprite()
        {
            return GetEnemyInterceptorSprite();
        }

        private static Sprite GetGeneratedEnemyInterceptorSprite()
        {
            return GetOrCreate("enemy-interceptor", BuildEnemyInterceptorTexture);
        }

        private static Sprite GetGeneratedEliteInterceptorSprite()
        {
            return GetOrCreate("elite-interceptor", BuildEliteInterceptorTexture);
        }

        private static Sprite GetGeneratedBossFlagshipSprite()
        {
            return GetOrCreate("boss-flagship", BuildBossFlagshipTexture);
        }

        public static Sprite GetBattlefieldBackgroundSprite()
        {
            return GetOrCreate("battlefield-background", BuildBattlefieldBackgroundTexture);
        }

        public static string GetRaidenStageBackgroundResourcePath(int stageNumber)
        {
            int safeIndex = Mathf.Clamp(stageNumber - 1, 0, RaidenStageBackgroundResourcePaths.Length - 1);
            return RaidenStageBackgroundResourcePaths[safeIndex];
        }

        public static Sprite GetRaidenStageBackgroundSprite(int stageNumber)
        {
            string resourcePath = GetRaidenStageBackgroundResourcePath(stageNumber);
            return GetResourceSpriteOrFallback("raiden-stage-background-" + stageNumber, resourcePath, GetBattlefieldBackgroundSprite);
        }

        public static Sprite GetSkyBackgroundSprite()
        {
            return GetOrCreate("sky-background", BuildSkyBackgroundTexture);
        }

        public static Sprite GetCloudLayerSprite()
        {
            return GetOrCreate("cloud-layer", BuildCloudLayerTexture);
        }

        public static Sprite GetCloudStreakSprite()
        {
            return GetOrCreate("cloud-streak-layer", BuildCloudStreakTexture);
        }

        public static Sprite GetCarrierDeckSprite()
        {
            return GetOrCreate("carrier-deck", BuildCarrierDeckTexture);
        }

        public static Sprite GetExplosionSprite()
        {
            return GetOrCreate("explosion-fireball", BuildExplosionTexture);
        }

        public static Sprite GetCoinSprite()
        {
            return GetResourceSpriteOrFallback("coin-ai", CoinResourcePath, GetGeneratedCoinSprite);
        }

        public static Sprite GetBombPickupSprite()
        {
            return GetOrCreate("bomb-pickup", BuildBombPickupTexture);
        }

        private static Sprite GetGeneratedCoinSprite()
        {
            return GetOrCreate("coin", BuildCoinTexture);
        }

        private static Sprite GetOrCreate(string key, System.Func<Texture2D> textureFactory)
        {
            if (SpriteCache.TryGetValue(key, out Sprite sprite))
            {
                return sprite;
            }

            Texture2D texture = textureFactory();
            texture.filterMode = FilterMode.Bilinear;
            sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 128f);
            sprite.name = key;
            SpriteCache[key] = sprite;
            return sprite;
        }

        private static Sprite GetResourceSpriteOrFallback(string key, string resourcePath, System.Func<Sprite> fallbackFactory)
        {
            if (SpriteCache.TryGetValue(key, out Sprite sprite))
            {
                return sprite;
            }

            Texture2D texture = Resources.Load<Texture2D>(resourcePath);
            if (texture == null)
            {
                sprite = fallbackFactory();
                SpriteCache[key] = sprite;
                return sprite;
            }

            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 128f);
            sprite.name = key;
            SpriteCache[key] = sprite;
            return sprite;
        }

        private static Texture2D BuildRoundedSquareTexture()
        {
            const int size = 128;
            const float radius = 26f;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
            float half = size * 0.5f - 4f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 p = new Vector2(x, y) - center;
                    float dx = Mathf.Max(Mathf.Abs(p.x) - (half - radius), 0f);
                    float dy = Mathf.Max(Mathf.Abs(p.y) - (half - radius), 0f);
                    float distance = Mathf.Sqrt((dx * dx) + (dy * dy));
                    float alpha = distance <= radius ? 1f : 0f;
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D BuildCircleTexture()
        {
            const int size = 128;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
            float radius = size * 0.42f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    float alpha = distance <= radius ? 1f : 0f;
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D BuildCapsuleTexture()
        {
            const int width = 160;
            const int height = 96;
            const float radius = 42f;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Vector2 leftCenter = new Vector2(radius + 6f, height * 0.5f);
            Vector2 rightCenter = new Vector2(width - radius - 6f, height * 0.5f);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2 point = new Vector2(x, y);
                    bool inMiddle = x >= leftCenter.x && x <= rightCenter.x && Mathf.Abs(y - (height * 0.5f)) <= radius;
                    bool inLeft = Vector2.Distance(point, leftCenter) <= radius;
                    bool inRight = Vector2.Distance(point, rightCenter) <= radius;
                    float alpha = inMiddle || inLeft || inRight ? 1f : 0f;
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D BuildCarrierDeckTexture()
        {
            const int width = 512;
            const int height = 192;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Vector2 center = new Vector2(width * 0.5f, height * 0.5f);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float u = x / (float)(width - 1);
                    float v = y / (float)(height - 1);
                    float deckWidth = Mathf.Lerp(0.2f, 0.88f, v);
                    float dx = Mathf.Abs(u - 0.5f);
                    bool deck = dx <= deckWidth * 0.5f;

                    if (!deck)
                    {
                        texture.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    float noise = Mathf.PerlinNoise(u * 24f, v * 8f);
                    Color baseColor = Color.Lerp(new Color(0.09f, 0.1f, 0.14f), new Color(0.22f, 0.24f, 0.3f), v);
                    Color pixel = Color.Lerp(baseColor, new Color(0.33f, 0.35f, 0.42f), noise * 0.18f);
                    bool centerLine = Mathf.Abs(x - center.x) < 4f && y > 20;
                    bool runwayMark = centerLine || (Mathf.Abs(x - center.x) < 18f && y % 42 < 20 && y > 34);
                    bool sideLight = (Mathf.Abs(dx - (deckWidth * 0.5f)) < 0.018f) && y % 28 < 14;

                    if (runwayMark)
                    {
                        pixel = Color.Lerp(pixel, new Color(0.92f, 0.96f, 1f), 0.82f);
                    }
                    else if (sideLight)
                    {
                        pixel = Color.Lerp(pixel, new Color(0.28f, 0.9f, 1f), 0.72f);
                    }

                    texture.SetPixel(x, y, pixel);
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D BuildExplosionTexture()
        {
            const int size = 192;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Vector2 center = new Vector2(size * 0.5f, size * 0.5f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 point = new Vector2(x, y);
                    Vector2 delta = point - center;
                    float distance = delta.magnitude / (size * 0.5f);
                    float angleNoise = Mathf.PerlinNoise((delta.normalized.x * 2.6f) + 4.5f, (delta.normalized.y * 2.6f) + 3.2f);
                    float flameRadius = 0.58f + (angleNoise * 0.28f);
                    if (distance > flameRadius)
                    {
                        texture.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    Color pixel;
                    if (distance < 0.18f)
                    {
                        pixel = new Color(1f, 0.98f, 0.72f, 1f);
                    }
                    else if (distance < 0.38f)
                    {
                        pixel = Color.Lerp(new Color(1f, 0.86f, 0.22f), new Color(1f, 0.36f, 0.08f), Mathf.InverseLerp(0.18f, 0.38f, distance));
                    }
                    else if (distance < 0.6f)
                    {
                        pixel = Color.Lerp(new Color(1f, 0.22f, 0.04f), new Color(0.22f, 0.16f, 0.14f), Mathf.InverseLerp(0.38f, 0.6f, distance));
                    }
                    else
                    {
                        pixel = new Color(0.08f, 0.08f, 0.08f, Mathf.InverseLerp(flameRadius, 0.6f, distance) * 0.82f);
                    }

                    pixel.a *= Mathf.Clamp01((flameRadius - distance) * 5f);
                    texture.SetPixel(x, y, pixel);
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D BuildCoinTexture()
        {
            const int size = 128;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Vector2 center = new Vector2(size * 0.5f, size * 0.5f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    if (distance > 54f)
                    {
                        texture.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    float shine = Mathf.InverseLerp(54f, 0f, distance);
                    Color pixel = Color.Lerp(new Color(1f, 0.58f, 0.08f), new Color(1f, 0.95f, 0.32f), shine);
                    if (distance > 43f)
                    {
                        pixel = new Color(1f, 0.78f, 0.12f, 1f);
                    }

                    bool innerRing = Mathf.Abs(distance - 31f) < 3f;
                    bool coinMark = Mathf.Abs(x - center.x) < 5f && y > 34f && y < 94f;
                    if (innerRing || coinMark)
                    {
                        pixel = Color.Lerp(pixel, Color.white, 0.46f);
                    }

                    texture.SetPixel(x, y, pixel);
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D BuildBombPickupTexture()
        {
            const int size = 128;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Vector2 center = new Vector2(size * 0.5f, size * 0.5f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 point = new Vector2(x, y);
                    float distance = Vector2.Distance(point, center);
                    bool body = distance <= 38f;
                    bool cap = x >= 76 && x <= 100 && y >= 86 && y <= 104;
                    bool fuse = Mathf.Abs((y - 98f) - ((x - 92f) * 0.5f)) < 4f && x >= 92 && x <= 118;
                    bool spark = Vector2.Distance(point, new Vector2(116f, 110f)) <= 9f;

                    if (!(body || cap || fuse || spark))
                    {
                        texture.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    Color pixel = new Color(0.05f, 0.09f, 0.14f, 1f);
                    if (body)
                    {
                        pixel = Color.Lerp(new Color(0.06f, 0.12f, 0.2f), new Color(0.28f, 0.74f, 1f), Mathf.InverseLerp(40f, 0f, distance) * 0.55f);
                    }

                    if (cap)
                    {
                        pixel = new Color(0.76f, 0.88f, 0.96f, 1f);
                    }
                    else if (fuse)
                    {
                        pixel = new Color(1f, 0.9f, 0.44f, 1f);
                    }
                    else if (spark)
                    {
                        pixel = new Color(1f, 0.42f, 0.2f, 1f);
                    }

                    texture.SetPixel(x, y, pixel);
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D BuildMissileTexture()
        {
            const int width = 160;
            const int height = 240;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Vector2 center = new Vector2(width * 0.5f, height * 0.5f);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float dx = Mathf.Abs(x - center.x);
                    bool core = dx <= 10f && y >= 36f && y <= 194f;
                    bool nose = y > 194f && y <= 228f && dx <= Mathf.Lerp(1f, 10f, Mathf.InverseLerp(228f, 194f, y));
                    bool centerBlade = dx <= 4f && y >= 56f && y <= 206f;
                    bool leftFin = x >= 16f && x <= 52f && y >= 78f && y <= 126f && y <= 126f - ((x - 16f) * 0.88f);
                    bool rightFin = x >= 108f && x <= 144f && y >= 78f && y <= 126f && y <= 126f - ((144f - x) * 0.88f);
                    bool tailWing = y >= 20f && y <= 46f && dx <= Mathf.Lerp(28f, 12f, Mathf.InverseLerp(20f, 46f, y));
                    bool tailFlare = dx <= 8f && y >= 12f && y <= 30f;

                    bool filled = core || nose || centerBlade || leftFin || rightFin || tailWing || tailFlare;
                    if (!filled)
                    {
                        texture.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    if (tailFlare)
                    {
                        texture.SetPixel(x, y, new Color(1f, 0.84f, 0.34f, 1f));
                        continue;
                    }

                    if (centerBlade)
                    {
                        texture.SetPixel(x, y, new Color(0.97f, 0.98f, 1f, 1f));
                        continue;
                    }

                    if (nose)
                    {
                        texture.SetPixel(x, y, new Color(1f, 0.54f, 0.48f, 1f));
                        continue;
                    }

                    float shade = leftFin || rightFin ? 0.58f : 0.78f;
                    Color hullColor = Color.Lerp(new Color(0.4f, 0.74f, 1f), new Color(0.75f, 0.9f, 1f), shade);
                    texture.SetPixel(x, y, hullColor);
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D BuildBulletTexture(AmmoPowerupType type)
        {
            const int width = 96;
            const int height = 144;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color core = GetWeaponColor(type);
            Vector2 center = new Vector2(width * 0.5f, height * 0.5f);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float dx = Mathf.Abs(x - center.x);
                    float dy = Mathf.Abs(y - center.y);
                    Color pixel = Color.clear;

                    switch (type)
                    {
                        case AmmoPowerupType.Laser:
                            float laserLengthFade = Mathf.Min(Mathf.InverseLerp(2f, 18f, y), Mathf.InverseLerp(142f, 124f, y));
                            float laserCore = Mathf.InverseLerp(7f, 0f, dx);
                            float laserInner = Mathf.InverseLerp(15f, 4f, dx) * 0.72f;
                            float laserOuter = Mathf.InverseLerp(25f, 9f, dx) * 0.34f;
                            float laserAlpha = Mathf.Clamp01(Mathf.Max(laserCore, Mathf.Max(laserInner, laserOuter)) * laserLengthFade);
                            pixel = Color.Lerp(core, Color.white, Mathf.Clamp01(laserCore * 0.9f + 0.1f));
                            pixel.a = laserAlpha;
                            break;
                        case AmmoPowerupType.Plasma:
                            float plasmaDistance = Vector2.Distance(new Vector2(x, y), center);
                            float plasmaAlpha = Mathf.InverseLerp(36f, 12f, plasmaDistance);
                            pixel = Color.Lerp(core, Color.white, Mathf.InverseLerp(30f, 0f, plasmaDistance) * 0.8f);
                            pixel.a = plasmaAlpha;
                            break;
                        case AmmoPowerupType.Burst:
                            float burstShape = 48f - dy - (dx * 0.82f);
                            float burstAlpha = Mathf.Clamp01(Mathf.InverseLerp(-5f, 10f, burstShape));
                            pixel = dx <= 9f ? Color.white : Color.Lerp(core, new Color(1f, 0.62f, 0.18f), 0.35f);
                            pixel.a = burstAlpha;
                            break;
                        case AmmoPowerupType.Homing:
                            float homingBeam = Mathf.InverseLerp(13f, 2f, dx) * Mathf.Min(Mathf.InverseLerp(12f, 28f, y), Mathf.InverseLerp(136f, 118f, y));
                            float homingWing = Mathf.InverseLerp(7f, 0f, Mathf.Abs(dx - 22f)) * Mathf.InverseLerp(68f, 34f, y) * Mathf.InverseLerp(18f, 30f, y);
                            pixel = Color.Lerp(core, Color.white, Mathf.Clamp01(homingBeam * 0.65f));
                            pixel.a = Mathf.Clamp01(Mathf.Max(homingBeam, homingWing * 0.82f));
                            break;
                        case AmmoPowerupType.Wave:
                            float waveCenter = Mathf.Sin((y - center.y) * 0.12f) * 10f;
                            float waveAlpha = Mathf.InverseLerp(20f, 2f, Mathf.Abs((x - center.x) - waveCenter)) * Mathf.Min(Mathf.InverseLerp(8f, 24f, y), Mathf.InverseLerp(138f, 116f, y));
                            pixel = Color.Lerp(core, Color.white, Mathf.Clamp01(waveAlpha * 0.55f));
                            pixel.a = waveAlpha;
                            break;
                        case AmmoPowerupType.Guard:
                            float ring = Mathf.InverseLerp(8f, 0f, Mathf.Abs(Vector2.Distance(new Vector2(x, y), center) - 26f));
                            float guardBeam = Mathf.InverseLerp(10f, 1f, dx) * Mathf.InverseLerp(44f, 20f, dy);
                            pixel = Color.Lerp(core, Color.white, Mathf.Clamp01(guardBeam));
                            pixel.a = Mathf.Clamp01(Mathf.Max(ring * 0.78f, guardBeam));
                            break;
                        case AmmoPowerupType.Scatter:
                            float scatterRadius = Mathf.Lerp(5f, 18f, Mathf.InverseLerp(136f, 16f, y));
                            float scatterFade = Mathf.Min(Mathf.InverseLerp(8f, 24f, y), Mathf.InverseLerp(140f, 126f, y));
                            float scatterAlpha = Mathf.InverseLerp(scatterRadius + 5f, scatterRadius - 1f, dx) * scatterFade;
                            pixel = Color.Lerp(core, Color.white, Mathf.InverseLerp(scatterRadius, 0f, dx) * 0.75f);
                            pixel.a = scatterAlpha;
                            break;
                        case AmmoPowerupType.RapidFire:
                            float rapidLengthFade = Mathf.Min(Mathf.InverseLerp(6f, 18f, y), Mathf.InverseLerp(138f, 124f, y));
                            float rapidAlpha = Mathf.Max(Mathf.InverseLerp(7f, 0f, dx), Mathf.InverseLerp(16f, 7f, dx) * 0.32f) * rapidLengthFade;
                            pixel = Color.Lerp(core, Color.white, Mathf.InverseLerp(7f, 0f, dx) * 0.78f);
                            pixel.a = rapidAlpha;
                            break;
                        case AmmoPowerupType.Pierce:
                            float pierceLengthFade = Mathf.Min(Mathf.InverseLerp(6f, 20f, y), Mathf.InverseLerp(140f, 122f, y));
                            float pierceAlpha = Mathf.Max(Mathf.InverseLerp(9f, 0f, dx), Mathf.InverseLerp(19f, 8f, dx) * 0.38f) * pierceLengthFade;
                            pixel = Color.Lerp(core, Color.white, Mathf.InverseLerp(8f, 0f, dx) * 0.86f);
                            pixel.a = pierceAlpha;
                            break;
                        default:
                            float normalLengthFade = Mathf.Min(Mathf.InverseLerp(10f, 24f, y), Mathf.InverseLerp(134f, 116f, y));
                            float normalAlpha = Mathf.Max(Mathf.InverseLerp(11f, 1f, dx), Mathf.InverseLerp(20f, 9f, dx) * 0.26f) * normalLengthFade;
                            pixel = Color.Lerp(core, Color.white, Mathf.InverseLerp(10f, 0f, dx) * 0.7f);
                            pixel.a = normalAlpha;
                            break;
                    }

                    texture.SetPixel(x, y, pixel.a > 0.01f ? pixel : Color.clear);
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D BuildAmmoPackTexture(AmmoPowerupType type)
        {
            const int size = 128;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color core = GetWeaponColor(type);
            Vector2 center = new Vector2(size * 0.5f, size * 0.5f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Abs(x - center.x);
                    float dy = Mathf.Abs(y - center.y);
                    bool frame = Mathf.Max(dx, dy) <= 54f && Mathf.Max(dx, dy) >= 42f;
                    bool inner = Mathf.Max(dx, dy) < 42f;
                    bool glyph;

                    switch (type)
                    {
                        case AmmoPowerupType.Scatter:
                            glyph = (dy <= 22f && dx <= 5f) || (Mathf.Abs(dx - dy) <= 4f && dy <= 26f);
                            break;
                        case AmmoPowerupType.RapidFire:
                            glyph = (dx <= 5f && dy <= 30f) || (dx >= 14f && dx <= 22f && dy <= 30f);
                            break;
                        case AmmoPowerupType.Pierce:
                            glyph = dx <= 8f && dy <= 34f;
                            break;
                        case AmmoPowerupType.Laser:
                            glyph = dx <= 4f && dy <= 38f;
                            break;
                        case AmmoPowerupType.Plasma:
                            glyph = Vector2.Distance(new Vector2(x, y), center) <= 22f;
                            break;
                        case AmmoPowerupType.Burst:
                            glyph = dx + dy <= 32f;
                            break;
                        case AmmoPowerupType.Homing:
                            glyph = dy <= 28f && dx <= 16f + (Mathf.Sin(y * 0.18f) * 8f);
                            break;
                        case AmmoPowerupType.Wave:
                            glyph = Mathf.Abs(y - center.y - (Mathf.Sin((x - center.x) * 0.12f) * 14f)) <= 5f && dx <= 34f;
                            break;
                        case AmmoPowerupType.Guard:
                            glyph = Mathf.Abs(Vector2.Distance(new Vector2(x, y), center) - 22f) <= 4f;
                            break;
                        default:
                            glyph = dx <= 8f && dy <= 24f;
                            break;
                    }

                    if (glyph)
                    {
                        texture.SetPixel(x, y, Color.white);
                    }
                    else if (frame)
                    {
                        texture.SetPixel(x, y, core);
                    }
                    else if (inner)
                    {
                        texture.SetPixel(x, y, Color.Lerp(new Color(0.03f, 0.08f, 0.16f), core, 0.22f));
                    }
                    else
                    {
                        texture.SetPixel(x, y, Color.clear);
                    }
                }
            }

            texture.Apply();
            return texture;
        }

        public static Color GetWeaponColor(AmmoPowerupType type)
        {
            switch (type)
            {
                case AmmoPowerupType.Scatter:
                    return new Color(1f, 0.48f, 0.32f);
                case AmmoPowerupType.RapidFire:
                    return new Color(1f, 0.9f, 0.28f);
                case AmmoPowerupType.Pierce:
                    return new Color(0.45f, 0.95f, 1f);
                case AmmoPowerupType.Laser:
                    return new Color(0.2f, 0.82f, 1f);
                case AmmoPowerupType.Plasma:
                    return new Color(0.7f, 0.38f, 1f);
                case AmmoPowerupType.Burst:
                    return new Color(1f, 0.22f, 0.34f);
                case AmmoPowerupType.Homing:
                    return new Color(0.28f, 0.72f, 1f);
                case AmmoPowerupType.Wave:
                    return new Color(1f, 0.42f, 0.82f);
                case AmmoPowerupType.Guard:
                    return new Color(0.58f, 1f, 0.78f);
                default:
                    return new Color(1f, 0.86f, 0.32f);
            }
        }

        private static Texture2D BuildHeroFighterTexture()
        {
            const int width = 256;
            const int height = 256;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Vector2 center = new Vector2(width * 0.5f, height * 0.5f);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float dx = Mathf.Abs(x - center.x);
                    bool fuselage = dx <= 14f && y >= 30f && y <= 208f;
                    bool nose = y > 208f && y <= 246f && dx <= Mathf.Lerp(2f, 14f, Mathf.InverseLerp(246f, 208f, y));
                    bool centerBlade = dx <= 6f && y >= 50f && y <= 224f;
                    bool cockpit = dx <= 12f && y >= 144f && y <= 184f;
                    bool cockpitGlow = dx <= 6f && y >= 150f && y <= 178f;
                    bool wingRoot = y >= 100f && y <= 136f && dx <= 88f;
                    bool wingTip = y >= 76f && y <= 150f && dx > 88f && dx <= 122f && y >= 76f + ((dx - 88f) * 0.72f);
                    bool intake = y >= 104f && y <= 126f && dx >= 18f && dx <= 30f;
                    bool canard = y >= 142f && y <= 160f && dx >= 18f && dx <= 44f && y >= 142f + ((dx - 18f) * 0.36f);
                    bool rearBody = y >= 28f && y <= 78f && dx <= 34f;
                    bool tailPlane = y >= 58f && y <= 84f && dx <= 64f;
                    bool leftTail = x >= 64f && x <= 96f && y >= 48f && y <= 122f && y >= 48f + ((x - 64f) * 1.2f);
                    bool rightTail = x >= 160f && x <= 192f && y >= 48f && y <= 122f && y >= 48f + ((192f - x) * 1.2f);
                    bool engineGlow = dx >= 12f && dx <= 26f && y >= 20f && y <= 38f;
                    bool redCore = dx <= 4f && y >= 116f && y <= 132f;

                    bool filled = fuselage || nose || centerBlade || cockpit || cockpitGlow || wingRoot || wingTip || intake || canard || rearBody || tailPlane || leftTail || rightTail || engineGlow || redCore;
                    if (!filled)
                    {
                        texture.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    if (engineGlow)
                    {
                        texture.SetPixel(x, y, new Color(0.28f, 1f, 1f, 1f));
                        continue;
                    }

                    if (redCore)
                    {
                        texture.SetPixel(x, y, new Color(1f, 0.28f, 0.34f, 1f));
                        continue;
                    }

                    if (cockpitGlow)
                    {
                        texture.SetPixel(x, y, new Color(0.78f, 1f, 1f, 1f));
                        continue;
                    }

                    if (cockpit)
                    {
                        texture.SetPixel(x, y, new Color(0.06f, 0.82f, 1f, 1f));
                        continue;
                    }

                    float shade = 0.68f;
                    if (centerBlade || nose)
                    {
                        shade = 0.95f;
                    }
                    else if (wingTip || leftTail || rightTail)
                    {
                        shade = 0.58f;
                    }
                    else if (intake)
                    {
                        shade = 0.35f;
                    }

                    Color hullColor = Color.Lerp(new Color(0.06f, 0.18f, 0.52f), new Color(0.96f, 0.98f, 1f), shade);
                    texture.SetPixel(x, y, hullColor);
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D BuildEnemyInterceptorTexture()
        {
            const int width = 224;
            const int height = 224;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Vector2 center = new Vector2(width * 0.5f, height * 0.5f);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float dx = Mathf.Abs(x - center.x);
                    bool body = dx <= 13f && y >= 32f && y <= 186f;
                    bool nose = y > 186f && y <= 220f && dx <= Mathf.Lerp(2f, 13f, Mathf.InverseLerp(220f, 186f, y));
                    bool centerBlade = dx <= 5f && y >= 60f && y <= 198f;
                    bool clawWing = y >= 102f && y <= 132f && dx <= 76f;
                    bool wingSpike = y >= 88f && y <= 146f && dx > 76f && dx <= 104f && y >= 88f + ((dx - 76f) * 0.92f);
                    bool tailProng = y >= 32f && y <= 66f && dx <= 44f;
                    bool cockpit = dx <= 9f && y >= 138f && y <= 162f;
                    bool weaponPort = dx >= 26f && dx <= 34f && y >= 112f && y <= 126f;

                    bool filled = body || nose || centerBlade || clawWing || wingSpike || tailProng || cockpit || weaponPort;
                    if (!filled)
                    {
                        texture.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    if (cockpit)
                    {
                        texture.SetPixel(x, y, new Color(1f, 0.12f, 0.32f, 1f));
                        continue;
                    }

                    if (weaponPort)
                    {
                        texture.SetPixel(x, y, new Color(1f, 0.04f, 0.14f, 1f));
                        continue;
                    }

                    float shade = wingSpike ? 0.42f : 0.72f;
                    if (centerBlade || nose)
                    {
                        shade = 0.88f;
                    }

                    Color hullColor = Color.Lerp(new Color(0.06f, 0.02f, 0.08f), new Color(0.58f, 0.14f, 0.26f), shade);
                    texture.SetPixel(x, y, hullColor);
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D BuildEliteInterceptorTexture()
        {
            const int width = 240;
            const int height = 240;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Vector2 center = new Vector2(width * 0.5f, height * 0.5f);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float dx = Mathf.Abs(x - center.x);
                    bool body = dx <= 16f && y >= 30f && y <= 198f;
                    bool nose = y > 198f && y <= 236f && dx <= Mathf.Lerp(3f, 16f, Mathf.InverseLerp(236f, 198f, y));
                    bool centerBlade = dx <= 7f && y >= 52f && y <= 210f;
                    bool mainWing = y >= 96f && y <= 140f && dx <= 92f;
                    bool wingPike = y >= 80f && y <= 154f && dx > 92f && dx <= 118f && y >= 80f + ((dx - 92f) * 0.78f);
                    bool sideFin = y >= 54f && y <= 108f && dx >= 44f && dx <= 62f;
                    bool tailBase = y >= 30f && y <= 72f && dx <= 56f;
                    bool cockpit = dx <= 11f && y >= 146f && y <= 174f;
                    bool coreGlow = dx <= 5f && y >= 118f && y <= 134f;

                    bool filled = body || nose || centerBlade || mainWing || wingPike || sideFin || tailBase || cockpit || coreGlow;
                    if (!filled)
                    {
                        texture.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    if (coreGlow)
                    {
                        texture.SetPixel(x, y, new Color(1f, 0.72f, 0.04f, 1f));
                        continue;
                    }

                    if (cockpit)
                    {
                        texture.SetPixel(x, y, new Color(1f, 0.92f, 0.22f, 1f));
                        continue;
                    }

                    float shade = 0.66f;
                    if (centerBlade || nose)
                    {
                        shade = 0.9f;
                    }
                    else if (wingPike || sideFin)
                    {
                        shade = 0.45f;
                    }

                    Color hullColor = Color.Lerp(new Color(0.04f, 0.03f, 0.08f), new Color(0.56f, 0.28f, 0.04f), shade);
                    texture.SetPixel(x, y, hullColor);
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D BuildBossFlagshipTexture()
        {
            const int width = 320;
            const int height = 256;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Vector2 center = new Vector2(width * 0.5f, height * 0.5f);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float dx = Mathf.Abs(x - center.x);
                    bool coreBody = dx <= 26f && y >= 32f && y <= 210f;
                    bool nose = y > 210f && y <= 248f && dx <= Mathf.Lerp(6f, 26f, Mathf.InverseLerp(248f, 210f, y));
                    bool bridge = dx <= 16f && y >= 152f && y <= 184f;
                    bool reactor = dx <= 8f && y >= 118f && y <= 138f;
                    bool leftHull = x >= 24f && x <= 122f && y >= 74f && y <= 168f && y <= 168f - ((x - 24f) * 0.24f);
                    bool rightHull = x >= 198f && x <= 296f && y >= 74f && y <= 168f && y <= 168f - ((296f - x) * 0.24f);
                    bool leftShoulder = x >= 58f && x <= 116f && y >= 120f && y <= 208f && y >= 120f + ((x - 58f) * 0.52f);
                    bool rightShoulder = x >= 204f && x <= 262f && y >= 120f && y <= 208f && y >= 120f + ((262f - x) * 0.52f);
                    bool leftCannon = x >= 12f && x <= 48f && y >= 120f && y <= 150f;
                    bool rightCannon = x >= 272f && x <= 308f && y >= 120f && y <= 150f;
                    bool lowerWing = y >= 46f && y <= 84f && dx <= 110f;

                    bool filled = coreBody || nose || bridge || reactor || leftHull || rightHull || leftShoulder || rightShoulder || leftCannon || rightCannon || lowerWing;
                    if (!filled)
                    {
                        texture.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    if (reactor)
                    {
                        texture.SetPixel(x, y, new Color(1f, 0.04f, 0.22f, 1f));
                        continue;
                    }

                    if (bridge)
                    {
                        texture.SetPixel(x, y, new Color(0.58f, 1f, 1f, 1f));
                        continue;
                    }

                    float shade = 0.48f;
                    if (nose || coreBody)
                    {
                        shade = 0.76f;
                    }
                    else if (leftCannon || rightCannon)
                    {
                        shade = 0.9f;
                    }

                    Color hullColor = Color.Lerp(new Color(0.03f, 0.01f, 0.05f), new Color(0.44f, 0.08f, 0.28f), shade);
                    texture.SetPixel(x, y, hullColor);
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D BuildSkyBackgroundTexture()
        {
            const int width = 512;
            const int height = 1024;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

            for (int y = 0; y < height; y++)
            {
                float v = y / (float)(height - 1);
                Color baseColor = Color.Lerp(new Color(0.01f, 0.02f, 0.06f), new Color(0.02f, 0.04f, 0.12f), v);
                for (int x = 0; x < width; x++)
                {
                    float u = x / (float)(width - 1);
                    float hash = Mathf.Repeat(Mathf.Abs(Mathf.Sin(x * 127.1f + y * 311.7f) * 43758.5453f), 1.0f);
                    if (hash > 0.986f)
                    {
                        float brightness = Mathf.InverseLerp(0.986f, 1.0f, hash);
                        float ch = Mathf.Repeat(Mathf.Abs(Mathf.Sin(x * 94.3f + y * 232.1f) * 37281.9f), 1.0f);
                        Color star = ch > 0.7f
                            ? new Color(0.72f, 0.92f, 1f, brightness)
                            : ch > 0.4f ? new Color(1f, 0.95f, 0.80f, brightness)
                            : new Color(1f, 1f, 1f, brightness);
                        texture.SetPixel(x, y, star);
                    }
                    else
                    {
                        float nebula = Mathf.PerlinNoise(u * 1.8f + 2.4f, v * 2.6f + 1.1f) * 0.06f;
                        texture.SetPixel(x, y, Color.Lerp(baseColor, new Color(0.08f, 0.04f, 0.18f), nebula));
                    }
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D BuildCloudLayerTexture()
        {
            const int width = 512;
            const int height = 1024;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

            for (int y = 0; y < height; y++)
            {
                float v = y / (float)(height - 1);
                for (int x = 0; x < width; x++)
                {
                    float u = x / (float)(width - 1);
                    float hash = Mathf.Repeat(Mathf.Abs(Mathf.Sin(x * 213.1f + y * 471.7f) * 73219.4f), 1.0f);
                    if (hash > 0.976f)
                    {
                        float brightness = Mathf.InverseLerp(0.976f, 1.0f, hash);
                        float ch = Mathf.Repeat(Mathf.Abs(Mathf.Sin(x * 184.7f + y * 96.3f) * 51847.2f), 1.0f);
                        Color star = ch > 0.6f
                            ? new Color(0.5f, 0.95f, 1f, brightness * 0.82f)
                            : new Color(1f, 1f, 1f, brightness * 0.72f);
                        texture.SetPixel(x, y, star);
                    }
                    else
                    {
                        float nebula = Mathf.PerlinNoise((u * 3.2f) + 1.4f, (v * 5.8f) + 2.7f);
                        float alpha = Mathf.InverseLerp(0.68f, 0.88f, nebula) * 0.12f;
                        texture.SetPixel(x, y, new Color(0.28f, 0.14f, 0.58f, alpha));
                    }
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D BuildCloudStreakTexture()
        {
            const int width = 512;
            const int height = 1024;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

            for (int y = 0; y < height; y++)
            {
                float v = y / (float)(height - 1);
                for (int x = 0; x < width; x++)
                {
                    float u = x / (float)(width - 1);
                    float colNoise = Mathf.PerlinNoise(u * 64f, 0.5f);
                    float lenNoise = Mathf.PerlinNoise(u * 64f, v * 2.4f + 1.8f);
                    float hasStreak = colNoise > 0.74f ? 1f : 0f;
                    float brightness = lenNoise > 0.46f ? Mathf.InverseLerp(0.46f, 0.82f, lenNoise) : 0f;
                    float alpha = hasStreak * brightness * 0.44f;
                    texture.SetPixel(x, y, new Color(0.72f, 0.96f, 1f, alpha));
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D BuildBattlefieldBackgroundTexture()
        {
            const int width = 512;
            const int height = 1024;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

            for (int y = 0; y < height; y++)
            {
                float v = y / (float)(height - 1);
                Color color;

                if (v > 0.52f)
                {
                    float skyT = Mathf.InverseLerp(0.52f, 1f, v);
                    color = Color.Lerp(new Color(0.11f, 0.18f, 0.34f), new Color(0.36f, 0.52f, 0.88f), skyT);
                }
                else if (v > 0.32f)
                {
                    float seaT = Mathf.InverseLerp(0.32f, 0.52f, v);
                    color = Color.Lerp(new Color(0.04f, 0.1f, 0.2f), new Color(0.12f, 0.24f, 0.44f), seaT);
                }
                else
                {
                    float deckT = Mathf.InverseLerp(0f, 0.32f, v);
                    color = Color.Lerp(new Color(0.05f, 0.06f, 0.12f), new Color(0.14f, 0.14f, 0.22f), deckT);
                }

                for (int x = 0; x < width; x++)
                {
                    float u = x / (float)(width - 1);
                    float cloudNoise = Mathf.PerlinNoise((u * 4.1f) + 0.3f, (v * 7.4f) + 0.6f);
                    float fineNoise = Mathf.PerlinNoise((u * 17.5f) + 0.9f, (v * 26.1f) + 0.4f);
                    Color pixel = color;

                    if (v > 0.58f && cloudNoise > 0.62f)
                    {
                        float cloudStrength = Mathf.InverseLerp(0.62f, 0.9f, cloudNoise) * 0.45f;
                        pixel = Color.Lerp(pixel, new Color(0.72f, 0.88f, 1f), cloudStrength);
                    }

                    if (v > 0.32f && v < 0.52f)
                    {
                        pixel = Color.Lerp(pixel, new Color(0.48f, 0.14f, 0.48f), fineNoise * 0.09f);
                    }

                    if (v <= 0.32f)
                    {
                        float deckStripe = Mathf.Abs((u - 0.5f) * 2f);
                        if (deckStripe < Mathf.Lerp(0.18f, 0.04f, v / 0.32f))
                        {
                            pixel = Color.Lerp(pixel, new Color(0.22f, 0.78f, 1f), 0.4f);
                        }

                        if (u > 0.68f && u < 0.73f && v > 0.08f && v < 0.26f)
                        {
                            pixel = Color.Lerp(pixel, new Color(1f, 0.24f, 0.34f), 0.72f);
                        }

                        pixel = Color.Lerp(pixel, new Color(0.02f, 0.04f, 0.08f), fineNoise * 0.16f);
                    }

                    texture.SetPixel(x, y, pixel);
                }
            }

            texture.Apply();
            return texture;
        }
    }
}
