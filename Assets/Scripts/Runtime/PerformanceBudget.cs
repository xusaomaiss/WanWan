using UnityEngine;

namespace Wanwan.Runtime
{
    public static class PerformanceBudget
    {
        public const int TargetFrameRate = 60;
        public const float FrameBudgetMilliseconds = 16.7f;
        public const int MemoryWarningThresholdMb = 256;
        public const int TextureMemoryBudgetMb = 96;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ConfigureRuntime()
        {
            ApplyRuntimeSettings();
        }

        public static void ApplyRuntimeSettings()
        {
            QualitySettings.vSyncCount = 0;
            QualitySettings.antiAliasing = 0;
            QualitySettings.shadowDistance = 0f;
            QualitySettings.realtimeReflectionProbes = false;
            Application.targetFrameRate = TargetFrameRate;
            Application.backgroundLoadingPriority = ThreadPriority.Low;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        public static bool IsMemoryOverBudget(long memoryBytes)
        {
            long memoryMb = memoryBytes / (1024L * 1024L);
            return memoryMb >= MemoryWarningThresholdMb;
        }
    }
}
