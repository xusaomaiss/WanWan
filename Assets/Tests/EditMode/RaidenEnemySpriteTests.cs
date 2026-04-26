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
    }
}
