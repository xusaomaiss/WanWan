using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Wanwan.Runtime
{
    public class ArcadeButtonPressFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
    {
        private RectTransform rectTransform;
        private RectTransform faceTransform;
        private Image faceImage;
        private Image shineImage;
        private Image outerRingImage;
        private Color faceNormal;
        private Color facePressed;
        private Color shineNormal;
        private Color ringNormal;
        private Color ringPressed;
        private Vector2 faceStartMin;
        private Vector2 faceStartMax;
        private Vector3 targetScale = Vector3.one;
        public void Configure(Image face, Image shine, Image outerRing, Color accentColor)
        {
            rectTransform = GetComponent<RectTransform>();
            faceImage = face;
            shineImage = shine;
            outerRingImage = outerRing;
            faceTransform = face != null ? face.rectTransform : null;
            faceNormal = face != null ? face.color : ArcadeTheme.PanelBase;
            facePressed = Color.Lerp(faceNormal, ArcadeTheme.InkBlack, 0.42f);
            shineNormal = shine != null ? shine.color : Color.clear;
            ringNormal = outerRing != null ? outerRing.color : accentColor;
            ringPressed = Color.Lerp(accentColor, Color.white, 0.32f);

            if (faceTransform != null)
            {
                faceStartMin = faceTransform.offsetMin;
                faceStartMax = faceTransform.offsetMax;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            targetScale = Vector3.one * 0.94f;
            ApplyPressedVisuals(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            targetScale = Vector3.one;
            ApplyPressedVisuals(false);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            targetScale = Vector3.one;
            ApplyPressedVisuals(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerPress == gameObject)
            {
                targetScale = Vector3.one * 0.94f;
                ApplyPressedVisuals(true);
            }
        }

        private void Update()
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, Time.unscaledDeltaTime * 18f);
        }

        private void ApplyPressedVisuals(bool pressed)
        {
            if (faceTransform != null)
            {
                Vector2 drop = pressed ? new Vector2(0f, -8f) : Vector2.zero;
                faceTransform.offsetMin = faceStartMin + drop;
                faceTransform.offsetMax = faceStartMax + drop;
            }

            if (faceImage != null)
            {
                faceImage.color = pressed ? facePressed : faceNormal;
            }

            if (shineImage != null)
            {
                Color shine = shineNormal;
                shine.a = pressed ? 0.08f : shineNormal.a;
                shineImage.color = shine;
            }

            if (outerRingImage != null)
            {
                outerRingImage.color = pressed ? ringPressed : ringNormal;
            }
        }
    }
}
