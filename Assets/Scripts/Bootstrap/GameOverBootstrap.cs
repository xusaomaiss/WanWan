using UnityEngine;
using UnityEngine.UI;

namespace Wanwan.Runtime
{
    public class GameOverBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            bool victory = SessionState.LastRunWasVictory;
            Screen.orientation = ScreenOrientation.Portrait;
            EnsureCamera(victory ? new Color(0.03f, 0.06f, 0.14f) : new Color(0.09f, 0.03f, 0.12f));
            BuildGameOver();
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

        private void BuildGameOver()
        {
            Canvas canvas = UiFactory.CreateCanvas("GameOverCanvas");
            bool victory = SessionState.LastRunWasVictory;
            Image background = UiFactory.CreatePanel(canvas.transform, "Background", Color.white, Vector2.zero, Vector2.one);
            background.sprite = RuntimeSpriteFactory.GetRaidenStageBackgroundSprite(SessionState.CurrentStageNumber);
            background.preserveAspect = false;
            BuildFloatingGameOver(background.transform, victory);
        }

        private static void BuildFloatingGameOver(Transform background, bool victory)
        {
            Color titleColor = victory ? ArcadeTheme.ElectricBlue : ArcadeTheme.WarningRed;
            Color outlineColor = victory ? new Color(0.78f, 1f, 0.94f, 0.9f) : new Color(1f, 0.84f, 0.28f, 0.9f);
            Color detailColor = victory ? new Color(0.78f, 1f, 0.92f) : new Color(1f, 0.9f, 0.66f);
            string titleText = victory ? "任务完成" : "任务失败";
            string stageLine = victory
                ? $"第{SessionState.CurrentStageNumber}关突破  得分 {SessionState.LastScore:0000000}"
                : $"第{SessionState.CurrentStageNumber}关 {SessionState.CurrentStage.Name}  得分 {SessionState.LastScore:0000000}";

            UiFactory.CreatePanel(background, "ResultDim", new Color(0.01f, 0.01f, 0.03f, victory ? 0.5f : 0.58f), Vector2.zero, Vector2.one).raycastTarget = false;
            UiFactory.CreatePanel(background, "ResultTopShade", new Color(0f, 0f, 0f, 0.24f), new Vector2(0f, 0.56f), Vector2.one).raycastTarget = false;
            UiFactory.CreatePanel(background, "ResultBottomShade", new Color(0f, 0f, 0f, 0.34f), Vector2.zero, new Vector2(1f, 0.36f)).raycastTarget = false;

            Text shadow = UiFactory.CreateArcadeLabel(background, titleText, 92, TextAnchor.MiddleCenter, new Color(0f, 0f, 0f, 0.86f), FontStyle.Bold, new Vector2(0.08f, 0.6f), new Vector2(0.92f, 0.74f), new Vector2(6f, -8f));
            shadow.raycastTarget = false;
            Text title = UiFactory.CreateArcadeLabel(background, titleText, 92, TextAnchor.MiddleCenter, titleColor, FontStyle.Bold, new Vector2(0.08f, 0.6f), new Vector2(0.92f, 0.74f), Vector2.zero);
            title.raycastTarget = false;
            Outline titleOutline = title.gameObject.AddComponent<Outline>();
            titleOutline.effectColor = outlineColor;
            titleOutline.effectDistance = new Vector2(3f, -3f);

            Text stage = UiFactory.CreateArcadeLabel(background, stageLine, 34, TextAnchor.MiddleCenter, detailColor, FontStyle.Bold, new Vector2(0.08f, 0.52f), new Vector2(0.92f, 0.59f), Vector2.zero);
            stage.raycastTarget = false;
            Outline stageOutline = stage.gameObject.AddComponent<Outline>();
            stageOutline.effectColor = new Color(0f, 0f, 0f, 0.78f);
            stageOutline.effectDistance = new Vector2(2f, -2f);

            string primaryCopy = victory ? "继续下一关" : "重新挑战";
            Button primaryButton = UiFactory.CreatePixelButton(background, primaryCopy, victory ? ArcadeTheme.ElectricBlue : ArcadeTheme.WarningRed, new Vector2(390f, 118f), new Vector2(-226f, -610f));
            primaryButton.onClick.AddListener(victory ? SceneNavigator.LoadNextStage : SceneNavigator.LoadGame);
            Button menuButton = UiFactory.CreatePixelButton(background, "返回主页", ArcadeTheme.EnergyYellow, new Vector2(390f, 118f), new Vector2(226f, -610f));
            menuButton.onClick.AddListener(SceneNavigator.LoadMenu);
        }

        private static string BuildDifficultyText()
        {
            switch (SessionState.LastRunDifficulty)
            {
                case GameDifficulty.Medium:
                    return "中级";
                case GameDifficulty.High:
                    return "高级";
                default:
                    return "低级";
            }
        }
    }
}
