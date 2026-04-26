using UnityEngine;

namespace Wanwan.Runtime
{
    public static class PlayerDamageFeedback
    {
        public static bool ShouldVibrate(bool vibrationEnabled, int damageApplied)
        {
            return vibrationEnabled && damageApplied > 0;
        }

        public static void TriggerVibration(int damageApplied)
        {
            if (!ShouldVibrate(SessionState.VibrationEnabled, damageApplied))
            {
                return;
            }

            Handheld.Vibrate();
        }
    }
}
