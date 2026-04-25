using UnityEngine;
using UnityEngine.UI;

namespace Wanwan.Runtime
{
    public class MenuBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            EnsureCamera(new Color(0.02f, 0.04f, 0.11f));
            BuildMenu();
        }

        private static void EnsureCamera(Color background)
        {
            if (Camera.main != null)
            {
                Camera.main.backgroundColor = background;
                Camera.main.orthographic = true;
                return;
            }

            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            Camera cameraComponent = cameraObject.AddComponent<Camera>();
            cameraComponent.orthographic = true;
            cameraComponent.backgroundColor = background;
            cameraObject.AddComponent<AudioListener>();
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
        }

        private void BuildMenu()
        {
            Canvas canvas = UiFactory.CreateCanvas("MenuCanvas");
            Image background = UiFactory.CreatePanel(canvas.transform, "Background", new Color(0.02f, 0.04f, 0.11f), Vector2.zero, Vector2.one);
            UiFactory.CreatePanel(background.transform, "TopGlow", new Color(0.18f, 0.28f, 0.7f, 0.18f), new Vector2(0f, 0.58f), new Vector2(1f, 1f));
            UiFactory.CreatePanel(background.transform, "BottomGlow", new Color(0.56f, 0.08f, 0.28f, 0.16f), new Vector2(0f, 0f), new Vector2(1f, 0.4f));

            Image titleFrame = UiFactory.CreateArcadePanel(background.transform, "TitleFrame", new Color(0.05f, 0.08f, 0.16f, 0.94f), new Color(0.22f, 0.62f, 1f, 0.96f), new Vector2(0.05f, 0.62f), new Vector2(0.95f, 0.93f), new Vector2(10f, 10f));
            UiFactory.CreateArcadeLabel(titleFrame.transform, "旺旺雷电", 92, TextAnchor.MiddleCenter, new Color(0.95f, 0.98f, 1f), FontStyle.Bold, new Vector2(0.08f, 0.62f), new Vector2(0.92f, 0.9f), Vector2.zero);
            UiFactory.CreateArcadeLabel(titleFrame.transform, "开始任务", 42, TextAnchor.MiddleCenter, new Color(0.48f, 0.84f, 1f), FontStyle.Bold, new Vector2(0.1f, 0.42f), new Vector2(0.9f, 0.58f), Vector2.zero);
            UiFactory.CreateArcadeLabel(titleFrame.transform, "编队突袭  精英来袭  敌方旗舰", 28, TextAnchor.MiddleCenter, new Color(1f, 0.54f, 0.72f), FontStyle.Bold, new Vector2(0.08f, 0.2f), new Vector2(0.92f, 0.38f), Vector2.zero);

            Image playerArt = UiFactory.CreatePanel(background.transform, "PlayerShipArt", Color.white, new Vector2(0.08f, 0.35f), new Vector2(0.42f, 0.58f));
            playerArt.sprite = RuntimeSpriteFactory.GetFighterJetSprite();
            playerArt.preserveAspect = true;
            playerArt.color = new Color(0.96f, 0.98f, 1f);

            Image enemyArt = UiFactory.CreatePanel(background.transform, "EnemyShipArt", new Color(1f, 0.58f, 0.74f, 1f), new Vector2(0.58f, 0.34f), new Vector2(0.92f, 0.57f));
            enemyArt.sprite = RuntimeSpriteFactory.GetBossFlagshipSprite();
            enemyArt.preserveAspect = true;
            enemyArt.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 180f);

            Image difficultyFrame = UiFactory.CreateArcadePanel(background.transform, "DifficultyFrame", new Color(0.04f, 0.08f, 0.16f, 0.94f), new Color(0.45f, 0.3f, 0.92f, 0.95f), new Vector2(0.07f, 0.12f), new Vector2(0.93f, 0.3f), new Vector2(10f, 10f));
            UiFactory.CreateArcadeLabel(difficultyFrame.transform, "选择难度", 38, TextAnchor.MiddleCenter, new Color(0.88f, 0.95f, 1f), FontStyle.Bold, new Vector2(0.1f, 0.64f), new Vector2(0.9f, 0.9f), Vector2.zero);

            Button lowButton = UiFactory.CreateButton(difficultyFrame.transform, "低级", new Color(0.16f, 0.56f, 1f), Color.white, new Vector2(220f, 108f), new Vector2(-250f, -46f));
            lowButton.onClick.AddListener(() => SceneNavigator.LoadGame(GameDifficulty.Low));
            Button mediumButton = UiFactory.CreateButton(difficultyFrame.transform, "中级", new Color(0.48f, 0.34f, 0.96f), Color.white, new Vector2(220f, 108f), new Vector2(0f, -46f));
            mediumButton.onClick.AddListener(() => SceneNavigator.LoadGame(GameDifficulty.Medium));
            Button highButton = UiFactory.CreateButton(difficultyFrame.transform, "高级", new Color(1f, 0.36f, 0.34f), Color.white, new Vector2(220f, 108f), new Vector2(250f, -46f));
            highButton.onClick.AddListener(() => SceneNavigator.LoadGame(GameDifficulty.High));

            foreach (Button button in new[] { lowButton, mediumButton, highButton })
            {
                Text label = button.GetComponentInChildren<Text>();
                label.fontStyle = FontStyle.Bold;
                label.fontSize = 42;
            }

            string hiScore = SessionState.HighScore > 0
                ? $"最高分 {SessionState.HighScore:0000000}"
                : "最高分 0000000";
            UiFactory.CreateArcadeLabel(background.transform, hiScore, 34, TextAnchor.MiddleCenter, new Color(0.52f, 0.84f, 1f), FontStyle.Bold, new Vector2(0.18f, 0.04f), new Vector2(0.82f, 0.1f), Vector2.zero);
        }
    }
}
