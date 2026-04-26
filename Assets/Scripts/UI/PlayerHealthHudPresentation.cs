using UnityEngine;

namespace Wanwan.Runtime
{
    public static class PlayerHealthHudPresentation
    {
        public static float GetFillAmount(int currentHealth, int maxHealth)
        {
            if (maxHealth <= 0)
            {
                return 0f;
            }

            return Mathf.Clamp01(currentHealth / (float)maxHealth);
        }

        public static Color GetFillColor(float fillAmount)
        {
            if (fillAmount <= 0.25f)
            {
                return new Color(1f, 0.2f, 0.12f, 0.98f);
            }

            if (fillAmount <= 0.55f)
            {
                return new Color(1f, 0.72f, 0.16f, 0.98f);
            }

            return new Color(0.22f, 1f, 0.62f, 0.98f);
        }
    }
}
