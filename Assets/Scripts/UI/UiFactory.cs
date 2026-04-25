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
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
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
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 20;
            text.resizeTextMaxSize = 44;

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
