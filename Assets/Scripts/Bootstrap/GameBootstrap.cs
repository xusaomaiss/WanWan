using System.Collections.Generic;
using UnityEngine;

namespace Wanwan.Runtime
{
    public class GameBootstrap : MonoBehaviour
    {
        private const float PlayerShipWorldSize = 1f;

        private void Awake()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            StageDefinition stage = SessionState.CurrentStage;
            Camera cameraComponent = EnsureCamera(stage.BackgroundColor);
            float orthographicSize = 9f;
            cameraComponent.orthographicSize = orthographicSize;

            float topBound = orthographicSize + 1f;
            float bottomBound = -orthographicSize - 1f;
            float horizontalExtent = orthographicSize * cameraComponent.aspect;
            float leftBound = -horizontalExtent;
            float rightBound = horizontalExtent;
            CarrierLaunchIntroConfig introConfig = CarrierLaunchIntroConfig.Default;
            Vector3 gameplayPlayerPosition = new Vector3(0f, bottomBound + introConfig.GameplayStartYInset, 0f);

            ScrollingBackgroundLayer[] backgroundLayers = CreateScrollingBattlefieldBackdrop(orthographicSize, horizontalExtent, stage);
            EffectsController effects = new GameObject("EffectsController").AddComponent<EffectsController>();
            effects.Initialize(cameraComponent);

            UIController ui = new GameObject("UIController").AddComponent<UIController>();
            GameManager manager = new GameObject("GameManager").AddComponent<GameManager>();
            BlockSpawner spawner = new GameObject("BlockSpawner").AddComponent<BlockSpawner>();
            CreateCarrierDeck(orthographicSize, horizontalExtent, bottomBound + 0.45f);
            PlayerController player = CreatePlayer(leftBound, rightBound, bottomBound + introConfig.StartYInset);
            SpriteRenderer introBackdrop = CreateIntroCinematicBackdrop(orthographicSize, horizontalExtent);

            manager.Initialize(ui, effects, spawner, player, leftBound, rightBound, topBound, bottomBound);
            player.Initialize(manager, effects, cameraComponent, leftBound, rightBound, bottomBound, topBound);
            spawner.Initialize(manager, effects, cameraComponent, leftBound, rightBound, topBound);
            manager.BeginIntro();

            CarrierLaunchIntroController intro = new GameObject("CarrierLaunchIntroController").AddComponent<CarrierLaunchIntroController>();
            intro.Initialize(manager, player.transform, gameplayPlayerPosition, introConfig, backgroundLayers, introBackdrop, cameraComponent);
            intro.Play();
        }

        private static Camera EnsureCamera(Color background)
        {
            if (Camera.main != null)
            {
                Camera.main.backgroundColor = background;
                Camera.main.orthographic = true;
                return Camera.main;
            }

            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            Camera cameraComponent = cameraObject.AddComponent<Camera>();
            cameraComponent.orthographic = true;
            cameraComponent.backgroundColor = background;
            cameraObject.AddComponent<AudioListener>();
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
            return cameraComponent;
        }

        private static ScrollingBackgroundLayer[] CreateScrollingBattlefieldBackdrop(float orthographicSize, float horizontalExtent, StageDefinition stage)
        {
            List<ScrollingBackgroundLayer> layers = new List<ScrollingBackgroundLayer>();
            float targetWidth = (horizontalExtent * 2f) + 3f;
            float targetHeight = (orthographicSize * 2f) + 3f;
            Color stageTint = stage.BackgroundColor;
            Color cloudTint = Color.Lerp(Color.white, stage.AccentColor, 0.16f);
            float speed = stage.BackgroundSpeedMultiplier;
            Sprite stageBackground = RuntimeSpriteFactory.GetRaidenStageBackgroundSprite(stage.Number);
            Color backgroundTint = Color.Lerp(Color.white, stageTint * 2.2f, 0.08f);
            layers.Add(CreateBackgroundLayer("StageBackgroundA", stageBackground, -62, 0.62f * speed, targetWidth, targetHeight, 0f, backgroundTint));
            layers.Add(CreateBackgroundLayer("StageBackgroundB", stageBackground, -62, 0.62f * speed, targetWidth, targetHeight, targetHeight, backgroundTint));
            layers.Add(CreateBackgroundLayer("CloudLayerA", RuntimeSpriteFactory.GetCloudLayerSprite(), -55, 0.9f * speed, targetWidth, targetHeight, 0f, cloudTint));
            layers.Add(CreateBackgroundLayer("CloudLayerB", RuntimeSpriteFactory.GetCloudLayerSprite(), -55, 0.9f * speed, targetWidth, targetHeight, targetHeight, cloudTint));
            layers.Add(CreateBackgroundLayer("CloudStreakA", RuntimeSpriteFactory.GetCloudStreakSprite(), -54, 1.8f * speed, targetWidth, targetHeight, 0f, Color.Lerp(Color.white, stage.AccentColor, 0.24f)));
            layers.Add(CreateBackgroundLayer("CloudStreakB", RuntimeSpriteFactory.GetCloudStreakSprite(), -54, 1.8f * speed, targetWidth, targetHeight, targetHeight, Color.Lerp(Color.white, stage.AccentColor, 0.24f)));
            return layers.ToArray();
        }

