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
            Image background = UiFactory.CreatePanel(canvas.transform, "Background", victory ? new Color(0.03f, 0.06f, 0.14f) : Color.white, Vector2.zero, Vector2.one);
            if (!victory)
            {
                background.sprite = RuntimeSpriteFactory.GetRaidenStageBackgroundSprite(SessionState.CurrentStageNumber);
                background.preserveAspect = false;
                BuildFailureGameOver(background.transform);
                return;
            }

            UiFactory.CreatePanel(background.transform, "GlowLayer", victory ? new Color(0.16f, 0.45f, 0.94f, 0.22f) : new Color(0.9f, 0.12f, 0.28f, 0.18f), new Vector2(0f, 0.5f), new Vector2(1f, 1f));

            Color edgeColor = victory
                ? new Color(0.28f, 0.74f, 1f, 0.96f)
                : new Color(1f, 0.32f, 0.42f, 0.96f);
            Image resultCard = UiFactory.CreateArcadePanel(background.transform, "ResultCard", new Color(0.05f, 0.07f, 0.15f, 0.94f), edgeColor, new Vector2(0.07f, 0.14f), new Vector2(0.93f, 0.88f), new Vector2(12f, 12f));

            string title = victory ? "任务完成" : "任务失败";
            string stageLine = $"第{SessionState.CurrentStageNumber}关 {SessionState.CurrentStage.Name}  {BuildDifficultyText()}";
            string recordLine = $"评级 {SessionState.LastRunRating}  最高分 {SessionState.HighScore:0000000}";
            string comboLine = $"最高连击 {SessionState.LastRunMaxCombo}\n最高倍率 {SessionState.LastRunMaxMultiplier}倍";
            string summary = victory
                ? $"{SessionState.LastRunSummary}\n下一空域：{StageCatalog.GetStage(StageCatalog.GetNextStageIndex(SessionState.CurrentStageIndex)).Name}"
                : string.IsNullOrEmpty(SessionState.LastRunSummary) ? "再试一次，打穿敌方编队。\n优先保命，炸弹留给精英或旗舰压制。" : SessionState.LastRunSummary;
            string actionCopy = victory ? "继续下一关" : "重新挑战";

            UiFactory.CreateArcadeLabel(resultCard.transform, title, 84, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold, new Vector2(0.1f, 0.74f), new Vector2(0.9f, 0.9f), Vector2.zero);
            UiFactory.CreateArcadeLabel(resultCard.transform, stageLine, 32, TextAnchor.MiddleCenter, victory ? new Color(0.86f, 0.94f, 1f) : new Color(1f, 0.76f, 0.84f), FontStyle.Bold, new Vector2(0.12f, 0.64f), new Vector2(0.88f, 0.72f), Vector2.zero);
            UiFactory.CreateArcadeLabel(resultCard.transform, $"本局得分 {SessionState.LastScore:0000000}", 50, TextAnchor.MiddleCenter, new Color(0.95f, 0.98f, 1f), FontStyle.Bold, new Vector2(0.16f, 0.52f), new Vector2(0.84f, 0.62f), Vector2.zero);
            UiFactory.CreateArcadeLabel(resultCard.transform, recordLine, 34, TextAnchor.MiddleCenter, new Color(0.52f, 0.84f, 1f), FontStyle.Bold, new Vector2(0.16f, 0.45f), new Vector2(0.84f, 0.53f), Vector2.zero);
            UiFactory.CreateArcadeLabel(resultCard.transform, comboLine, 28, TextAnchor.MiddleCenter, new Color(1f, 0.86f, 0.32f), FontStyle.Bold, new Vector2(0.16f, 0.36f), new Vector2(0.84f, 0.45f), Vector2.zero);
            UiFactory.CreateArcadeLabel(resultCard.transform, summary, 22, TextAnchor.MiddleCenter, victory ? new Color(0.84f, 0.94f, 1f) : new Color(1f, 0.8f, 0.86f), FontStyle.Bold, new Vector2(0.14f, 0.27f), new Vector2(0.86f, 0.35f), Vector2.zero);

            Button retryButton = UiFactory.CreatePixelButton(background.transform, actionCopy, victory ? ArcadeTheme.ElectricBlue : ArcadeTheme.WarningRed, new Vector2(420f, 116f), new Vector2(0f, -150f));
            retryButton.onClick.AddListener(() =>
            {
                if (victory)
                {
                    SceneNavigator.LoadNextStage();
                }
                else
                {
                    SceneNavigator.LoadGame();
                }
            });

            Button menuButton = UiFactory.CreatePixelButton(background.transform, "返回主页", ArcadeTheme.EnergyYellow, new Vector2(420f, 116f), new Vector2(0f, -326f));
            menuButton.onClick.AddListener(SceneNavigator.LoadMenu);
        }

        private static void BuildFailureGameOver(Transform background)
        {
            UiFactory.CreatePanel(background, "FailureDim", new Color(0.01f, 0.01f, 0.03f, 0.58f), Vector2.zero, Vector2.one).raycastTarget = false;
            UiFactory.CreatePanel(background, "FailureTopShade", new Color(0f, 0f, 0f, 0.24f), new Vector2(0f, 0.56f), Vector2.one).raycastTarget = false;
            UiFactory.CreatePanel(background, "FailureBottomShade", new Color(0f, 0f, 0f, 0.4f), Vector2.zero, new Vector2(1f, 0.36f)).raycastTarget = false;

            Text shadow = UiFactory.CreateArcadeLabel(background, "任务失败", 92, TextAnchor.MiddleCenter, new Color(0f, 0f, 0f, 0.86f), FontStyle.Bold, new Vector2(0.08f, 0.6f), new Vector2(0.92f, 0.74f), new Vector2(6f, -8f));
            shadow.raycastTarget = false;
            Text title = UiFactory.CreateArcadeLabel(background, "任务失败", 92, TextAnchor.MiddleCenter, ArcadeTheme.WarningRed, FontStyle.Bold, new Vector2(0.08f, 0.6f), new Vector2(0.92f, 0.74f), Vector2.zero);
            title.raycastTarget = false;
            Outline titleOutline = title.gameObject.AddComponent<Outline>();
            titleOutline.effectColor = new Color(1f, 0.84f, 0.28f, 0.9f);
            titleOutline.effectDistance = new Vector2(3f, -3f);

            string stageLine = $"第{SessionState.CurrentStageNumber}关 {SessionState.CurrentStage.Name}  得分 {SessionState.LastScore:0000000}";
            Text stage = UiFactory.CreateArcadeLabel(background, stageLine, 34, TextAnchor.MiddleCenter, new Color(1f, 0.9f, 0.66f), FontStyle.Bold, new Vector2(0.08f, 0.52f), new Vector2(0.92f, 0.59f), Vector2.zero);
            stage.raycastTarget = false;
            Outline stageOutline = stage.gameObject.AddComponent<Outline>();
            stageOutline.effectColor = new Color(0f, 0f, 0f, 0.78f);
            stageOutline.effectDistance = new Vector2(2f, -2f);

            Button retryButton = UiFactory.CreatePixelButton(background, "重新挑战", ArcadeTheme.WarningRed, new Vector2(390f, 118f), new Vector2(-226f, -610f));
            retryButton.onClick.AddListener(SceneNavigator.LoadGame);
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
