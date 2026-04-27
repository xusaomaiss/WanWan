using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Wanwan.Runtime
{
    public static class UiFactory
    {
        public static Canvas CreateCanvas(string name)
        {
            EnsureEventSystem();

            GameObject canvasObject = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            return canvas;
        }

        public static Image CreatePanel(Transform parent, string name, Color color, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject panelObject = new GameObject(name, typeof(RectTransform), typeof(Image));
            panelObject.transform.SetParent(parent, false);

            RectTransform rect = panelObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image image = panelObject.GetComponent<Image>();
            image.color = color;
            return image;
        }

        public static Image CreateArcadePanel(
            Transform parent,
            string name,
            Color fillColor,
            Color edgeColor,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 inset)
        {
            Image frame = CreatePanel(parent, name, edgeColor, anchorMin, anchorMax);
            Image inner = CreatePanel(frame.transform, name + "Inner", fillColor, Vector2.zero, Vector2.one);
            inner.rectTransform.offsetMin = inset;
            inner.rectTransform.offsetMax = -inset;
            return frame;
        }

        public static Image CreatePixelPanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
        {
            return CreatePixelPanel(parent, name, ArcadeTheme.PanelBase, ArcadeTheme.DimGray, anchorMin, anchorMax, new Vector2(8f, 8f));
        }

        public static Image CreatePixelPanel(Transform parent, string name, Color fillColor, Color edgeColor, Vector2 anchorMin, Vector2 anchorMax, Vector2 inset)
        {
            Image frame = CreatePanel(parent, name, ArcadeTheme.InkBlack, anchorMin, anchorMax);
            Image darkEdge = CreatePanel(frame.transform, name + "DarkEdge", edgeColor, Vector2.zero, Vector2.one);
            darkEdge.rectTransform.offsetMin = new Vector2(2f, 2f);
            darkEdge.rectTransform.offsetMax = new Vector2(-2f, -2f);
            Image inner = CreatePanel(darkEdge.transform, name + "Fill", fillColor, Vector2.zero, Vector2.one);
            inner.rectTransform.offsetMin = inset;
            inner.rectTransform.offsetMax = -inset;
            Image highlight = CreatePanel(frame.transform, name + "Highlight", new Color(1f, 1f, 1f, 0.2f), new Vector2(0f, 0.985f), Vector2.one);
            highlight.raycastTarget = false;
            return frame;
        }

        public static Image CreateDivider(Transform parent, string name, Color color, Vector2 anchorMin, Vector2 anchorMax)
        {
            Image divider = CreatePanel(parent, name, color, anchorMin, anchorMax);
            divider.raycastTarget = false;
            return divider;
        }

        public static Text CreateText(Transform parent, string content, int fontSize, TextAnchor alignment, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition)
        {
            GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);

            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = anchoredPosition;

            Text text = textObject.GetComponent<Text>();
            text.text = content;
            ArcadeFontProvider.ApplyTo(text);
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.resizeTextForBestFit = false;
            text.resizeTextMinSize = Mathf.Max(10, fontSize - 10);
            text.resizeTextMaxSize = fontSize;
            return text;
        }

        public static Text CreateArcadeLabel(
            Transform parent,
            string content,
            int fontSize,
            TextAnchor alignment,
            Color color,
            FontStyle fontStyle,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 anchoredPosition)
        {
            Text text = CreateText(parent, content, fontSize, alignment, color, anchorMin, anchorMax, anchoredPosition);
            text.fontStyle = fontStyle;
            return text;
        }

        public static void ConfigureSingleLine(Text text)
        {
            if (text == null)
            {
                return;
            }

            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
        }

        public static Button CreateButton(Transform parent, string label, Color buttonColor, Color textColor, Vector2 size, Vector2 anchoredPosition)
        {
            return CreateButton(parent, label, buttonColor, textColor, size, anchoredPosition, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        }

        public static Button CreateButton(Transform parent, string label, Color buttonColor, Color textColor, Vector2 size, Vector2 anchoredPosition, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject buttonObject = new GameObject(label + "Button", typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);

            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;

            Image image = buttonObject.GetComponent<Image>();
            image.color = buttonColor;

            Button button = buttonObject.GetComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = buttonColor;
            colors.highlightedColor = buttonColor * 1.05f;
            colors.pressedColor = buttonColor * 0.95f;
            colors.selectedColor = buttonColor;
            colors.disabledColor = new Color(buttonColor.r, buttonColor.g, buttonColor.b, 0.35f);
            button.colors = colors;

            Text text = CreateText(buttonObject.transform, label, 44, TextAnchor.MiddleCenter, textColor, Vector2.zero, Vector2.one, Vector2.zero);
            ConfigureSingleLine(text);
            text.resizeTextForBestFit = false;
            text.resizeTextMinSize = 20;
            text.resizeTextMaxSize = 44;

            return button;
        }

        public static Button CreatePixelButton(Transform parent, string label, Color accentColor, Vector2 size, Vector2 anchoredPosition)
        {
            Button button = CreateButton(parent, label, ArcadeTheme.PanelBase, ArcadeTheme.White, size, anchoredPosition);
            Image image = button.GetComponent<Image>();
            image.color = ArcadeTheme.PanelBase;

            ColorBlock colors = button.colors;
            colors.normalColor = ArcadeTheme.PanelBase;
            colors.highlightedColor = accentColor;
            colors.pressedColor = ArcadeTheme.WarningRed;
            colors.selectedColor = accentColor;
            colors.disabledColor = ArcadeTheme.DimGray;
            button.colors = colors;

            CreateDivider(button.transform, "TopEdge", new Color(1f, 1f, 1f, 0.26f), new Vector2(0f, 0.94f), Vector2.one);
            CreateDivider(button.transform, "BottomEdge", ArcadeTheme.InkBlack, Vector2.zero, new Vector2(1f, 0.08f));
            Text text = button.GetComponentInChildren<Text>();
            text.fontStyle = FontStyle.Bold;
            text.fontSize = 30;
            text.resizeTextMaxSize = 30;
            return button;
        }

        public static Button CreateArcadeCircleButton(Transform parent, string label, Color accentColor, Vector2 size, Vector2 anchoredPosition)
        {
            Button button = CreateButton(parent, label, ArcadeTheme.PanelBase, ArcadeTheme.White, size, anchoredPosition);
            Image image = button.GetComponent<Image>();
            image.sprite = RuntimeSpriteFactory.GetCircleSprite();
            image.color = Color.clear;

            Color faceColor = Color.Lerp(accentColor, Color.white, 0.08f);
            ColorBlock colors = button.colors;
            colors.normalColor = faceColor;
            colors.highlightedColor = Color.Lerp(faceColor, Color.white, 0.18f);
            colors.pressedColor = Color.Lerp(faceColor, ArcadeTheme.InkBlack, 0.28f);
            colors.selectedColor = accentColor;
            colors.disabledColor = ArcadeTheme.DimGray;
            button.colors = colors;

            Image outer = CreatePanel(button.transform, "OuterRing", accentColor, new Vector2(0.04f, 0.04f), new Vector2(0.96f, 0.96f));
            outer.sprite = RuntimeSpriteFactory.GetCircleSprite();
            outer.raycastTarget = false;

            Image face = CreatePanel(button.transform, "ButtonFace", faceColor, new Vector2(0.13f, 0.13f), new Vector2(0.87f, 0.87f));
            face.sprite = RuntimeSpriteFactory.GetCircleSprite();
            face.raycastTarget = false;
            button.targetGraphic = face;

            Image shine = CreatePanel(button.transform, "ButtonShine", new Color(1f, 1f, 1f, 0.22f), new Vector2(0.24f, 0.62f), new Vector2(0.74f, 0.82f));
            shine.sprite = RuntimeSpriteFactory.GetCircleSprite();
            shine.raycastTarget = false;

            ArcadeButtonPressFeedback feedback = button.gameObject.AddComponent<ArcadeButtonPressFeedback>();
            feedback.Configure(face, shine, outer, accentColor);

            Text text = button.GetComponentInChildren<Text>();
            text.transform.SetAsLastSibling();
            text.fontStyle = FontStyle.Bold;
            text.fontSize = 30;
            text.resizeTextMinSize = 18;
            text.resizeTextMaxSize = 30;
            text.color = accentColor == ArcadeTheme.EnergyYellow ? ArcadeTheme.InkBlack : ArcadeTheme.White;
            return button;
        }

        public static Button CreateArcadeIconButton(Transform parent, string icon, Color accentColor, Vector2 size, Vector2 anchoredPosition, Vector2 anchorMin, Vector2 anchorMax)
        {
            Button button = CreateArcadeCircleButton(parent, icon, accentColor, size, anchoredPosition);
            RectTransform rect = button.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;

            Text text = button.GetComponentInChildren<Text>();
            text.fontSize = 34;
            text.resizeTextMaxSize = 34;
            return button;
        }

        public static Button CreateArcadeSpriteIconButton(Transform parent, Sprite icon, Color accentColor, Vector2 size, Vector2 anchoredPosition, Vector2 anchorMin, Vector2 anchorMax)
        {
            Button button = CreateButton(parent, string.Empty, Color.clear, Color.clear, size, anchoredPosition);
            RectTransform rect = button.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;

            Image background = button.GetComponent<Image>();
            background.color = new Color(0f, 0f, 0f, 0.18f);
            background.sprite = RuntimeSpriteFactory.GetRoundedSquareSprite();

            Image glow = CreatePanel(button.transform, "IconGlow", accentColor, new Vector2(0.08f, 0.08f), new Vector2(0.92f, 0.92f));
            glow.sprite = RuntimeSpriteFactory.GetRoundedSquareSprite();
            glow.color = new Color(accentColor.r, accentColor.g, accentColor.b, 0.22f);
            glow.raycastTarget = false;

            Image iconImage = CreatePanel(button.transform, "Icon", accentColor, new Vector2(0.18f, 0.18f), new Vector2(0.82f, 0.82f));
            iconImage.sprite = icon;
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;
            button.targetGraphic = iconImage;

            ColorBlock colors = button.colors;
            colors.normalColor = accentColor;
            colors.highlightedColor = Color.Lerp(accentColor, Color.white, 0.18f);
            colors.pressedColor = Color.Lerp(accentColor, ArcadeTheme.WarningRed, 0.28f);
            colors.selectedColor = accentColor;
            colors.disabledColor = ArcadeTheme.DimGray;
            button.colors = colors;

            ArcadeButtonPressFeedback feedback = button.gameObject.AddComponent<ArcadeButtonPressFeedback>();
            feedback.Configure(iconImage, null, glow, accentColor);
            return button;
        }

        public static Button CreateMenuItem(Transform parent, string label, int order, UnityEngine.Events.UnityAction onClick)
        {
            Button button = CreatePixelButton(parent, "▶ " + label, ArcadeTheme.EnergyYellow, new Vector2(600f, 88f), new Vector2(0f, 180f - (order * 104f)));
            button.onClick.AddListener(onClick);
            return button;
        }

        public static Slider CreatePixelSlider(Transform parent, string name, float value, Vector2 anchorMin, Vector2 anchorMax, UnityEngine.Events.UnityAction<float> onChanged, out Text valueText)
        {
            Image row = CreatePixelPanel(parent, name + "Row", new Color(0.12f, 0.12f, 0.22f, 0.95f), ArcadeTheme.DimGray, anchorMin, anchorMax, new Vector2(4f, 4f));
            CreateArcadeLabel(row.transform, name, ArcadeTheme.SmallSize, TextAnchor.MiddleLeft, ArcadeTheme.White, FontStyle.Bold, new Vector2(0.04f, 0f), new Vector2(0.3f, 1f), Vector2.zero);
            valueText = CreateArcadeLabel(row.transform, Mathf.RoundToInt(value * 100f) + "%", ArcadeTheme.SmallSize, TextAnchor.MiddleRight, ArcadeTheme.EnergyYellow, FontStyle.Bold, new Vector2(0.78f, 0f), new Vector2(0.96f, 1f), Vector2.zero);

            GameObject sliderObject = new GameObject(name + "Slider", typeof(RectTransform), typeof(Slider));
            sliderObject.transform.SetParent(row.transform, false);
            RectTransform sliderRect = sliderObject.GetComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.32f, 0.18f);
            sliderRect.anchorMax = new Vector2(0.76f, 0.82f);
            sliderRect.offsetMin = Vector2.zero;
            sliderRect.offsetMax = Vector2.zero;

            Image track = CreatePanel(sliderObject.transform, "Track", ArcadeTheme.InkBlack, new Vector2(0f, 0.35f), new Vector2(1f, 0.65f));
            Image fill = CreatePanel(track.transform, "Fill", ArcadeTheme.ElectricBlue, Vector2.zero, Vector2.one);
            Image handle = CreatePanel(sliderObject.transform, "Handle", ArcadeTheme.EnergyYellow, new Vector2(0f, 0.08f), new Vector2(0.07f, 0.92f));

            Slider slider = sliderObject.GetComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = Mathf.Clamp01(value);
            slider.fillRect = fill.rectTransform;
            slider.handleRect = handle.rectTransform;
            slider.targetGraphic = handle;
            slider.direction = Slider.Direction.LeftToRight;
            slider.onValueChanged.AddListener(onChanged);
            return slider;
        }

        public static Button CreatePixelToggle(Transform parent, string label, bool enabled, Vector2 anchoredPosition, UnityEngine.Events.UnityAction onClick)
        {
            Button button = CreatePixelButton(parent, $"{label} {(enabled ? "开" : "关")}", enabled ? ArcadeTheme.MilitaryGreen : ArcadeTheme.WarningRed, new Vector2(420f, 76f), anchoredPosition);
            button.onClick.AddListener(onClick);
            return button;
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }
    }
}
