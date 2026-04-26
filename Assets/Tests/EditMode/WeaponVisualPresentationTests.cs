using NUnit.Framework;
using UnityEngine;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class WeaponVisualPresentationTests
    {
        [Test]
        public void LaserBulletSprite_HasWideBlueWhiteBeamWithSoftGlow()
        {
            Sprite sprite = RuntimeSpriteFactory.GetBulletSprite(AmmoPowerupType.Laser);
            Texture2D resource = Resources.Load<Texture2D>(RuntimeSpriteFactory.BulletLaserArcadeResourcePath);
            Texture2D texture = sprite.texture;
            int centerY = texture.height / 2;
            int visiblePixels = 0;
            int softGlowPixels = 0;

            Assert.That(texture, Is.SameAs(resource));
            for (int x = 0; x < texture.width; x++)
            {
                Color pixel = texture.GetPixel(x, centerY);
                if (pixel.a > 0.02f)
                {
                    visiblePixels++;
                }

                if (pixel.a > 0.02f && pixel.a < 0.85f)
                {
                    softGlowPixels++;
                }
            }

            Assert.That(visiblePixels, Is.GreaterThanOrEqualTo(28));
            Assert.That(softGlowPixels, Is.GreaterThanOrEqualTo(12));
            Assert.That(RuntimeSpriteFactory.GetWeaponColor(AmmoPowerupType.Laser).b, Is.GreaterThan(0.9f));
        }

        [Test]
        public void ShotPresentation_MakesPlayerBulletsLargerAndLaserBeamsTaller()
        {
            Vector3 normalScale = WeaponShotPresentation.GetPlayerScale(AmmoPowerupType.Normal, false);
            Vector3 laserScale = WeaponShotPresentation.GetPlayerScale(AmmoPowerupType.Laser, true);

            Assert.That(normalScale.x, Is.GreaterThanOrEqualTo(0.26f));
            Assert.That(normalScale.y, Is.GreaterThanOrEqualTo(0.46f));
            Assert.That(laserScale.x, Is.GreaterThanOrEqualTo(0.2f));
            Assert.That(laserScale.y, Is.GreaterThanOrEqualTo(0.78f));
            Assert.That(WeaponShotPresentation.GetLaserSideOffset(), Is.GreaterThanOrEqualTo(0.34f));
        }
    }
}
