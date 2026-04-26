using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Wanwan.Runtime
{
    public class MenuBootstrap : MonoBehaviour
    {
        public static readonly bool ShowStartupLogo = false;
        public const int TitleCircleButtonFontSize = 30;
        public const int TitleHighScoreFontSize = TitleCircleButtonFontSize;

        private static readonly Vector2 ShipPreviewSize = new Vector2(128f, 128f);

        private Canvas canvas;
        private MenuUiState state;
        private PlayerShipType selectedShip;
        private Text audioStatusText;
        private Text musicValueText;
        private Text sfxValueText;
        private Text opacityValueText;
        private Text saveStatusText;
        private float saveStatusTimer;

        private void Awake()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            selectedShip = SessionState.SelectedShip;
            EnsureCamera(ArcadeTheme.BackgroundBlack);
            canvas = UiFactory.CreateCanvas("MenuCanvas");
            if (ShowStartupLogo)
            {
                ShowLogo();
                StartCoroutine(LogoToTitle());
            }
            else
            {
                ShowTitle();
            }
        }

        private void Update()
        {
            RefreshSaveStatus();
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

        private IEnumerator LogoToTitle()
        {
            yield return new WaitForSeconds(2f);
            if (state == MenuUiState.Logo)
            {
                ShowTitle();
            }
        }

        private void ClearCanvas()
        {
            saveStatusText = null;
            saveStatusTimer = 0f;
            for (int i = canvas.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(canvas.transform.GetChild(i).gameObject);
            }
        }

        private Image CreateBackground(string name)
        {
            ClearCanvas();
            Image background = UiFactory.CreatePanel(canvas.transform, name, Color.white, Vector2.zero, Vector2.one);
            background.sprite = state == MenuUiState.Title ? RuntimeSpriteFactory.GetMenuStormTitleSprite() : RuntimeSpriteFactory.GetSkyBackgroundSprite();
            background.preserveAspect = false;
            background.raycastTarget = false;

            Image shade = UiFactory.CreatePanel(background.transform, name + "Shade", new Color(0.01f, 0.02f, 0.05f, 0.34f), Vector2.zero, Vector2.one);
            shade.raycastTarget = false;
            UiFactory.CreatePanel(background.transform, name + "TopVignette", new Color(0f, 0f, 0f, 0.18f), new Vector2(0f, 0.72f), Vector2.one).raycastTarget = false;
            if (state != MenuUiState.Title)
            {
                UiFactory.CreatePanel(background.transform, name + "BottomVignette", new Color(0f, 0f, 0f, 0.58f), Vector2.zero, new Vector2(1f, 0.42f)).raycastTarget = false;
            }
            CreateScanlines(background.transform);
            return background;
        }

        private static void CreateScanlines(Transform parent)
        {
            for (int i = 0; i < 24; i++)
            {
                float y = i / 24f;
                UiFactory.CreateDivider(parent, "Scanline" + i, new Color(1f, 1f, 1f, 0.025f), new Vector2(0f, y), new Vector2(1f, y + 0.0035f));
            }
        }

        private void ShowLogo()
        {
            state = MenuUiState.Logo;
            Image background = CreateBackground("LogoBackground");
            UiFactory.CreateArcadeLabel(background.transform, "WANWAN", ArcadeTheme.LogoSize, TextAnchor.MiddleCenter, ArcadeTheme.White, FontStyle.Bold, new Vector2(0.08f, 0.55f), new Vector2(0.92f, 0.65f), Vector2.zero);
            UiFactory.CreateArcadeLabel(background.transform, "空投爆破", ArcadeTheme.ScreenTitleSize, TextAnchor.MiddleCenter, ArcadeTheme.EnergyYellow, FontStyle.Bold, new Vector2(0.08f, 0.48f), new Vector2(0.92f, 0.56f), Vector2.zero);
            UiFactory.CreateDivider(background.transform, "LogoDivider", ArcadeTheme.WarningRed, new Vector2(0.2f, 0.47f), new Vector2(0.8f, 0.476f));
            UiFactory.CreateArcadeLabel(background.transform, "街机空战指挥部", ArcadeTheme.BodySize, TextAnchor.MiddleCenter, ArcadeTheme.ElectricBlue, FontStyle.Bold, new Vector2(0.08f, 0.39f), new Vector2(0.92f, 0.45f), Vector2.zero);
        }

        private void ShowTitle()
        {
            state = MenuUiState.Title;
            Image background = CreateBackground("TitleBackground");

            Button leaderboard = UiFactory.CreateArcadeSpriteIconButton(background.transform, RuntimeSpriteFactory.GetLeaderboardIconSprite(), ArcadeTheme.EnergyYellow, new Vector2(96f, 96f), new Vector2(76f, -132f), new Vector2(0f, 1f), new Vector2(0f, 1f));
            leaderboard.onClick.AddListener(ShowLeaderboard);
            Button settings = UiFactory.CreateArcadeSpriteIconButton(background.transform, RuntimeSpriteFactory.GetSettingsIconSprite(), ArcadeTheme.ElectricBlue, new Vector2(96f, 96f), new Vector2(-76f, -132f), Vector2.one, Vector2.one);
            settings.onClick.AddListener(ShowSettings);

            saveStatusText = UiFactory.CreateArcadeLabel(background.transform, string.Empty, ArcadeTheme.BodySize, TextAnchor.MiddleCenter, ArcadeTheme.EnergyYellow, FontStyle.Bold, new Vector2(0.12f, 0.27f), new Vector2(0.88f, 0.32f), Vector2.zero);
            saveStatusText.gameObject.SetActive(false);

            Image menu = UiFactory.CreatePanel(background.transform, "TitleMenu", Color.clear, new Vector2(0.06f, 0.075f), new Vector2(0.94f, 0.265f));
            menu.raycastTarget = false;
            Button save = UiFactory.CreateArcadeCircleButton(menu.transform, "保存", ArcadeTheme.ElectricBlue, new Vector2(213f, 213f), new Vector2(-360f, 0f));
            save.onClick.AddListener(ShowAutoSaveStatus);
            Button start = UiFactory.CreateArcadeCircleButton(menu.transform, "开始", ArcadeTheme.EnergyYellow, new Vector2(285f, 285f), Vector2.zero);
            start.onClick.AddListener(SceneNavigator.LoadGame);
            Button exit = UiFactory.CreateArcadeCircleButton(menu.transform, "退出", ArcadeTheme.WarningRed, new Vector2(213f, 213f), new Vector2(360f, 0f));
            exit.onClick.AddListener(Application.Quit);

            string hiScore = $"最高分 {SessionState.HighScore:0000000}";
            UiFactory.CreateArcadeLabel(background.transform, hiScore, TitleHighScoreFontSize, TextAnchor.MiddleCenter, ArcadeTheme.EnergyYellow, FontStyle.Bold, new Vector2(0.12f, 0.025f), new Vector2(0.88f, 0.07f), Vector2.zero);
        }

        private void ShowShipSelect()
        {
            state = MenuUiState.ShipSelect;
            Image background = CreateBackground("ShipSelectBackground");
            UiFactory.CreateArcadeLabel(background.transform, "选择战机", ArcadeTheme.ScreenTitleSize, TextAnchor.MiddleCenter, ArcadeTheme.White, FontStyle.Bold, new Vector2(0.06f, 0.86f), new Vector2(0.94f, 0.94f), Vector2.zero);
            CreateShipCard(background.transform, PlayerShipType.Green, new Vector2(0.08f, 0.48f), new Vector2(0.92f, 0.78f));
            CreateShipCard(background.transform, PlayerShipType.Blue, new Vector2(0.08f, 0.17f), new Vector2(0.92f, 0.47f));
            Button back = UiFactory.CreatePixelButton(background.transform, "返回", ArcadeTheme.DimGray, new Vector2(260f, 76f), new Vector2(-230f, -812f));
            back.onClick.AddListener(ShowTitle);
            Button next = UiFactory.CreatePixelButton(background.transform, "确认", ArcadeTheme.EnergyYellow, new Vector2(260f, 76f), new Vector2(230f, -812f));
            next.onClick.AddListener(() =>
            {
                SessionState.SelectShip(selectedShip);
                ShowDifficulty();
            });
        }

        private void CreateShipCard(Transform parent, PlayerShipType shipType, Vector2 anchorMin, Vector2 anchorMax)
        {
            ShipDefinition ship = ShipDefinition.Get(shipType);
            Color edge = selectedShip == shipType ? ArcadeTheme.EnergyYellow : ship.AccentColor;
            Image card = UiFactory.CreatePixelPanel(parent, ship.DisplayName + "Card", new Color(0.08f, 0.08f, 0.16f, 0.96f), edge, anchorMin, anchorMax, new Vector2(8f, 8f));
            Image shipImage = UiFactory.CreatePanel(card.transform, "ShipImage", ship.AccentColor, new Vector2(0.165f, 0.49f), new Vector2(0.165f, 0.49f));
            ConfigureShipPreview(shipImage, ship.AccentColor);
            UiFactory.CreateArcadeLabel(card.transform, ship.DisplayName, ArcadeTheme.TitleSize, TextAnchor.MiddleLeft, ship.AccentColor, FontStyle.Bold, new Vector2(0.32f, 0.68f), new Vector2(0.74f, 0.86f), Vector2.zero);
            UiFactory.CreateArcadeLabel(card.transform, $"主武器 {ship.MainWeapon}\n副武器 {ship.SubWeapon}", ArcadeTheme.BodySize, TextAnchor.MiddleLeft, ArcadeTheme.White, FontStyle.Bold, new Vector2(0.32f, 0.36f), new Vector2(0.82f, 0.66f), Vector2.zero);
            UiFactory.CreateArcadeLabel(card.transform, $"速度 {Stars(ship.SpeedStars)}   火力 {Stars(ship.PowerStars)}", ArcadeTheme.BodySize, TextAnchor.MiddleLeft, ArcadeTheme.EnergyYellow, FontStyle.Bold, new Vector2(0.32f, 0.14f), new Vector2(0.84f, 0.32f), Vector2.zero);
            Button select = UiFactory.CreatePixelButton(card.transform, selectedShip == shipType ? "已选择" : "选择", edge, new Vector2(170f, 70f), new Vector2(318f, -6f));
            select.onClick.AddListener(() =>
            {
                selectedShip = shipType;
                ShowShipSelect();
            });
        }

        private static void ConfigureShipPreview(Image image, Color tint)
        {
            image.sprite = RuntimeSpriteFactory.GetRaidenFighterJetSprite();
            image.color = tint;
            image.preserveAspect = true;
            image.raycastTarget = false;
            image.rectTransform.sizeDelta = ShipPreviewSize;
        }

        private void ShowDifficulty()
        {
            state = MenuUiState.Difficulty;
            Image background = CreateBackground("DifficultyBackground");
            UiFactory.CreateArcadeLabel(background.transform, "选择难度", ArcadeTheme.ScreenTitleSize, TextAnchor.MiddleCenter, ArcadeTheme.White, FontStyle.Bold, new Vector2(0.06f, 0.86f), new Vector2(0.94f, 0.94f), Vector2.zero);
            CreateDifficultyCard(background.transform, GameDifficulty.Low, "简单", "初始生命 5 / 敌弹较慢 / 适合长流程验收", ArcadeTheme.MilitaryGreen, new Vector2(0.08f, 0.62f), new Vector2(0.92f, 0.78f));
            CreateDifficultyCard(background.transform, GameDifficulty.Medium, "普通", "初始生命 3 / 标准弹幕 / 推荐体验", ArcadeTheme.ElectricBlue, new Vector2(0.08f, 0.42f), new Vector2(0.92f, 0.58f));
            CreateDifficultyCard(background.transform, GameDifficulty.High, "困难", "初始生命 2 / 强化火力 / 高压挑战", ArcadeTheme.WarningRed, new Vector2(0.08f, 0.22f), new Vector2(0.92f, 0.38f));
            Button back = UiFactory.CreatePixelButton(background.transform, "返回选机", ArcadeTheme.DimGray, new Vector2(360f, 78f), new Vector2(0f, -780f));
            back.onClick.AddListener(ShowShipSelect);
        }

        private void CreateDifficultyCard(Transform parent, GameDifficulty difficulty, string title, string desc, Color edge, Vector2 anchorMin, Vector2 anchorMax)
        {
            Image card = UiFactory.CreatePixelPanel(parent, title + "Card", new Color(0.08f, 0.08f, 0.16f, 0.96f), edge, anchorMin, anchorMax, new Vector2(8f, 8f));
            UiFactory.CreateArcadeLabel(card.transform, title, ArcadeTheme.TitleSize, TextAnchor.MiddleLeft, edge, FontStyle.Bold, new Vector2(0.08f, 0.5f), new Vector2(0.34f, 0.86f), Vector2.zero);
            UiFactory.CreateArcadeLabel(card.transform, desc, ArcadeTheme.BodySize, TextAnchor.MiddleLeft, ArcadeTheme.White, FontStyle.Bold, new Vector2(0.08f, 0.16f), new Vector2(0.68f, 0.5f), Vector2.zero);
            Button start = UiFactory.CreatePixelButton(card.transform, "出击", edge, new Vector2(180f, 72f), new Vector2(306f, -2f));
            start.onClick.AddListener(() => SceneNavigator.LoadGame(difficulty));
        }

        private void ShowLeaderboard()
        {
            state = MenuUiState.Leaderboard;
            Image background = CreateBackground("LeaderboardBackground");
            UiFactory.CreateArcadeLabel(background.transform, "本地排行榜", ArcadeTheme.ScreenTitleSize, TextAnchor.MiddleCenter, ArcadeTheme.White, FontStyle.Bold, new Vector2(0.06f, 0.86f), new Vector2(0.94f, 0.94f), Vector2.zero);
            UiFactory.CreateArcadeLabel(background.transform, BuildDifficultySettingText(SessionState.SelectedDifficulty) + " / 单机记录", ArcadeTheme.BodySize, TextAnchor.MiddleCenter, ArcadeTheme.EnergyYellow, FontStyle.Bold, new Vector2(0.1f, 0.81f), new Vector2(0.9f, 0.86f), Vector2.zero);
            Image board = UiFactory.CreatePixelPanel(background.transform, "LeaderboardPanel", new Color(0.08f, 0.08f, 0.16f, 0.96f), ArcadeTheme.EnergyYellow, new Vector2(0.08f, 0.18f), new Vector2(0.92f, 0.82f), new Vector2(8f, 8f));
            UiFactory.CreateArcadeLabel(board.transform, "名次  名号   分数      难度  关卡", ArcadeTheme.BodySize, TextAnchor.UpperLeft, ArcadeTheme.ElectricBlue, FontStyle.Bold, new Vector2(0.08f, 0.9f), new Vector2(0.92f, 0.98f), Vector2.zero);
            LeaderboardEntry[] entries = SessionState.GetLeaderboardEntries(SessionState.SelectedDifficulty);
            if (entries.Length == 0)
            {
                UiFactory.CreateArcadeLabel(board.transform, "暂无记录\n出击后刷新榜单", ArcadeTheme.TitleSize, TextAnchor.MiddleCenter, ArcadeTheme.DimGray, FontStyle.Bold, new Vector2(0.1f, 0.35f), new Vector2(0.9f, 0.62f), Vector2.zero);
            }
            else
            {
                for (int i = 0; i < entries.Length; i++)
                {
                    LeaderboardEntry entry = entries[i];
                    Color color = i < 3 ? ArcadeTheme.EnergyYellow : ArcadeTheme.White;
                    string row = $"{i + 1:00}  {entry.Name,-3}  {entry.Score:0000000}  {BuildDifficultyShort(entry.Difficulty),-2}   {entry.LoopNumber}-{entry.StageNumber}";
                    float top = 0.84f - i * 0.075f;
                    UiFactory.CreateArcadeLabel(board.transform, row, ArcadeTheme.BodySize, TextAnchor.MiddleLeft, color, FontStyle.Bold, new Vector2(0.08f, top - 0.055f), new Vector2(0.92f, top), Vector2.zero);
                }
            }

            Button back = UiFactory.CreatePixelButton(background.transform, "返回标题", ArcadeTheme.DimGray, new Vector2(360f, 78f), new Vector2(0f, -780f));
            back.onClick.AddListener(ShowTitle);
        }

        private void ShowSettings()
        {
            state = MenuUiState.Settings;
            Image background = CreateBackground("SettingsBackground");
            UiFactory.CreateArcadeLabel(background.transform, "设置", ArcadeTheme.ScreenTitleSize, TextAnchor.MiddleCenter, ArcadeTheme.White, FontStyle.Bold, new Vector2(0.06f, 0.86f), new Vector2(0.94f, 0.94f), Vector2.zero);
            Image panel = UiFactory.CreatePixelPanel(background.transform, "SettingsPanel", new Color(0.08f, 0.08f, 0.16f, 0.96f), ArcadeTheme.ElectricBlue, new Vector2(0.08f, 0.18f), new Vector2(0.92f, 0.82f), new Vector2(8f, 8f));
            audioStatusText = UiFactory.CreateArcadeLabel(panel.transform, BuildAudioText(), ArcadeTheme.BodySize, TextAnchor.MiddleCenter, ArcadeTheme.EnergyYellow, FontStyle.Bold, new Vector2(0.12f, 0.86f), new Vector2(0.88f, 0.96f), Vector2.zero);

            Button audioButton = UiFactory.CreatePixelToggle(panel.transform, "总声音", SessionState.AudioEnabled, new Vector2(0f, 260f), () =>
            {
                SessionState.SetAudioEnabled(!SessionState.AudioEnabled);
                ShowSettings();
            });

            UiFactory.CreatePixelSlider(panel.transform, "音乐音量", SessionState.MusicVolume, new Vector2(0.08f, 0.58f), new Vector2(0.92f, 0.68f), value =>
            {
                SessionState.SetMusicVolume(value);
                if (musicValueText != null) musicValueText.text = Mathf.RoundToInt(SessionState.MusicVolume * 100f) + "%";
            }, out musicValueText);
            UiFactory.CreatePixelSlider(panel.transform, "音效音量", SessionState.SfxVolume, new Vector2(0.08f, 0.45f), new Vector2(0.92f, 0.55f), value =>
            {
                SessionState.SetSfxVolume(value);
                if (sfxValueText != null) sfxValueText.text = Mathf.RoundToInt(SessionState.SfxVolume * 100f) + "%";
            }, out sfxValueText);
            UiFactory.CreatePixelSlider(panel.transform, "按键透明", SessionState.VirtualButtonOpacity, new Vector2(0.08f, 0.32f), new Vector2(0.92f, 0.42f), value =>
            {
                SessionState.SetVirtualButtonOpacity(value);
                if (opacityValueText != null) opacityValueText.text = Mathf.RoundToInt(SessionState.VirtualButtonOpacity * 100f) + "%";
            }, out opacityValueText);

            Button defaultShipButton = UiFactory.CreatePixelButton(panel.transform, "默认战机 " + BuildDefaultShipText(), ArcadeTheme.EnergyYellow, new Vector2(420f, 72f), new Vector2(-220f, -136f));
            defaultShipButton.onClick.AddListener(CycleDefaultShip);
            Button defaultDifficultyButton = UiFactory.CreatePixelButton(panel.transform, "默认难度 " + BuildDifficultySettingText(SessionState.SelectedDifficulty), ArcadeTheme.ElectricBlue, new Vector2(420f, 72f), new Vector2(220f, -136f));
            defaultDifficultyButton.onClick.AddListener(CycleDefaultDifficulty);

            Button sensitivityButton = UiFactory.CreatePixelButton(panel.transform, "灵敏度 " + BuildSensitivityText(), ArcadeTheme.EnergyYellow, new Vector2(420f, 72f), new Vector2(-220f, -226f));
            sensitivityButton.onClick.AddListener(CycleSensitivity);
            Button nameButton = UiFactory.CreatePixelButton(panel.transform, "榜名 " + SessionState.LeaderboardName, ArcadeTheme.ElectricBlue, new Vector2(420f, 72f), new Vector2(220f, -226f));
            nameButton.onClick.AddListener(CycleLeaderboardName);
            Button vibrationButton = UiFactory.CreatePixelToggle(panel.transform, "震动", SessionState.VibrationEnabled, new Vector2(-220f, -316f), () =>
            {
                SessionState.SetVibrationEnabled(!SessionState.VibrationEnabled);
                ShowSettings();
            });
            Button damageButton = UiFactory.CreatePixelToggle(panel.transform, "伤害数字", SessionState.DamageNumbersEnabled, new Vector2(220f, -316f), () =>
            {
                SessionState.SetDamageNumbersEnabled(!SessionState.DamageNumbersEnabled);
                ShowSettings();
            });
            Button effectsButton = UiFactory.CreatePixelButton(panel.transform, "特效质量 " + BuildVisualEffectsQualityText(), ArcadeTheme.MilitaryGreen, new Vector2(420f, 72f), new Vector2(-220f, -406f));
            effectsButton.onClick.AddListener(CycleVisualEffectsQuality);
            Button resetButton = UiFactory.CreatePixelButton(panel.transform, "恢复默认", ArcadeTheme.WarningRed, new Vector2(420f, 72f), new Vector2(220f, -406f));
            resetButton.onClick.AddListener(() =>
            {
                SessionState.ResetSettings();
                ShowSettings();
            });

            Button back = UiFactory.CreatePixelButton(background.transform, "返回标题", ArcadeTheme.DimGray, new Vector2(360f, 78f), new Vector2(0f, -780f));
            back.onClick.AddListener(ShowTitle);
        }

        private static string Stars(int count)
        {
            return new string('■', Mathf.Clamp(count, 1, 5)).PadRight(5, '□');
        }

        private static string BuildDifficultyShort(GameDifficulty difficulty)
        {
            switch (difficulty)
            {
                case GameDifficulty.Medium:
                    return "普";
                case GameDifficulty.High:
                    return "难";
                default:
                    return "易";
            }
        }

        private static string BuildDifficultySettingText(GameDifficulty difficulty)
        {
            switch (difficulty)
            {
                case GameDifficulty.Low:
                    return "简单";
                case GameDifficulty.High:
                    return "困难";
                default:
                    return "普通";
            }
        }

        private static string BuildDefaultShipText()
        {
            return ShipDefinition.Get(SessionState.SelectedShip).DisplayName;
        }

        private string BuildAudioText()
        {
            return SessionState.AudioEnabled ? "声音开启" : "声音关闭";
        }

        private void ShowAutoSaveStatus()
        {
            PlayerPrefs.Save();
            saveStatusTimer = 2.2f;
            if (saveStatusText != null)
            {
                saveStatusText.text = "已自动保存";
                saveStatusText.gameObject.SetActive(true);
            }
        }

        private void RefreshSaveStatus()
        {
            if (saveStatusText == null || saveStatusTimer <= 0f)
            {
                return;
            }

            saveStatusTimer = Mathf.Max(0f, saveStatusTimer - Time.unscaledDeltaTime);
            if (saveStatusTimer <= 0f)
            {
                saveStatusText.gameObject.SetActive(false);
            }
        }

        private string BuildSensitivityText()
        {
            switch (SessionState.ControlSensitivity)
            {
                case ControlSensitivity.Low:
                    return "低";
                case ControlSensitivity.High:
                    return "高";
                default:
                    return "中";
            }
        }

        private string BuildVisualEffectsQualityText()
        {
            return SessionState.VisualEffectsQuality == VisualEffectsQuality.BatterySaver ? "省电" : "完整";
        }

        private void CycleVisualEffectsQuality()
        {
            VisualEffectsQuality next = SessionState.VisualEffectsQuality == VisualEffectsQuality.Full
                ? VisualEffectsQuality.BatterySaver
                : VisualEffectsQuality.Full;
            SessionState.SetVisualEffectsQuality(next);
            ShowSettings();
        }

        private void CycleSensitivity()
        {
            ControlSensitivity next = SessionState.ControlSensitivity == ControlSensitivity.Low
                ? ControlSensitivity.Medium
                : SessionState.ControlSensitivity == ControlSensitivity.Medium ? ControlSensitivity.High : ControlSensitivity.Low;
            SessionState.SetControlSensitivity(next);
            ShowSettings();
        }

        private void CycleDefaultShip()
        {
            PlayerShipType next = SessionState.SelectedShip == PlayerShipType.Green ? PlayerShipType.Blue : PlayerShipType.Green;
            selectedShip = next;
            SessionState.SelectShip(next);
            ShowSettings();
        }

        private void CycleDefaultDifficulty()
        {
            GameDifficulty next;
            switch (SessionState.SelectedDifficulty)
            {
                case GameDifficulty.Low:
                    next = GameDifficulty.Medium;
                    break;
                case GameDifficulty.Medium:
                    next = GameDifficulty.High;
                    break;
                default:
                    next = GameDifficulty.Low;
                    break;
            }

            SessionState.SelectDifficulty(next);
            ShowSettings();
        }

        private void CycleLeaderboardName()
        {
            string current = SessionState.LeaderboardName;
            char[] chars = current.ToCharArray();
            int value = chars[2] - 'A';
            chars[2] = (char)('A' + ((value + 1) % 26));
            SessionState.SetLeaderboardName(new string(chars));
            ShowSettings();
        }
    }
}
