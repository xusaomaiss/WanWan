using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class PickupPresentationTests
    {
        [Test]
        public void PickupPresentation_KeepsBombReadableWithoutOversizing()
        {
            Assert.That(PickupPresentation.BombVisualScale, Is.EqualTo(0.92f).Within(0.001f));
            Assert.That(PickupPresentation.AmmoPackVisualScale, Is.LessThan(0.8f));
            Assert.That(PickupPresentation.PowerCapsuleVisualScale, Is.EqualTo(PickupPresentation.AmmoPackVisualScale).Within(0.001f));
            Assert.That(PickupPresentation.HealthPickupVisualScale, Is.EqualTo(PickupPresentation.AmmoPackVisualScale).Within(0.001f));
            Assert.That(PickupPresentation.BombColliderRadius, Is.GreaterThan(PickupPresentation.AmmoPackColliderRadius));
            Assert.That(PickupPresentation.BombColliderRadius, Is.LessThan(0.7f));
        }

        [Test]
        public void BombPickupSprite_HasLargeBrightReadableBody()
        {
            Texture2D texture = RuntimeSpriteFactory.GetBombPickupSprite().texture;
            int visiblePixels = 0;
            int brightPixels = 0;

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    Color pixel = texture.GetPixel(x, y);
                    if (pixel.a <= 0.05f)
                    {
                        continue;
                    }

                    visiblePixels++;
                    if (pixel.r + pixel.g + pixel.b > 2.2f)
                    {
                        brightPixels++;
                    }
                }
            }

            Assert.That(visiblePixels, Is.GreaterThan(4800));
            Assert.That(brightPixels, Is.GreaterThan(260));
        }

        [Test]
        public void SciFiBombResource_HasTransparentBackground()
        {
            Texture2D texture = Resources.Load<Texture2D>(SciFiResourcePaths.PickupBomb);
            TextureImporter importer = AssetImporter.GetAtPath("Assets/Resources/SciFi/Pickups/bomb_pickup.png") as TextureImporter;

            Assert.That(texture, Is.Not.Null);
            Assert.That(importer, Is.Not.Null);
            Assert.That(importer.DoesSourceTextureHaveAlpha(), Is.True);
        }
    }
}
