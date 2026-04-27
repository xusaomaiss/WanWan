using NUnit.Framework;
using UnityEngine;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    [TestFixture]
    public class EffectsControllerExplosionTests
    {
        [Test]
        public void ExplosionMethods_ExistOnEffectsController()
        {
            var type = typeof(EffectsController);
            Assert.IsNotNull(type.GetMethod("PlaySmallBurst"));
            Assert.IsNotNull(type.GetMethod("PlayEliteBurst"));
        }

        [Test]
        public void ExplosionMethods_HaveCorrectSignatures()
        {
            var type = typeof(EffectsController);
            var small = type.GetMethod("PlaySmallBurst");
            var elite = type.GetMethod("PlayEliteBurst");

            Assert.AreEqual(typeof(void), small.ReturnType);
            Assert.AreEqual(typeof(void), elite.ReturnType);

            var smallParams = small.GetParameters();
            Assert.AreEqual(2, smallParams.Length);
            Assert.AreEqual(typeof(Vector3), smallParams[0].ParameterType);
            Assert.AreEqual(typeof(Color), smallParams[1].ParameterType);

            var eliteParams = elite.GetParameters();
            Assert.AreEqual(2, eliteParams.Length);
            Assert.AreEqual(typeof(Vector3), eliteParams[0].ParameterType);
            Assert.AreEqual(typeof(Color), eliteParams[1].ParameterType);
        }
    }
}
