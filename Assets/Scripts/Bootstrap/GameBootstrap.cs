using System.Collections;
using UnityEngine;

namespace Wanwan.Runtime
{
    public class GameBootstrap : MonoBehaviour
    {
        private const float PlayerShipWorldSize = 0.5f;
        private const float PlayerLaunchStartInset = 1.25f;
        private const float PlayerLaunchTargetInset = 4.15f;

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
            float leftBound = -horizontalExtent + 0.9f;
            float rightBound = horizontalExtent - 0.9f;

            CreateScrollingBattlefieldBackdrop(orthographicSize, horizontalExtent, stage);
            EffectsController effects = new GameObject("EffectsController").AddComponent<EffectsController>();
            effects.Initialize(cameraComponent);

            UIController ui = new GameObject("UIController").AddComponent<UIController>();
            GameManager manager = new GameObject("GameManager").AddComponent<GameManager>();
            BlockSpawner spawner = new GameObject("BlockSpawner").AddComponent<BlockSpawner>();
            CreateCarrierDeck(orthographicSize, horizontalExtent, bottomBound + 0.45f);
            PlayerController player = CreatePlayer(leftBound, rightBound, bottomBound + PlayerLaunchStartInset);
            CreateBaseBoundary(leftBound, rightBound, bottomBound + 0.85f);

            manager.Initialize(ui, effects, spawner, player, leftBound, rightBound, topBound, bottomBound);
            player.Initialize(manager, effects, cameraComponent, leftBound, rightBound);
            spawner.Initialize(manager, effects, cameraComponent, leftBound, rightBound, topBound);
            manager.BeginLaunchSequence();
            StartCoroutine(PlayCarrierLaunch(player.transform, bottomBound + PlayerLaunchTargetInset, manager));
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

        private static void CreateScrollingBattlefieldBackdrop(float orthographicSize, float horizontalExtent, StageDefinition stage)
        {
            float targetWidth = (horizontalExtent * 2f) + 3f;
            float targetHeight = (orthographicSize * 2f) + 3f;
            Color stageTint = stage.BackgroundColor;
            Color cloudTint = Color.Lerp(Color.white, stage.AccentColor, 0.16f);
            float speed = stage.BackgroundSpeedMultiplier;
            Sprite stageBackground = RuntimeSpriteFactory.GetRaidenStageBackgroundSprite(stage.Number);
            Color backgroundTint = Color.Lerp(Color.white, stageTint * 2.2f, 0.08f);
            CreateBackgroundLayer("StageBackgroundA", stageBackground, -62, 0.62f * speed, targetWidth, targetHeight, 0f, backgroundTint);
            CreateBackgroundLayer("StageBackgroundB", stageBackground, -62, 0.62f * speed, targetWidth, targetHeight, targetHeight, backgroundTint);
            CreateBackgroundLayer("CloudLayerA", RuntimeSpriteFactory.GetCloudLayerSprite(), -55, 0.9f * speed, targetWidth, targetHeight, 0f, cloudTint);
            CreateBackgroundLayer("CloudLayerB", RuntimeSpriteFactory.GetCloudLayerSprite(), -55, 0.9f * speed, targetWidth, targetHeight, targetHeight, cloudTint);
            CreateBackgroundLayer("CloudStreakA", RuntimeSpriteFactory.GetCloudStreakSprite(), -54, 1.8f * speed, targetWidth, targetHeight, 0f, Color.Lerp(Color.white, stage.AccentColor, 0.24f));
            CreateBackgroundLayer("CloudStreakB", RuntimeSpriteFactory.GetCloudStreakSprite(), -54, 1.8f * speed, targetWidth, targetHeight, targetHeight, Color.Lerp(Color.white, stage.AccentColor, 0.24f));
        }

        private static void CreateBackgroundLayer(string name, Sprite sprite, int sortingOrder, float speed, float targetWidth, float targetHeight, float yOffset)
        {
            CreateBackgroundLayer(name, sprite, sortingOrder, speed, targetWidth, targetHeight, yOffset, Color.white);
        }

        private static void CreateBackgroundLayer(string name, Sprite sprite, int sortingOrder, float speed, float targetWidth, float targetHeight, float yOffset, Color color)
        {
            GameObject backgroundObject = new GameObject(name);
            backgroundObject.transform.position = new Vector3(0f, yOffset, 8f);

            SpriteRenderer renderer = backgroundObject.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;

            Vector2 spriteSize = renderer.sprite.bounds.size;
            backgroundObject.transform.localScale = new Vector3(targetWidth / spriteSize.x, targetHeight / spriteSize.y, 1f);
            backgroundObject.AddComponent<ScrollingBackgroundLayer>().Initialize(speed, targetHeight);
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

        private static IEnumerator PlayCarrierLaunch(Transform playerTransform, float targetY, GameManager manager)
        {
            Vector3 start = playerTransform.position;
            Vector3 end = new Vector3(start.x, targetY, start.z);
            float duration = 1.85f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));
                playerTransform.position = Vector3.Lerp(start, end, t);
                yield return null;
            }

            playerTransform.position = end;
            yield return new WaitForSeconds(1f);
            manager.CompleteLaunchSequence();
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

        private static void CreateBaseBoundary(float leftBound, float rightBound, float y)
        {
            GameObject boundary = new GameObject("BaseBoundary");
            boundary.transform.position = new Vector3(0f, y, 0f);
            BoxCollider2D collider = boundary.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2((rightBound - leftBound) + 1f, 0.8f);
            boundary.AddComponent<BaseBoundary>();
        }
    }
}
