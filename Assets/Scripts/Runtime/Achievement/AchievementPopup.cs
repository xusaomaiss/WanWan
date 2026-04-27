using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Wanwan.Runtime.Achievement
{
    public class AchievementPopup : MonoBehaviour
    {
        private Transform popupPanel;
        private Text achievementText;
        private Coroutine displayCoroutine;

        public void Initialize(Transform parent)
        {
            var panel = new GameObject("AchievementPanel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            panel.GetComponent<Image>().color = new Color(0.12f, 0.12f, 0.22f, 0.95f);

            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.52f, 0.88f);
            rect.anchorMax = new Vector2(0.96f, 0.95f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            popupPanel = panel.transform;

            achievementText = UiFactory.CreateArcadeLabel(
                popupPanel, "成就解锁", 28, TextAnchor.MiddleCenter,
                ArcadeTheme.EnergyYellow, FontStyle.Bold,
                new Vector2(0.05f, 0.1f), new Vector2(0.95f, 0.9f), Vector2.zero);
        }

        public void ShowAchievement(AchievementType type)
        {
            if (popupPanel == null)
                return;

            if (displayCoroutine != null)
            {
                StopCoroutine(displayCoroutine);
            }

            displayCoroutine = StartCoroutine(AnimateAchievement(type.GetDisplayName()));
        }

        private IEnumerator AnimateAchievement(string label)
        {
            achievementText.text = "成就解锁: " + label;
            popupPanel.gameObject.SetActive(true);

            var rect = popupPanel.GetComponent<RectTransform>();
            Vector3 endPos = rect.anchoredPosition;
            rect.anchoredPosition += Vector2.right * 300f;

            float elapsed = 0f;
            float duration = 0.3f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float eased = 1f - Mathf.Pow(1f - t, 3f);
                rect.anchoredPosition = Vector3.Lerp(rect.anchoredPosition, endPos, eased * 0.5f);
                yield return null;
            }
            rect.anchoredPosition = endPos;

            yield return new WaitForSecondsRealtime(2.5f);

            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                rect.anchoredPosition = Vector3.Lerp(endPos, endPos + Vector3.right * 300f, t);
                yield return null;
            }

            popupPanel.gameObject.SetActive(false);
            displayCoroutine = null;
        }
    }
}
