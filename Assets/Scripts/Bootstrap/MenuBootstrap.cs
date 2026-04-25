using UnityEngine;
using UnityEngine.UI;

namespace Wanwan.Runtime
{
    public class MenuBootstrap : MonoBehaviour
    {
        private GameObject settingsPanel;
        private Text audioStatusText;
        private Text musicValueText;
        private Text sfxValueText;

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

            Image previewFrame = UiFactory.CreateArcadePanel(background.transform, "StagePreviewFrame", new Color(0.04f, 0.09f, 0.15f, 0.94f), SessionState.CurrentStage.AccentColor, new Vector2(0.09f, 0.3f), new Vector2(0.91f, 0.42f), new Vector2(8f, 8f));
            UiFactory.CreateArcadeLabel(previewFrame.transform, "关卡预览", 26, TextAnchor.UpperCenter, new Color(0.7f, 0.9f, 1f), FontStyle.Bold, new Vector2(0.08f, 0.68f), new Vector2(0.92f, 0.94f), Vector2.zero);
            UiFactory.CreateArcadeLabel(previewFrame.transform, StageCatalog.BuildPreviewSummary(SessionState.CurrentStageIndex), 30, TextAnchor.MiddleCenter, new Color(0.96f, 0.98f, 1f), FontStyle.Bold, new Vector2(0.08f, 0.08f), new Vector2(0.92f, 0.74f), Vector2.zero);

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
            UiFactory.CreateArcadeLabel(background.transform, hiScore, 34, TextAnchor.MiddleCenter, new Color(0.52f, 0.84f, 1f), FontStyle.Bold, new Vector2(0.18f, 0.04f), new Vector2(0.68f, 0.1f), Vector2.zero);
            Button settingsButton = UiFactory.CreateButton(background.transform, "设置", new Color(0.18f, 0.58f, 0.9f), Color.white, new Vector2(190f, 82f), new Vector2(330f, -846f));
            settingsButton.onClick.AddListener(ToggleSettingsPanel);
            settingsButton.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;

            BuildSettingsPanel(background.transform);
        }

        private void BuildSettingsPanel(Transform parent)
        {
            Image settingsFrame = UiFactory.CreateArcadePanel(parent, "SettingsFrame", new Color(0.04f, 0.07f, 0.13f, 0.98f), new Color(0.28f, 0.72f, 1f, 0.96f), new Vector2(0.08f, 0.18f), new Vector2(0.92f, 0.54f), new Vector2(10f, 10f));
            settingsPanel = settingsFrame.gameObject;
            settingsPanel.SetActive(false);

            UiFactory.CreateArcadeLabel(settingsFrame.transform, "设置", 52, TextAnchor.MiddleCenter, new Color(0.94f, 0.98f, 1f), FontStyle.Bold, new Vector2(0.1f, 0.78f), new Vector2(0.9f, 0.94f), Vector2.zero);
            audioStatusText = UiFactory.CreateArcadeLabel(settingsFrame.transform, BuildAudioStatusText(), 30, TextAnchor.MiddleCenter, new Color(0.84f, 0.94f, 1f), FontStyle.Bold, new Vector2(0.1f, 0.62f), new Vector2(0.9f, 0.72f), Vector2.zero);

            Button audioButton = UiFactory.CreateButton(settingsFrame.transform, "音效开关", new Color(0.24f, 0.62f, 1f), Color.white, new Vector2(360f, 86f), new Vector2(0f, 232f));
            audioButton.onClick.AddListener(() =>
            {
                SessionState.SetAudioEnabled(!SessionState.AudioEnabled);
                RefreshSettingsLabels();
            });

            CreateVolumeSlider(settingsFrame.transform, "音乐", SessionState.MusicVolume, new Vector2(0.16f, 0.4f), new Vector2(0.84f, 0.52f), value =>
            {
                SessionState.SetMusicVolume(value);
                RefreshSettingsLabels();
            }, out musicValueText);
            CreateVolumeSlider(settingsFrame.transform, "音效", SessionState.SfxVolume, new Vector2(0.16f, 0.22f), new Vector2(0.84f, 0.34f), value =>
            {
                SessionState.SetSfxVolume(value);
                RefreshSettingsLabels();
            }, out sfxValueText);

            Button closeButton = UiFactory.CreateButton(settingsFrame.transform, "关闭", new Color(0.82f, 0.3f, 0.78f), Color.white, new Vector2(300f, 82f), new Vector2(0f, -246f));
            closeButton.onClick.AddListener(ToggleSettingsPanel);
        }

        private static void CreateVolumeSlider(Transform parent, string label, float value, Vector2 anchorMin, Vector2 anchorMax, UnityEngine.Events.UnityAction<float> onChanged, out Text valueText)
        {
            Image row = UiFactory.CreatePanel(parent, label + "Row", new Color(0.08f, 0.11f, 0.2f, 0.82f), anchorMin, anchorMax);
            UiFactory.CreateArcadeLabel(row.transform, label, 28, TextAnchor.MiddleLeft, new Color(0.94f, 0.98f, 1f), FontStyle.Bold, new Vector2(0.03f, 0f), new Vector2(0.28f, 1f), Vector2.zero);
            valueText = UiFactory.CreateArcadeLabel(row.transform, Mathf.RoundToInt(value * 100f) + "%", 26, TextAnchor.MiddleRight, new Color(0.54f, 0.86f, 1f), FontStyle.Bold, new Vector2(0.76f, 0f), new Vector2(0.97f, 1f), Vector2.zero);

            GameObject sliderObject = new GameObject(label + "Slider", typeof(RectTransform), typeof(Slider));
            sliderObject.transform.SetParent(row.transform, false);
            RectTransform sliderRect = sliderObject.GetComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.31f, 0.22f);
            sliderRect.anchorMax = new Vector2(0.73f, 0.78f);
            sliderRect.offsetMin = Vector2.zero;
            sliderRect.offsetMax = Vector2.zero;

            Image background = UiFactory.CreatePanel(sliderObject.transform, "Background", new Color(0.02f, 0.04f, 0.09f, 0.95f), new Vector2(0f, 0.34f), new Vector2(1f, 0.66f));
            Image fill = UiFactory.CreatePanel(background.transform, "Fill", new Color(0.28f, 0.78f, 1f, 0.96f), Vector2.zero, Vector2.one);
            Image handle = UiFactory.CreatePanel(sliderObject.transform, "Handle", new Color(1f, 0.72f, 0.3f, 1f), new Vector2(0f, 0.12f), new Vector2(0.08f, 0.88f));

            Slider slider = sliderObject.GetComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = value;
            slider.fillRect = fill.rectTransform;
            slider.handleRect = handle.rectTransform;
            slider.targetGraphic = handle;
            slider.direction = Slider.Direction.LeftToRight;
            slider.onValueChanged.AddListener(onChanged);
        }

        private void ToggleSettingsPanel()
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
            RefreshSettingsLabels();
        }

        private void RefreshSettingsLabels()
        {
            audioStatusText.text = BuildAudioStatusText();
            musicValueText.text = Mathf.RoundToInt(SessionState.MusicVolume * 100f) + "%";
            sfxValueText.text = Mathf.RoundToInt(SessionState.SfxVolume * 100f) + "%";
        }

        private static string BuildAudioStatusText()
        {
            return SessionState.AudioEnabled ? "声音 开" : "声音 关";
        }
    }
}
