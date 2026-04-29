using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Wanwan.Runtime
{
    public class MenuButtonAnimator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
    {
        private RectTransform rectTransform;
        private Image buttonImage;
        private Image glowImage;
        private Sprite normalSprite;
        private Sprite pressedSprite;
        private Color glowNormal;
        private Color glowHover;
        private float flashTimer;
        private bool hovering;
        private bool pressed;

        public float PressedScale { get; private set; } = 0.95f;

        public void Configure(Image buttonFace, Image glow, Color accentColor, Sprite pressedStateSprite)
        {
            rectTransform = GetComponent<RectTransform>();
            buttonImage = buttonFace;
            glowImage = glow;
            normalSprite = buttonFace != null ? buttonFace.sprite : null;
            pressedSprite = pressedStateSprite;
            glowNormal = glow != null ? glow.color : new Color(accentColor.r, accentColor.g, accentColor.b, 0.12f);
            glowHover = new Color(accentColor.r, accentColor.g, accentColor.b, 0.34f);
            if (glowImage != null)
            {
                glowImage.color = glowNormal;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            pressed = true;
            flashTimer = 0.16f;
            ApplySpriteState();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            pressed = false;
            flashTimer = 0.14f;
            ApplySpriteState();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hovering = false;
            pressed = false;
            ApplySpriteState();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hovering = true;
        }

        private void Update()
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            Vector3 targetScale = Vector3.one * (pressed ? PressedScale : 1f);
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, Time.unscaledDeltaTime * 20f);

            if (glowImage == null)
            {
                return;
            }

            flashTimer = Mathf.Max(0f, flashTimer - Time.unscaledDeltaTime);
            Color targetColor = hovering ? glowHover : glowNormal;
            if (flashTimer > 0f)
            {
                targetColor = Color.Lerp(targetColor, Color.white, Mathf.PingPong(flashTimer * 18f, 1f) * 0.45f);
            }

            glowImage.color = Color.Lerp(glowImage.color, targetColor, Time.unscaledDeltaTime * 16f);
        }

        private void ApplySpriteState()
        {
            if (buttonImage != null && pressedSprite != null)
            {
                buttonImage.sprite = pressed ? pressedSprite : normalSprite;
            }
        }
    }
}
