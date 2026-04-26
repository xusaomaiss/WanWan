using NUnit.Framework;
using UnityEngine;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class RaidenEnemySpriteTests
    {
        [Test]
        public void EnemyInterceptorSprite_UsesRaidenEnemyJetResource()
        {
            Texture2D resource = Resources.Load<Texture2D>(RuntimeSpriteFactory.RaidenEnemyJetResourcePath);

            Assert.That(resource, Is.Not.Null);
            Assert.That(resource.width, Is.EqualTo(128));
            Assert.That(resource.height, Is.EqualTo(128));
            Assert.That(RuntimeSpriteFactory.GetEnemyInterceptorSprite().texture, Is.SameAs(resource));
        }

        [Test]
        public void AdvancedEnemySprites_UseAiGeneratedResources()
        {
            Texture2D toughResource = Resources.Load<Texture2D>(RuntimeSpriteFactory.ToughEnemyResourcePath);
            Texture2D eliteResource = Resources.Load<Texture2D>(RuntimeSpriteFactory.EliteEnemyResourcePath);
            Texture2D bossResource = Resources.Load<Texture2D>(RuntimeSpriteFactory.BossFlagshipResourcePath);

            Assert.That(toughResource, Is.Not.Null);
            Assert.That(eliteResource, Is.Not.Null);
            Assert.That(bossResource, Is.Not.Null);
            Assert.That(RuntimeSpriteFactory.GetToughInterceptorSprite().texture, Is.SameAs(toughResource));
            Assert.That(RuntimeSpriteFactory.GetEliteInterceptorSprite().texture, Is.SameAs(eliteResource));
            Assert.That(RuntimeSpriteFactory.GetBossFlagshipSprite().texture, Is.SameAs(bossResource));
        }

        [Test]
        public void CoinSprite_UsesAiGeneratedPickupResource()
        {
            Texture2D resource = Resources.Load<Texture2D>(RuntimeSpriteFactory.CoinResourcePath);

            Assert.That(resource, Is.Not.Null);
            Assert.That(resource.width, Is.EqualTo(128));
            Assert.That(resource.height, Is.EqualTo(128));
            Assert.That(RuntimeSpriteFactory.GetCoinSprite().texture, Is.SameAs(resource));
        }
    }
}