        private static ScrollingBackgroundLayer CreateBackgroundLayer(string name, Sprite sprite, int sortingOrder, float speed, float targetWidth, float targetHeight, float yOffset)
        {
            return CreateBackgroundLayer(name, sprite, sortingOrder, speed, targetWidth, targetHeight, yOffset, Color.white);
        }

        private static ScrollingBackgroundLayer CreateBackgroundLayer(string name, Sprite sprite, int sortingOrder, float speed, float targetWidth, float targetHeight, float yOffset, Color color)
        {
            GameObject backgroundObject = new GameObject(name);
            backgroundObject.transform.position = new Vector3(0f, yOffset, 8f);

            SpriteRenderer renderer = backgroundObject.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;

            Vector2 spriteSize = renderer.sprite.bounds.size;
            backgroundObject.transform.localScale = new Vector3(targetWidth / spriteSize.x, targetHeight / spriteSize.y, 1f);
            ScrollingBackgroundLayer scrollingLayer = backgroundObject.AddComponent<ScrollingBackgroundLayer>();
            scrollingLayer.Initialize(speed, targetHeight);
            return scrollingLayer;
        }

        private static void CreateCarrierDeck(float orthographicSize, float horizontalExtent, float y)
        {
            GameObject deckObject = new GameObject("CarrierDeck");
            deckObject.transform.position = new Vector3(0f, y, 4f);

            SpriteRenderer renderer = deckObject.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.GetCarrierDeckSprite();
            renderer.sortingOrder = -20;

            Vector2 spriteSize = renderer.sprite.bounds.size;
            float targetWidth = (horizontalExtent * 2f) + 2f;
            float targetHeight = orthographicSize * 0.42f;
            deckObject.transform.localScale = new Vector3(targetWidth / spriteSize.x, targetHeight / spriteSize.y, 1f);
        }

        private static SpriteRenderer CreateIntroCinematicBackdrop(float orthographicSize, float horizontalExtent)
        {
            GameObject backdropObject = new GameObject("LaunchWeatherIntroBackdrop");
            backdropObject.transform.position = new Vector3(0f, 0f, 7f);

            SpriteRenderer renderer = backdropObject.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.GetLaunchWeatherIntroSprite();
            renderer.sortingOrder = -59;
            renderer.color = Color.white;

            Vector2 spriteSize = renderer.sprite.bounds.size;
            float targetWidth = (horizontalExtent * 2f) + 3.5f;
            float targetHeight = (orthographicSize * 2f) + 3.5f;
            float scale = Mathf.Max(targetWidth / spriteSize.x, targetHeight / spriteSize.y);
            backdropObject.transform.localScale = new Vector3(scale, scale, 1f);
            return renderer;
        }

        private static PlayerController CreatePlayer(float leftBound, float rightBound, float y)
        {
            GameObject playerObject = new GameObject("Player");
            playerObject.transform.position = new Vector3(0f, y, 0f);

            SpriteRenderer renderer = playerObject.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.GetRaidenFighterJetSprite();
            renderer.color = Color.Lerp(Color.white, ShipDefinition.Get(SessionState.SelectedShip).AccentColor, 0.45f);
            renderer.sortingOrder = 12;
            Vector2 spriteSize = renderer.sprite.bounds.size;
            playerObject.transform.localScale = new Vector3(PlayerShipWorldSize / spriteSize.x, PlayerShipWorldSize / spriteSize.y, 1f);

            BoxCollider2D collider = playerObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.38f, 0.5f);
            collider.offset = new Vector2(0f, -0.04f);

            PlayerController playerController = playerObject.AddComponent<PlayerController>();
            return playerController;
        }

    }
}
