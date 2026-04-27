using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    [TestFixture]
    public class WeatherParticleSystemTests
    {
        [Test]
        public void StageWeather_AllTypesDefined()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(StageWeather), StageWeather.None));
            Assert.IsTrue(System.Enum.IsDefined(typeof(StageWeather), StageWeather.Rain));
            Assert.IsTrue(System.Enum.IsDefined(typeof(StageWeather), StageWeather.Spores));
            Assert.IsTrue(System.Enum.IsDefined(typeof(StageWeather), StageWeather.Sand));
        }

        [Test]
        public void StageDefinition_HasWeatherProperty()
        {
            var prop = typeof(StageDefinition).GetProperty("Weather");
            Assert.IsNotNull(prop);
            Assert.AreEqual(typeof(StageWeather), prop.PropertyType);
        }

        [Test]
        public void WeatherParticleSystem_Exists()
        {
            var type = typeof(WeatherParticleSystem);
            Assert.IsNotNull(type);
            var method = type.GetMethod("Initialize");
            Assert.IsNotNull(method);
        }

        [Test]
        public void StageCatalog_Stage3_HasRainWeather()
        {
            var stage = StageCatalog.GetStage(2);
            Assert.AreEqual(StageWeather.Rain, stage.Weather);
        }

        [Test]
        public void StageCatalog_Stage6_HasSporesWeather()
        {
            var stage = StageCatalog.GetStage(5);
            Assert.AreEqual(StageWeather.Spores, stage.Weather);
        }

        [Test]
        public void StageCatalog_Stage1_HasNoWeather()
        {
            var stage = StageCatalog.GetStage(0);
            Assert.AreEqual(StageWeather.None, stage.Weather);
        }
    }
}
