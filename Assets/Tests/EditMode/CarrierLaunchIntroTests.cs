using NUnit.Framework;
using UnityEngine;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class CarrierLaunchIntroTests
    {
        [Test]
        public void GameManager_StartsInIntroAndOnlyPlaysAfterIntroCompletes()
        {
            GameManager manager = new GameObject("GameManager").AddComponent<GameManager>();

            Assert.That(manager.CurrentState, Is.EqualTo(GameFlowState.Intro));
            Assert.That(manager.IsPlaying, Is.False);

            manager.CompleteIntro();

            Assert.That(manager.CurrentState, Is.EqualTo(GameFlowState.Playing));
            Assert.That(manager.IsPlaying, Is.True);

            Object.DestroyImmediate(manager.gameObject);
        }

        [Test]
        public void CarrierLaunchIntroConfig_DefaultDurationStaysInArcadeIntroRange()
        {
            CarrierLaunchIntroConfig config = CarrierLaunchIntroConfig.Default;

            Assert.That(config.HoldSeconds, Is.EqualTo(0.35f).Within(0.001f));
            Assert.That(config.TotalDurationSeconds, Is.InRange(3f, 5f));
            Assert.That(config.TotalDurationSeconds, Is.EqualTo(4.2f).Within(0.001f));
            Assert.That(config.ExhaustIntensity, Is.GreaterThan(0f));
            Assert.That(config.MaxSpeed, Is.GreaterThan(config.InitialSpeed));
            Assert.That(config.SkipInputGraceSeconds, Is.GreaterThanOrEqualTo(0.2f));
        }

        [Test]
        public void CarrierLaunchIntroController_SkipCompletesIntroAndResetsPlayerPosition()
        {
            GameManager manager = new GameObject("GameManager").AddComponent<GameManager>();
            Transform player = new GameObject("Player").transform;
            CarrierLaunchIntroController intro = new GameObject("Intro").AddComponent<CarrierLaunchIntroController>();
            CarrierLaunchIntroConfig config = CarrierLaunchIntroConfig.Default;
            Vector3 gameplayPosition = new Vector3(0f, -5f, 0f);

            intro.Initialize(manager, player, gameplayPosition, config, new ScrollingBackgroundLayer[0]);
            player.position = new Vector3(0f, -8f, 0f);

            intro.SkipIntro();

            Assert.That(manager.CurrentState, Is.EqualTo(GameFlowState.Playing));
            Assert.That(player.position, Is.EqualTo(gameplayPosition));

            Object.DestroyImmediate(intro.gameObject);
            Object.DestroyImmediate(player.gameObject);
            Object.DestroyImmediate(manager.gameObject);
        }
    }
}
